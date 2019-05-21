using System;
using System.Collections.Generic;
using System.Text;

namespace DroneChallenge
{
    public static class OrderHelper
    {
        public enum RatingType
        {
            Neutral,
            Detractor,
            Promoter
        }

        public const String TIME_FORMAT = "HH:mm:ss";
        public const int DETRACTOR_HOUR_LIMIT = 4;
        public const int NEUTRAL_HOUR_LIMIT = 2;

        public static String GetOrderString(String id, String destination, DateTime created)
        {
            if (String.IsNullOrWhiteSpace(id)) id = "MW001";
            if (String.IsNullOrWhiteSpace(destination)) destination = "N1E1";
            if (created == null) created = new DateTime();

            return String.Format("{0} {1} {2}", id, destination.ToUpper(), created.ToString(TIME_FORMAT));
        }

        public static String GetOrderString(Order order)
        {
            return GetOrderString(order.Id, order.DestinationEncoded, order.Created);
        }

        public static Order ParseOrder(String orderString, DateTime startTime)
        {
            var tokens = orderString.Trim().Split(' ');

            var idIndex = 0;
            var destinationIndex = 1;
            var timeIndex = 2;
         
            Order newOrder = new Order(tokens[idIndex], ParseCreated(tokens[timeIndex], startTime), tokens[destinationIndex]);

            return newOrder;
        }

        public static DateTime ParseCreated(String createdString, DateTime startTime)
        {
            var tokens = createdString.Split(':');
            var hourIndex = 0;
            var minuteIndex = 1;
            var secondIndex = 2;

            return new DateTime(startTime.Year, startTime.Month, startTime.Day, int.Parse(tokens[hourIndex]), int.Parse(tokens[minuteIndex]), int.Parse(tokens[secondIndex]));
        }

        public static Point ParseLocation(String locationString)
        {
            var axis = ' ';
            var factor = 1;
            var x = 0;
            var y = 0;

            locationString = locationString.ToLower();
            for (var i = 0; i < locationString.Length; i++)
            {
                axis = ' ';

                // Pick the correct direction
                switch (locationString[i])
                {
                    case 'n':
                        axis = 'y';
                        break;

                    case 's':
                        axis = 'y';
                        factor = -1;
                        break;

                    case 'w':
                        axis = 'x';
                        factor = -1;
                        break;

                    case 'e':
                        axis = 'x';
                        break;
                }

                if (axis != ' ')
                {
                    // Get the number for the current direction
                    StringBuilder number = new StringBuilder();
                    i++;
                    while (i < locationString.Length)
                    {
                        var isDigit = Char.IsDigit(locationString[i]);
                        if (isDigit)
                        {
                            number.Append(locationString[i]);
                            i++;
                        }
                        
                        // Build the number
                        if(!isDigit || i == locationString.Length)
                        {
                            // Move back to the letter
                            if (!isDigit) i--;

                            if (number.Length > 0)
                            {
                                if (axis == 'x')
                                {
                                    x = int.Parse(number.ToString()) * factor;
                                }
                                else
                                {
                                    y = int.Parse(number.ToString()) * factor;
                                }
                                
                            }
                            break;
                        }
                    }
                }
            }

            return new Point
            {
                X = x,
                Y = y
            };
        }

        public static RatingType GetRatingType(DeliveredOrder deliveredOrder)
        {
            int decimalPlaces = 2;
            Double elapsedHours = Math.Round((deliveredOrder.DeliveredTime - deliveredOrder.OrderPlaced).TotalHours, decimalPlaces);
            RatingType rating = RatingType.Promoter;

            if (elapsedHours >= DETRACTOR_HOUR_LIMIT)
            {
                rating = RatingType.Detractor;
            }
            else if (elapsedHours >= NEUTRAL_HOUR_LIMIT)
            {
                rating = RatingType.Neutral;
            }

            return rating;
        }
    }
}

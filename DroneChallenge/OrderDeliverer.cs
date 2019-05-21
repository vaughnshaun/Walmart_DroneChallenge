using System;
using System.Collections.Generic;
using System.Text;

namespace DroneChallenge
{
    public class OrderDeliverer
    {
        private List<DeliveredOrder> completedOrders = new List<DeliveredOrder>();
        private IOrderStreamer orderStreamer;
        private Warehouse warehouse;
        private Double promoters;
        private Double detractors;
        private Action<DeliveredOrder> deliveredOrderAction;

        public OrderDeliverer(Warehouse warehouse, IOrderStreamer orderStreamer)
        {
            this.warehouse = warehouse;
            this.orderStreamer = orderStreamer;
        }

        public virtual void ProcessOrder()
        {
            // Artificially advance the time to the next order waiting to be created
            // This is a fail safe, just in case the the processing of orders don't advance time enough
            if (!warehouse.HasOrders)
            {
                orderStreamer.AdvanceTime();
            }

            // Keep processing orders while there are orders
            if (warehouse.HasOrders)
            {
                Order order;

                // If there isn't time for delivery the order should be moved to next day delivery
                if (!warehouse.HasTimeToDeliverNextOrder(orderStreamer.CurrentTime))
                {
                    warehouse.MoveNextOrderToNextDay();
                }
                else if (warehouse.TrySendNextOrder(out order)) // Try to send the order out of the warehouse
                {
                    // Create a delivered order and track its status and times
                    DeliveredOrder outboundOrder = new DeliveredOrder(order.Id);
                    outboundOrder.OrderPlaced = order.Created;
                    outboundOrder.DepartureTime = orderStreamer.CurrentTime;
                    outboundOrder.DeliveredTime = outboundOrder.DepartureTime;

                    // Time traveled to the destination
                    double travelMinutes = warehouse.GetOrderDeliveryMinutes(order);
                    outboundOrder.DeliveredTime = outboundOrder.DeliveredTime.AddMinutes(travelMinutes);

                    // Total time traveled, includes to destination and returning back to the warehouse 
                    travelMinutes += warehouse.GetOrderReturnMinutes(order);

                    completedOrders.Add(outboundOrder);
                    deliveredOrderAction(outboundOrder);

                    switch (outboundOrder.GetRating())
                    {
                        case OrderHelper.RatingType.Detractor:
                            detractors++;
                            break;

                        case OrderHelper.RatingType.Promoter:
                            promoters++;
                            break;
                    }

                    warehouse.DockDrone();

                    // Update the mock global time (will also bring more new orders depending on the time)
                    orderStreamer.AddMinutesToTime(travelMinutes);
                }
            }
        }

        public void OnDeliveredOrder(Action<DeliveredOrder> deliveredAction)
        {
            deliveredOrderAction += deliveredAction;
        }

        /// <summary>
        /// The number of orders successfully delivered
        /// </summary>
        /// <returns>Returns an int for the count of delivered orders</returns>
        public int GetNumberOfCompleted()
        {
            return completedOrders.Count;
        }

        public double GetNps()
        {
            double nps = 0;

            if (completedOrders.Count > 0)
            {
                double promoterPercent = (promoters / completedOrders.Count) * 100;
                double detractorPercent = (detractors / completedOrders.Count) * 100;
                int decimalPlaces = 2;

                nps = Math.Round(promoterPercent - detractorPercent, decimalPlaces);
            }

            return nps;
        }
    }
}

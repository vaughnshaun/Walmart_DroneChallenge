using System;
using System.Collections.Generic;
using System.Text;

namespace DroneChallenge
{
    public class DeliveredOrder
    {
        public String Id {get;}
        public DateTime OrderPlaced { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime DeliveredTime { get; set; }

        public DeliveredOrder(String id)
        {
            this.Id = id;
        }

        public OrderHelper.RatingType GetRating()
        {
            return OrderHelper.GetRatingType(this);
        }
    }
}

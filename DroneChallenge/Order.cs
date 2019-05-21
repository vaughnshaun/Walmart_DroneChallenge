using System;
using System.Collections.Generic;
using System.Text;

namespace DroneChallenge
{
    public class Order: IComparable<Order>
    {
        public DateTime Created { get; }
        public String Id { get; }
        public Point Destination { get; }
        public String DestinationEncoded { get; }

        /// <summary>
        /// Creates an order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="created"></param>
        /// <param name="destinationEncoded"></param>
        public Order(String id, DateTime created, String destinationEncoded)
        {
            this.Id = id;
            this.Created = created;
            this.Destination = OrderHelper.ParseLocation(destinationEncoded);
            this.DestinationEncoded = destinationEncoded;
        }

        /// <summary>
        /// The compare logic isn't done here.
        /// Forced to implement this method because
        /// the priority queue needs IComparable.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Order other)
        {
            throw new NotImplementedException();
        }
    }
}

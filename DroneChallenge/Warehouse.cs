using ConcurrentPriorityQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace DroneChallenge
{
    /// <summary>
    /// Handles queueing and delivery of orders.
    /// TODO: Implement logic to handle invalid orders
    /// </summary>
    public class Warehouse
    {
        private IPriorityQueue<Order, Order> ordersToShipQueue;
        private const Double MINUTES_PER_STEP = 1;
        private const Double MILLISECONDS_PER_STEP = MINUTES_PER_STEP * 60 * 1000;
        private Point Position { get; }
        public Boolean HasOrders { get { return ordersToShipQueue.Count > 0; } }
        private Boolean HasAvailableDrone { get; set; } = true;
        private DateTime endTime;

        public Warehouse(DateTime dateTime) : this(dateTime, new Point{X = 0, Y = 0})
        {
        }

        public Warehouse(DateTime endTime, Point position)
        {
            ordersToShipQueue = new ConcurrentPriorityQueue<Order, Order>(new OrderComparer(this));
            this.Position = position;
            this.endTime = endTime;
        }

        /// <summary>
        /// Adds an order to the warehouse ready to ship queue
        /// </summary>
        /// <param name="order">The order to ship</param>
        public void EnqueueForDelivery(Order order)
        {
            ordersToShipQueue.Enqueue(order, order);
        }

        /// <summary>
        /// Determines if the drone can make it back in time
        /// before the alloted time frame.
        /// </summary>
        /// <returns>Return true if can deliver otherwise false</returns>
        public virtual Boolean HasTimeToDeliverNextOrder(DateTime currentTime)
        {
            Order nextOrder = ordersToShipQueue.Peek();
            Double totalTravelTime = GetOrderDeliveryMinutes(nextOrder) + GetOrderReturnMinutes(nextOrder);

            // The time the drone should be back
            DateTime returnTime = currentTime;
            returnTime = returnTime.AddMinutes(totalTravelTime);
            return DateTime.Compare(returnTime, endTime) <= 0;
        }

        /// <summary>
        /// Removes the next order from the
        /// ready to ship queue.
        /// TODO: Add to the next day queue
        /// </summary>
        public virtual void MoveNextOrderToNextDay()
        {
            ordersToShipQueue.Dequeue();
        }

        /// <summary>
        /// Trys to send an order from the
        /// warehouse if the conditions are meant
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual Boolean TrySendNextOrder(out Order order)
        {
            order = null;
            if (HasAvailableDrone)
            {
                HasAvailableDrone = false;
                order = ordersToShipQueue.Dequeue();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns drone back to the warehouse
        /// TODO: Implement logic for an actual drone and multiple drones
        /// </summary>
        public virtual void DockDrone()
        {
            HasAvailableDrone = true;
        }

        /// <summary>
        /// Compares an order to see how seen it should be shipped
        /// </summary>
        private class OrderComparer : IComparer<Order>
        {
            private Warehouse fromWarehouse;
            public OrderComparer(Warehouse fromWarehouse)
            {
                this.fromWarehouse = fromWarehouse;
            }

            /// <summary>
            /// The priority method that is called when
            /// inserting orders into the ready for shipping queue
            /// </summary>
            /// <param name="a">An order to compare</param>
            /// <param name="b">Another order to compare</param>
            /// <returns></returns>
            public int Compare(Order a, Order b)
            {
                DateTime aTime = a.Created;
                DateTime bTime = b.Created;

                // Add the estimated delivery.
                // This forces shorter trips to have higher priority
                aTime = aTime.AddMinutes(fromWarehouse.GetOrderDeliveryMinutes(a));
                bTime = bTime.AddMinutes(fromWarehouse.GetOrderDeliveryMinutes(b));

                return DateTime.Compare(bTime, aTime);
            }
        }

        /// <summary>
        /// Get the time in minutes
        /// it takes for an order to reach its destination
        /// </summary>
        /// <param name="order">The order to get an estimated delivery time</param>
        /// <returns></returns>
        public virtual Double GetOrderDeliveryMinutes(Order order)
        {
            return GetOrderDistance(order) * MINUTES_PER_STEP;
        }

        /// <summary>
        /// Get the time in minutes it takes for a drone
        /// to return from a order's destination to the warehouse
        /// </summary>
        /// <param name="order">The order to get an estimated return time for</param>
        /// <returns></returns>
        public virtual Double GetOrderReturnMinutes(Order order)
        {
            return GetOrderDistance(order) * (MINUTES_PER_STEP / 2);
        }

        /// <summary>
        /// The distance of an order's destination
        /// from the warehous
        /// </summary>
        /// <param name="order">The order to test</param>
        /// <returns></returns>
        private Double GetOrderDistance(Order order)
        {
            return Math.Abs(order.Destination.X - Position.X) + Math.Abs(order.Destination.Y - Position.Y);
        }
    }
}

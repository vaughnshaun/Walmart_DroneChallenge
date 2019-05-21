using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DroneChallenge
{
    /// <summary>
    /// Streams in created orders based
    /// on user mocked time
    /// </summary>
    public class OrderMockedTimeStreamer : IOrderStreamer
    {
        private Action newOrdersEvent;
        private DateTime currentTime;
        private DateTime currentOrderEndTime; // The cut off time for allowing orders (order must be less than or equal to this)
        private List<Order> allOrders;
        private int currentOrderIndex = 0; // The index of the current order to process
        private int lastCreatedIndex = -1; // The index the last order to be created
        private Boolean streamingStarted = false;

        public bool IsActive { get { return streamingStarted && lastCreatedIndex + 1 < allOrders.Count; } }
        public DateTime CurrentTime { get { return currentTime; } }

        /// <summary>
        /// Create a OrderMockedTimeStreamer
        /// </summary>
        /// <param name="reader">The reader that contains the orders</param>
        /// <param name="currentTime">The mock current time to use</param>
        public OrderMockedTimeStreamer(TextReader reader, DateTime currentTime)
        {
            allOrders = new List<Order>();
            this.currentTime = currentTime;

            // Store all of the orders in a list because business logic is required
            DateTime startTime = currentTime;
            String orderStr;
            while ((orderStr = reader.ReadLine()) != null)
            {
                allOrders.Add(OrderHelper.ParseOrder(orderStr, startTime));
            }
        }

        /// <summary>
        /// A listener for receiving new orders.
        /// </summary>
        /// <param name="action"></param>
        public void OnNewOrders(Action<Order> action)
        {
            newOrdersEvent += () =>
            {
                Boolean isOrderCreated = true; // Determines if the order should be created based on the current time

                // Loop from the most recent order until the time constraint is reached
                // If an order is not created, break since the orders come in ascending order
                for (var i = currentOrderIndex; i < allOrders.Count && isOrderCreated; i++)
                {
                    var curOrder = allOrders[i];
                    isOrderCreated = DateTime.Compare(curOrder.Created, currentOrderEndTime) <= 0;
                    if (isOrderCreated)
                    {
                        action(curOrder);
                        lastCreatedIndex = i;
                    }
                }
            };
        }

        /// <summary>
        /// Adds minutes to the current time
        /// and notifies for new orders if 
        /// certain conditions are met
        /// </summary>
        /// <param name="value">The number of minutes</param>
        public void AddMinutesToTime(double value)
        {
            currentTime = currentTime.AddMinutes(value);
            NotifyNewOrders();
        }

        /// <summary>
        /// Starts the streamer and makes the first call 
        /// to NotifyNewOrders to stream orders to the listeners
        /// </summary>
        public void Start()
        {
            if (!streamingStarted)
            {
                streamingStarted = true;
                NotifyNewOrders();
            }
        }

        /// <summary>
        /// Set the current time to the same time as the next created order
        /// </summary>
        public void AdvanceTime()
        {
            if (IsActive && DateTime.Compare(currentTime, allOrders[currentOrderIndex].Created) < 0)
            {
                currentTime = allOrders[currentOrderIndex].Created;
                NotifyNewOrders();
            }
        }

        /// <summary>
        /// Call all listeners and sends the new orders to the
        /// if the conditions are met.
        /// </summary>
        private void NotifyNewOrders()
        {
            // if the current time equals or exceeds the date of the next created order, process all of the listeners for the incoming orders
            if (IsActive && DateTime.Compare(currentTime, allOrders[currentOrderIndex].Created) >= 0)
            {
                currentOrderEndTime = currentTime;
                newOrdersEvent();
                // move the starting index
                currentOrderIndex = lastCreatedIndex + 1;
            }
        }
    }
}

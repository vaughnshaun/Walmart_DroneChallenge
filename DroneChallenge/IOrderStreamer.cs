using System;
using System.Collections.Generic;
using System.Text;

namespace DroneChallenge
{
    /// <summary>
    /// Interfaces with the DeliveryService
    /// The implementations could be for file streaming
    /// or even pulling from a async resource
    /// </summary>
    public interface IOrderStreamer
    {
        /// <summary>
        /// Listens the streamer for new orders
        /// </summary>
        /// <param name="action">All the new orders</param>
        void OnNewOrders(Action<Order> action);

        /// <summary>
        /// Add time manipulation capabilities.
        /// Generally good for mimicking time.
        /// NOTE: A real stream shouldn't need this
        /// </summary>
        /// <param name="value"></param>
        void AddMinutesToTime(Double value);

        /// <summary>
        /// Start the streamer
        /// </summary>
        void Start();

        /// <summary>
        /// Advance the time to a predetermined
        /// value by the implemented logic.
        /// NOTE: A real stream shouldn't need this
        /// </summary>
        void AdvanceTime();

        /// <summary>
        /// Determines if the streamer should stream.
        /// If false the service will shutdown.
        /// This might be changed in the future
        /// </summary>
        Boolean IsActive { get; }

        /// <summary>
        /// Get the current time.
        /// This can be any implementation.
        /// NOTE: For real streamers, DateTime.Now
        /// is reccommended.
        /// </summary>
        DateTime CurrentTime { get; }
    }
}

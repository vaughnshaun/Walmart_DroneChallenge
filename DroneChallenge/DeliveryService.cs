using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DroneChallenge
{
    public class DeliveryService
    {
        private OrderDeliverer orderDeliverer;

        public Boolean IsRunning { get; private set; } = false;
        //private const int RUN_INTERVAL_MS = 0;

        public DeliveryService(OrderDeliverer orderDeliverer)
        {
            this.orderDeliverer = orderDeliverer;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Start(Action processedAction = null)
        {
            if (!IsRunning)
            {
                IsRunning = true;

                // Default callback for when an the delivery service executes a cycle
                if (processedAction == null)
                {
                    processedAction = () => { };
                }

                // Run until the user calls stop
                while (IsRunning)
                {
                    orderDeliverer.ProcessOrder();
                    processedAction();
                    // Could add a sleep, timer, or async loop to make this a real scheduler
                    /*if (RUN_INTERVAL_MS > 0)
                    {
                        Thread.Sleep(RUN_INTERVAL_MS);
                    }*/
                }
            }
        }
    }
}

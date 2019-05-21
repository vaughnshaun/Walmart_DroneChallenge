using System;
using System.Collections.Generic;
using System.IO;

namespace DroneChallenge
{
    /// <summary>
    /// This is where all of the configuration code
    /// should go.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            IOrderStreamer orderStreamer = null;
            // Make sure a file is present
            if (args.Length == 0)
            {
                Console.WriteLine("A file path is required to run this program.");
            }
            else
            {
                // The time range for deliveries
                DateTime today = new DateTime();
                DateTime currentTime = new DateTime(today.Year, today.Month, today.Day, 6, 0, 0);
                DateTime endTime = new DateTime(today.Year, today.Month, today.Day, 22, 0, 0);

                // A file reader
                var filePath = args[0];
                TextReader orderReader = CreateFileReader(filePath);

                // Run the delivery service if everything works out
                if (orderReader != null)
                {
                    orderStreamer = new OrderMockedTimeStreamer(orderReader, currentTime);
                    // Delivers orders (NOTE: I'm passing orderStreamer because of the time restriction)
                    Warehouse warehouse = new Warehouse(endTime);
                    OrderDeliverer orderDeliverer = new OrderDeliverer(warehouse, orderStreamer);

                    // Listen for incoming orders
                    orderStreamer.OnNewOrders(newOrder =>
                    {
                        warehouse.EnqueueForDelivery(newOrder);
                    });

                    orderStreamer.Start();

                    // Write the delivery results
                    var fileOutput = "DroneResults.txt";
                    using (StreamWriter file = new StreamWriter(fileOutput, false))
                    {
                        // Whenever a package is delivered, write to the results file
                        orderDeliverer.OnDeliveredOrder(deliveredOrder =>
                        {
                            file.WriteLine(String.Format("{0} {1}", deliveredOrder.Id, deliveredOrder.DepartureTime.ToString(OrderHelper.TIME_FORMAT)));
                        });

                        // Start the service: Will continously run the order deliverer
                        DeliveryService deliveryService = new DeliveryService(orderDeliverer);
                        deliveryService.Start(() =>
                        {
                            // Force the serve to stop when the stream is no longer active 
                            // and the warehouse doesn't have any more orders
                            // NOTE: The orderStream.IsActive can be implemented to run indefinitely
                            if (!orderStreamer.IsActive && !warehouse.HasOrders)
                            {
                                deliveryService.Stop();
                            }
                        });
                        file.WriteLine("NPS: " + orderDeliverer.GetNps());
                        Console.WriteLine("Results located at " + Path.GetFullPath(fileOutput));
                    }

                    orderReader.Close();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Creates a file reader
        /// </summary>
        /// <param name="filePath">Takes a file path to the file</param>
        /// <returns>Returns a TextReader</returns>
        private static TextReader CreateFileReader(String filePath)
        {
            TextReader reader = null;
            try
            {
                reader = File.OpenText(filePath);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("File not found: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception: " + ex.Message);
            }

            return reader;
        }
    }
}

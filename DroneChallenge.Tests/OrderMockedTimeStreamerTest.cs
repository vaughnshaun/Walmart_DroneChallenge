using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace DroneChallenge.Tests
{
    public class OrderMockedTimeStreamerTest
    {
        private OrderMockedTimeStreamer orderStreamer;
        DateTime currentTime;
        public OrderMockedTimeStreamerTest()
        {
            //orderStreamer = new OrderMockedTimeStreamer()
            currentTime = new DateTime(1, 1, 1, 6, 0, 0);
        }

        [Fact]
        public void Should_NotNotifyNewOrders_When_NotStarted()
        {
            String order = OrderHelper.GetOrderString(null, null, currentTime);
            var isNotified = false;

            using (var reader = new StringReader(order))
            {
                orderStreamer = new OrderMockedTimeStreamer(reader, currentTime);

                orderStreamer.OnNewOrders(newOrder =>
                {
                    isNotified = true;
                });
            }

            Assert.False(isNotified);
        }

        [Fact]
        public void Should_NotifyNewOrders_When_Started()
        {
            String order = OrderHelper.GetOrderString(null, null, currentTime);
            var isNotified = false;

            using (var reader = new StringReader(order))
            {
                orderStreamer = new OrderMockedTimeStreamer(reader, currentTime);

                orderStreamer.OnNewOrders(newOrder =>
                {
                    isNotified = true;
                });
                orderStreamer.Start();
            }

            Assert.True(isNotified);
        }

        [Fact]
        public void Should_NotNotifyNewOrders_When_OrderHasAFutureDate()
        {
            String order = OrderHelper.GetOrderString(null, null, currentTime.AddHours(1));
            var isNotified = false;

            using (var reader = new StringReader(order))
            {
                orderStreamer = new OrderMockedTimeStreamer(reader, currentTime);

                orderStreamer.OnNewOrders(newOrder =>
                {
                    isNotified = true;
                });
                orderStreamer.Start();
            }

            Assert.False(isNotified);
        }

        [Theory]
        [InlineData("MW001 S1E1 05:05:11", "MW002 S1E1 09:05:11")]
        [InlineData("MW001 S1E1 09:05:11", "MW002 S1E1 22:05:11")]
        public void Should_NotifyNewOrders_When_CurrentTimeCanBeAdvanced(params String[] orders)
        {
            currentTime = new DateTime(1, 1, 1, 1, 1, 1);
            using (var reader = new StringReader(String.Join("\n", orders)))
            {
                orderStreamer = new OrderMockedTimeStreamer(reader, currentTime);

                orderStreamer.Start();
                var orderIndex = 0;
                orderStreamer.OnNewOrders(newOrder =>
                {
                    Assert.Equal(orders[orderIndex], OrderHelper.GetOrderString(newOrder));
                    orderIndex++;
                });
                for (var i = 0; i < orders.Length; i++)
                {
                    orderStreamer.AdvanceTime();
                }
                
                Assert.Equal(orderIndex, orders.Length);
            }
        }

        [Theory]
        [InlineData(1, "MW002 S1E1 01:40:00")]
        [InlineData(9, "MW002 S1E1 10:00:00")]
        public void Should_NotifyNewOrders_When_AddedTimeGreaterEqualOrderCreated(int startHour, String order)
        {
            currentTime = new DateTime();
            currentTime.AddHours(startHour);

            using (var reader = new StringReader(order))
            {
                orderStreamer = new OrderMockedTimeStreamer(reader, currentTime);

                orderStreamer.Start();
                Order resultOrder = null;
                orderStreamer.OnNewOrders(newOrder =>
                {
                    resultOrder = newOrder;
                });
                orderStreamer.AddMinutesToTime(999);

                Assert.NotNull(resultOrder);
            }
        }

        [Theory]
        [InlineData(1, "MW002 S1E1 01:40:00")]
        [InlineData(9, "MW002 S1E1 10:00:00")]
        public void Should_NotNotifyNewOrders_When_AddedTimeLessOrderCreated(int startHour, String order)
        {
            currentTime = new DateTime();
            currentTime.AddHours(startHour);

            using (var reader = new StringReader(order))
            {
                orderStreamer = new OrderMockedTimeStreamer(reader, currentTime);

                orderStreamer.Start();
                Order resultOrder = null;
                orderStreamer.OnNewOrders(newOrder =>
                {
                    resultOrder = newOrder;
                });
                orderStreamer.AddMinutesToTime(1);

                Assert.Null(resultOrder);
            }
        }
    }
}

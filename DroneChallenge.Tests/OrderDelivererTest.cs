using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DroneChallenge.Tests
{
    public class OrderDelivererTest
    {
        private Warehouse warehouse;
        private IOrderStreamer orderStreamer;
        private OrderDeliverer orderDeliverer;

        public OrderDelivererTest()
        {
            warehouse = Substitute.For<Warehouse>(new DateTime());
            orderStreamer = Substitute.For<IOrderStreamer>();
            orderDeliverer = new OrderDeliverer(warehouse, orderStreamer);
        }

        [Fact]
        public void Should_AdvancedTime_When_NoOrdersToDeliver()
        {
            orderDeliverer.ProcessOrder();

            orderStreamer.Received().AdvanceTime();
            Assert.False(warehouse.HasOrders);
        }

        [Fact]
        public void Should_MoveOrderToNextDay_When_OrderNotWithinWarehouseHours()
        {
            orderDeliverer.ProcessOrder();
            Order order = new Order("Id", new DateTime(), "S1E2");
            warehouse.EnqueueForDelivery(order);
            warehouse.HasTimeToDeliverNextOrder(Arg.Any<DateTime>()).Returns(false);
        }

        /*[Fact]
        public void Should_SuccessfullyDeliverAnOrder_When_OrderCanBeSent()
        {
            Order order = new Order("Id", new DateTime(), "S1E2");
            warehouse.EnqueueForDelivery(order);
            warehouse.HasTimeToDeliverNextOrder(Arg.Any<DateTime>()).Returns(true);
            warehouse.TrySendNextOrder(null).Returns(true);
            var numberBeforeSend = orderDeliverer.GetNumberOfCompleted();

            orderDeliverer.ProcessOrder();

            Assert.Equal(0, numberBeforeSend);
            Assert.Equal(1, orderDeliverer.GetNumberOfCompleted());
        }*/
    }
}
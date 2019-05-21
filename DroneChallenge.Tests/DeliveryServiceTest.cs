using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DroneChallenge.Tests
{
    public class DeliveryServiceTest
    {
        DeliveryService deliveryService;
        OrderDeliverer deliverer;

        public DeliveryServiceTest()
        {
            Warehouse warehouse = new Warehouse(new DateTime());
            IOrderStreamer orderStreamer = Substitute.For<IOrderStreamer>();
            deliverer = Substitute.For<OrderDeliverer>(warehouse, orderStreamer);

            deliveryService = new DeliveryService(deliverer);
        }

        [Fact]
        public void Should_ProcessOrder_When_StartIsCalled()
        {
            deliveryService.Start(deliveryService.Stop);
            deliverer.Received().ProcessOrder();
        }

        [Fact]
        public void Should_NotProcessOrder_When_StartIsNotCalled()
        {
            deliverer.DidNotReceive().ProcessOrder();
        }
    }
}

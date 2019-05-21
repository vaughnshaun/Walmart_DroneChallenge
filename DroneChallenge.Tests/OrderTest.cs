using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DroneChallenge.Tests
{
    public class OrderTest
    {
        [Fact]
        public void Should_SetPropertiesCorrectly_When_OrderCreated()
        {
            var expectedId = "MW001";
            var expectedTime = new DateTime();
            var expectedDest = "S1W2";
            Order order = new Order(expectedId, expectedTime, expectedDest);
            Assert.Equal(expectedId, order.Id);
            Assert.Equal(expectedTime, order.Created);
            Assert.Equal(expectedDest, order.DestinationEncoded);
            Assert.Equal(order.Destination.Y, -1);
            Assert.Equal(order.Destination.X, -2);
        }
    }
}

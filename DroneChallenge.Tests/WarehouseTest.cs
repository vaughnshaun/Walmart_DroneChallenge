using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DroneChallenge.Tests
{
    public class WarehouseTest
    {
        private Warehouse warehouse;
        private Order genericOrder;

        public WarehouseTest()
        {
            warehouse = new Warehouse(new DateTime());
            genericOrder = new Order("generic", new DateTime(), "N11E02");
        }

        [Fact]
        public void Should_HaveTimeToDeliverNextOrder_When_OrderInTimeRange()
        {
            DateTime endTime = new DateTime();
            DateTime currentTime = endTime;
            endTime = endTime.AddMinutes(5);
            Warehouse warehouse = new Warehouse(endTime);
            Order order = new Order("MW001", new DateTime(), "S01E2");
            warehouse.EnqueueForDelivery(order);

            Assert.True(warehouse.HasTimeToDeliverNextOrder(currentTime));
        }

        [Fact]
        public void Should_NotHaveTimeToDeliverNextOrder_When_OrderNotInTimeRange()
        {
            DateTime endTime = new DateTime();
            DateTime currentTime = endTime;
            endTime = endTime.AddMinutes(5);
            Warehouse warehouse = new Warehouse(endTime);
            Order order = new Order("MW001", new DateTime(), "S01E7");
            warehouse.EnqueueForDelivery(order);

            Assert.False(warehouse.HasTimeToDeliverNextOrder(currentTime));
        }

        [Fact]
        public void Should_AddOrderToShippingReadyQueue_When_OrderIsValid()
        {
            Assert.False(warehouse.HasOrders);
            warehouse.EnqueueForDelivery(genericOrder);
            Assert.True(warehouse.HasOrders);
        }

        [Fact]
        public void Should_MoveNextOrderToNextDay_When_OrderIsPresent()
        {
            Assert.False(warehouse.HasOrders);
            warehouse.EnqueueForDelivery(genericOrder);
            warehouse.MoveNextOrderToNextDay();
            Assert.False(warehouse.HasOrders);
        }

        [Fact]
        public void Should_SendNextOrder_When_ADroneIsAvailable()
        {
            warehouse.EnqueueForDelivery(genericOrder);
            Order orderToDeliver;
            Assert.True(warehouse.TrySendNextOrder(out orderToDeliver));
        }

        [Fact]
        public void Should_NotSendNextOrder_When_NoDronesAreAvailable()
        {
            warehouse.EnqueueForDelivery(genericOrder);
            Order anotherOrder = new Order("generic2", new DateTime(), "S3E11");
            warehouse.EnqueueForDelivery(anotherOrder);

            // Send an order with return the drone
            Order sentGenericOrder;
            warehouse.TrySendNextOrder(out sentGenericOrder);

            Order sentAnotherOrder;
            Assert.False(warehouse.TrySendNextOrder(out sentAnotherOrder));
        }

        [Fact]
        public void Should_AllowSendNextOrder_When_ADroneReturns()
        {
            warehouse.EnqueueForDelivery(genericOrder);
            Order sentGenericOrder;

            // Send and immediately return the drone
            warehouse.TrySendNextOrder(out sentGenericOrder);
            warehouse.DockDrone();

            // Verify that another send will work
            warehouse.EnqueueForDelivery(genericOrder);
            Assert.True(warehouse.TrySendNextOrder(out sentGenericOrder));
        }

        [Fact]
        public void Should_PrioritizeOrderForDelivery_WhenInsertedFirst()
        {
            // The order that
            DateTime currentTime = new DateTime();
            Order firstOrder = new Order("first", currentTime, "S0E0");
            Order secondOrder = new Order("last", currentTime, "S0E0");
            warehouse.EnqueueForDelivery(firstOrder);
            warehouse.EnqueueForDelivery(secondOrder);

            // Verify that the first inserted order is sent first
            Order sentOrder;
            warehouse.TrySendNextOrder(out sentOrder);
            Assert.Equal(firstOrder.Id, sentOrder.Id);
        }

        [Fact]
        public void Should_PrioritizeOrderForDelivery_When_EarliestCreated()
        {
            // The order that
            DateTime currentTime = new DateTime();
            Order earliestOrder = new Order("early", currentTime, "S0E0");
            Order latestOrder = new Order("late", currentTime.AddMinutes(30), "S0E0");
            warehouse.EnqueueForDelivery(latestOrder);
            warehouse.EnqueueForDelivery(earliestOrder);

            // Verify that the earliest order is sent first
            Order sentOrder;
            warehouse.TrySendNextOrder(out sentOrder);
            Assert.Equal(earliestOrder.Id, sentOrder.Id);
        }

        [Fact]
        public void Should_PrioritizeOrderForDelivery_When_ShortestDistance()
        {
            // The order that
            DateTime currentTime = new DateTime();
            Order shortOrder = new Order("short", currentTime, "S1E1");
            Order farOrder = new Order("far", currentTime, "S20E20");
            warehouse.EnqueueForDelivery(farOrder);
            warehouse.EnqueueForDelivery(shortOrder);

            // Verify that the order with the shortest distance is sent first
            Order sentOrder;
            warehouse.TrySendNextOrder(out sentOrder);
            Assert.Equal(shortOrder.Id, sentOrder.Id);
        }

        [Fact]
        public void Should_PrioritizeOrderForDelivery_When_ShortestDistanceAndCreated()
        {
            // The order that
            DateTime currentTime = new DateTime();
            Order orderWinner = new Order("winner", currentTime.AddMinutes(50), "S1E1");
            Order orderLoser = new Order("loser", currentTime, "S20E35");
            warehouse.EnqueueForDelivery(orderLoser);
            warehouse.EnqueueForDelivery(orderWinner);

            // Verify that the order with the least cost is sent first
            Order sentOrder;
            warehouse.TrySendNextOrder(out sentOrder);
            Assert.Equal(orderWinner.Id, sentOrder.Id);
        }
    }
}

using ElevatorSimulation.Core.Enums;
using ElevatorSimulation.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ElevatorSimulation.Tests
{
    [TestFixture]
    public class ElevatorTests
    {
        private Elevator _elevator;
        private Mock<ILogger<Elevator>> _mockLogger; // Mock logger

        [SetUp]
        public void SetUp()
        {
            // Create a mock logger
            _mockLogger = new Mock<ILogger<Elevator>>();

            // Initialize the elevator with the mock logger
            _elevator = new Elevator(1, 6, ElevatorType.Standard, _mockLogger.Object);
        }

        [Test]
        public void MoveToFloor_ShouldMoveToCorrectFloor_WhenFloorIsAbove()
        {
            // Arrange
            int destinationFloor = 5;

            // Act
            _elevator.MoveToFloor(destinationFloor);

            // Assert
            Assert.AreEqual(destinationFloor, _elevator.CurrentFloor);
            Assert.AreEqual(ElevatorState.Idle, _elevator.State); // Should return to Idle after moving
        }

        [Test]
        public void MoveToFloor_ShouldMoveToCorrectFloor_WhenFloorIsBelow()
        {
            // Arrange
            _elevator.MoveToFloor(5); // Start at a higher floor
            int destinationFloor = 2;

            // Act
            _elevator.MoveToFloor(destinationFloor);

            // Assert
            Assert.AreEqual(destinationFloor, _elevator.CurrentFloor);
            Assert.AreEqual(ElevatorState.Idle, _elevator.State); // Should return to Idle after moving
        }

        [Test]
        public void AddPassengers_ShouldIncreasePassengerCount_WhenCapacityAllows()
        {
            // Arrange
            int passengersToAdd = 3;

            // Act
            _elevator.AddPassengers(passengersToAdd);

            // Assert
            Assert.AreEqual(passengersToAdd, _elevator.CurrentPassengers);
        }

        [Test]
        public void AddPassengers_ShouldNotExceedCapacity()
        {
            // Arrange
            int passengersToAdd = 10; // Exceeds capacity

            // Act
            _elevator.AddPassengers(passengersToAdd);

            // Assert
            Assert.AreEqual(0, _elevator.CurrentPassengers); // No passengers should be added
        }

        [Test]
        public void RemovePassengers_ShouldDecreasePassengerCount()
        {
            // Arrange
            int passengersToAdd = 4;
            _elevator.AddPassengers(passengersToAdd);
            int passengersToRemove = 2;

            // Act
            _elevator.RemovePassengers(passengersToRemove);

            // Assert
            Assert.AreEqual(2, _elevator.CurrentPassengers); // 4 - 2 = 2
        }

        [Test]
        public void RemovePassengers_ShouldNotRemoveMoreThanCurrentPassengers()
        {
            // Arrange
            int passengersToAdd = 3;
            _elevator.AddPassengers(passengersToAdd);
            int passengersToRemove = 5; // Exceeds current passengers

            // Act
            _elevator.RemovePassengers(passengersToRemove);

            // Assert
            Assert.AreEqual(0, _elevator.CurrentPassengers, "Passenger count should not drop below zero.");
        }
    }
}

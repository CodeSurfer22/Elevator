using System;
using System.Collections.Generic;
using ElevatorSimulation.Controllers;
using ElevatorSimulation.Core.Enums;
using ElevatorSimulation.Core.Interfaces;

namespace ElevatorSimulation
{
    public class Building
    {
        public int TotalFloors { get; }
        public List<IElevator> Elevators { get; } // Public property to access elevators
        private ElevatorController _elevatorController;

        // Constructor to initialize the building with elevators and an elevator controller
        public Building(int totalFloors, List<IElevator> elevators, ElevatorController elevatorController)
        {
            TotalFloors = totalFloors;
            Elevators = elevators ?? throw new ArgumentNullException(nameof(elevators));
            _elevatorController = elevatorController ?? throw new ArgumentNullException(nameof(elevatorController));
        }

        // Method to call an elevator for passengers
        public void CallElevator(int currentFloor, int destinationFloor, int passengers)
        {
            // Validate input
            if (currentFloor < 1 || currentFloor > TotalFloors)
            {
                Console.WriteLine($"Invalid floor number. Please select a floor between 1 and {TotalFloors}.");
                return;
            }

            if (destinationFloor < 1 || destinationFloor > TotalFloors)
            {
                Console.WriteLine($"Invalid destination floor. Please select a floor between 1 and {TotalFloors}.");
                return;
            }

            if (passengers < 1)
            {
                Console.WriteLine("The number of passengers must be at least 1.");
                return;
            }

            // Attempt to dispatch an elevator
            var success = _elevatorController.DispatchElevator(currentFloor, destinationFloor, passengers);

            if (!success)
            {
                // Simulate passengers exiting elevators to free up capacity
                Console.WriteLine("No elevators are currently available. Simulating passenger exits...");
                _elevatorController.SimulatePassengerExits();

                // Retry dispatching the elevator
                success = _elevatorController.DispatchElevator(currentFloor, destinationFloor, passengers);

                if (!success)
                {
                    Console.WriteLine("Still no elevators available. Please wait.");
                }
            }
        }

        // Method to display the status of all elevators
        public void DisplayElevatorStatus()
        {
            foreach (var elevator in Elevators)
            {
                Console.WriteLine($"Elevator {elevator.Id} - Floor: {elevator.CurrentFloor}, " +
                                  $"State: {elevator.State}, Passengers: {elevator.CurrentPassengers}/{elevator.Capacity}");
            }
        }

        // Private helper method to find the nearest available elevator
        private IElevator FindNearestAvailableElevator(int currentFloor, int passengers)
        {
            IElevator nearestElevator = null;
            int shortestDistance = int.MaxValue;

            foreach (var elevator in Elevators)
            {
                if (elevator.State == ElevatorState.Idle && elevator.CanAccommodate(passengers))
                {
                    int distance = Math.Abs(elevator.CurrentFloor - currentFloor);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestElevator = elevator;
                    }
                }
            }

            return nearestElevator;
        }
    }
}

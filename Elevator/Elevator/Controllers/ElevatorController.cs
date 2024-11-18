using ElevatorSimulation.Core.Enums;
using System;
using System.Collections.Generic;

namespace ElevatorSimulation.Controllers
{
    public class ElevatorController
    {
        private List<Elevator> _elevators;

        public ElevatorController(List<Elevator> elevators)
        {
            _elevators = elevators;
        }

        public bool DispatchElevator(int currentFloor, int destinationFloor, int passengers)
        {
            var availableElevator = FindNearestAvailableElevator(currentFloor, passengers);

            if (availableElevator != null)
            {
                // Set direction based on current floor and destination
                if (currentFloor > availableElevator.CurrentFloor)
                {
                    availableElevator.State = ElevatorState.MovingUp; // Set the elevator to moving up
                }
                else
                {
                    availableElevator.State = ElevatorState.MovingDown; // Set the elevator to moving down
                }

                // Dispatch the elevator
                availableElevator.SetIsMoving(true);
                availableElevator.AddPassengers(passengers); // Add passengers when elevator is dispatched
                availableElevator.MoveToFloor(currentFloor);

                Console.WriteLine($"Elevator {availableElevator.Id} dispatched to floor {currentFloor} with {passengers} passengers.");

                // Move to destination floor
                availableElevator.MoveToFloor(destinationFloor);
                availableElevator.SetIsMoving(false); // Stop elevator after it arrives
                availableElevator.RemovePassengers(passengers); // Remove passengers when elevator reaches destination

                Console.WriteLine($"Elevator {availableElevator.Id} has arrived at floor {destinationFloor}.");
                return true;
            }

            return false; // No available elevator
        }

        private Elevator FindNearestAvailableElevator(int currentFloor, int passengers)
        {
            Elevator nearestElevator = null;
            int shortestDistance = int.MaxValue;

            foreach (var elevator in _elevators)
            {
                if (elevator.State == ElevatorState.Idle && elevator.CanAccommodate(passengers)) // Check if elevator is idle
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

        public void SimulatePassengerExits()
        {
            foreach (var elevator in _elevators)
            {
                if (elevator.CurrentPassengers > 0)
                {
                    int exitingPassengers = new Random().Next(1, elevator.CurrentPassengers + 1);
                    elevator.RemovePassengers(exitingPassengers);
                    Console.WriteLine($"Elevator {elevator.Id}: {exitingPassengers} passengers exited. Current load: {elevator.CurrentPassengers}/{elevator.Capacity}.");
                }
            }
        }

        public void UpdateElevatorStatus()
        {
            foreach (var elevator in _elevators)
            {
                Console.WriteLine($"Elevator {elevator.Id} - Floor: {elevator.CurrentFloor}, State: {elevator.State}, " +
                                  $"Moving: {(elevator.State == ElevatorState.Idle ? "No" : "Yes")}, Passengers: {elevator.CurrentPassengers}/{elevator.Capacity}");
            }
        }
    }
}

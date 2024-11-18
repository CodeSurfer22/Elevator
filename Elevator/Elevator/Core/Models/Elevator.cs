using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Core.Enums;
using Microsoft.Extensions.Logging;
using System;

namespace ElevatorSimulation
{
    public class Elevator : IElevator
    {
        private readonly ILogger<Elevator> _logger;

        public int Id { get; }
        public int CurrentFloor { get; private set; }
        public int Capacity { get; }
        public ElevatorType Type { get; }
        public ElevatorState State { get; set; }
        public int CurrentPassengers { get; private set; }

        public Elevator(int id, int capacity, ElevatorType type, ILogger<Elevator> logger)
        {
            Id = id;
            Capacity = capacity;
            Type = type;
            CurrentFloor = 1; // Default to the first floor
            CurrentPassengers = 0;
            State = ElevatorState.Idle; // Initially idle
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool CanAccommodate(int passengers)
        {
            return CurrentPassengers + passengers <= Capacity;
        }

        public void RequestFloor(int floor)
        {
            _logger.LogInformation($"Elevator {Id} requested to go to floor {floor}.");
            MoveToFloor(floor);
        }

        public void MoveToNextFloor()
        {
            if (State == ElevatorState.MovingUp)
            {
                CurrentFloor++;
            }
            else if (State == ElevatorState.MovingDown)
            {
                CurrentFloor--;
            }
            _logger.LogInformation($"Elevator {Id} is now on floor {CurrentFloor}.");
        }

        public void OpenDoor()
        {
            _logger.LogInformation($"Elevator {Id} doors are opening.");
        }

        public void CloseDoor()
        {
            _logger.LogInformation($"Elevator {Id} doors are closing.");
        }

        public void AddPassengers(int passengers)
        {
            if (CanAccommodate(passengers))
            {
                CurrentPassengers += passengers;
                _logger.LogInformation($"Added {passengers} passengers to Elevator {Id}. Current passengers: {CurrentPassengers}/{Capacity}.");
            }
            else
            {
                _logger.LogWarning($"Cannot add {passengers} passengers. Elevator {Id} has insufficient capacity. Current passengers: {CurrentPassengers}/{Capacity}.");
            }
        }

        public void RemovePassengers(int passengers)
        {
            if (passengers <= 0)
            {
                _logger.LogWarning($"Cannot remove a non-positive number of passengers from Elevator {Id}.");
                return;
            }

            if (CurrentPassengers >= passengers)
            {
                CurrentPassengers -= passengers;
                _logger.LogInformation($"Removed {passengers} passengers from Elevator {Id}. Current passengers: {CurrentPassengers}/{Capacity}.");
            }
            else
            {
                _logger.LogWarning($"Cannot remove {passengers} passengers. Elevator {Id} only has {CurrentPassengers} passengers.");
                CurrentPassengers = 0; // Ensure passenger count does not go negative
                _logger.LogInformation($"All passengers removed from Elevator {Id}. Current passengers: 0.");
            }
        }

        public void SetIsMoving(bool isMoving)
        {
            if (isMoving)
            {
                State = ElevatorState.MovingUp; // Example: always moving up for this simulation
                _logger.LogInformation($"Elevator {Id} is now moving.");
            }
            else
            {
                State = ElevatorState.Idle;
                _logger.LogInformation($"Elevator {Id} is now idle.");
            }
        }

        public void MoveToFloor(int floor)
        {
            if (CurrentFloor < floor)
            {
                State = ElevatorState.MovingUp;
                _logger.LogInformation($"Elevator {Id} is moving up to floor {floor}.");
                while (CurrentFloor < floor)
                {
                    MoveToNextFloor();
                }
            }
            else if (CurrentFloor > floor)
            {
                State = ElevatorState.MovingDown;
                _logger.LogInformation($"Elevator {Id} is moving down to floor {floor}.");
                while (CurrentFloor > floor)
                {
                    MoveToNextFloor();
                }
            }

            State = ElevatorState.Idle;
            _logger.LogInformation($"Elevator {Id} has arrived at floor {CurrentFloor}.");
        }
    }
}

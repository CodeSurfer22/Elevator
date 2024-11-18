using ElevatorSimulation.Core.Enums;

namespace ElevatorSimulation.Core.Interfaces
{
    public interface IElevator
    {
        int Id { get; }
        int CurrentFloor { get; }
        int Capacity { get; }
        ElevatorType Type { get; }
        ElevatorState State { get; }
        int CurrentPassengers { get; } // Add this property

        bool CanAccommodate(int passengers); // Add this method

        void RequestFloor(int floor);
        void MoveToNextFloor();
        void OpenDoor();
        void CloseDoor();
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ElevatorSimulation;
using ElevatorSimulation.Core.Enums;
using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Controllers;

class Program
{
    static void Main(string[] args)
    {
        // Build service provider
        var serviceProvider = ConfigureServices();

        // Logger instance
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            Console.WriteLine("Enter the number of floors in the building:");
            int totalFloors = GetValidIntegerInput(1, 300);

            Console.WriteLine("Enter the number of elevators in the building:");
            int elevatorCount = GetValidIntegerInput(1, 300);

            var elevators = new List<Elevator>();

            for (int i = 1; i <= elevatorCount; i++)
            {
                Console.WriteLine($"Enter the capacity for Elevator {i}:");
                int capacity = GetValidIntegerInput(1, 24);

                Console.WriteLine($"Enter the type for Elevator {i} (Standard, HighSpeed, Freight):");
                ElevatorType type;
                while (!Enum.TryParse(Console.ReadLine(), true, out type))
                {
                    Console.WriteLine("Invalid type. Please choose from Standard, HighSpeed, or Freight.");
                }

                var elevatorLogger = serviceProvider.GetRequiredService<ILogger<Elevator>>();
                elevators.Add(new Elevator(i, capacity, type, elevatorLogger));
            }

            // Convert List<Elevator> to List<IElevator>
            List<IElevator> elevatorList = new List<IElevator>(elevators);

            var elevatorController = new ElevatorController(elevators);
            var building = new Building(totalFloors, elevatorList, elevatorController);

            // Main loop
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Elevator Simulation Menu:");
                Console.WriteLine("1. Call an Elevator");
                Console.WriteLine("2. Display Real-Time Elevator Status");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");

                int choice = GetValidIntegerInput(1, 3);

                switch (choice)
                {
                    case 1:
                        CallElevator(building);
                        break;
                    case 2:
                        building.DisplayElevatorStatus();
                        Console.WriteLine("Press any key to return to the menu...");
                        Console.ReadKey();
                        break;
                    case 3:
                        Console.WriteLine("Exiting Elevator Simulation...");
                        return;
                }

                Thread.Sleep(500);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred.");
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .BuildServiceProvider();
    }

    static void CallElevator(Building building)
    {
        try
        {
            Console.WriteLine("Enter the floor you are currently on:");
            int currentFloor = GetValidIntegerInput(1, building.TotalFloors);

            Console.WriteLine("Enter the floor you want to go to:");
            int destinationFloor = GetValidIntegerInput(1, building.TotalFloors);

            if (currentFloor == destinationFloor)
            {
                Console.WriteLine("You are already on the selected floor.");
                return;
            }

            Console.WriteLine("Enter the number of passengers:");
            int passengers = GetValidIntegerInput(1, 30);

            // Call the elevator via the Building class
            building.CallElevator(currentFloor, destinationFloor, passengers);

            Console.WriteLine("Elevator dispatched. Please wait...");
            Console.WriteLine("Press any key to return to the menu.");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while calling the elevator: {ex.Message}");
        }
    }


    static int GetValidIntegerInput(int min, int max)
    {
        int result;
        while (true)
        {
            Console.Write($"Enter a number between {min} and {max}: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out result) && result >= min && result <= max)
            {
                return result;
            }

            Console.WriteLine($"Invalid input. Please enter a valid number between {min} and {max}.");
        }
    }

}

using System;
using System.Collections.Generic;
using Roommates.Repositories;
using Roommates.Models;

namespace Roommates
{
    class Program
    {
        //  This is the address of the database.
        //  We define it here as a constant since it will never change.
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true;TrustServerCertificate=true;";


        static void Main(string[] args)
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            ChoreRepository choreRepo = new ChoreRepository(CONNECTION_STRING);
            RoomateRepository roomateRepository = new RoomateRepository(CONNECTION_STRING);

            bool runProgram = true;
            while (runProgram)
            {
                string selection = GetMenuSelection();

                switch (selection)
                {
                    case ("Show all rooms"):
                        List<Room> rooms = roomRepo.GetAll();
                        foreach (Room r in rooms)
                        {
                            Console.WriteLine($"{r.Name} has an Id of {r.Id} and a max occupancy of {r.MaxOccupancy}");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for room"):
                        // Do stuff
                        Console.Write("Room Id: ");
                        int id = int.Parse(Console.ReadLine());

                        Room room = roomRepo.GetById(id);

                        Console.WriteLine($"{room.Id} - {room.Name} Max Occupancy({room.MaxOccupancy})");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Add a room"):
                        // Do stuff
                        Console.Write("Room name: ");
                        string name = Console.ReadLine();

                        Console.Write("Max occupancy: ");
                        int max = int.Parse(Console.ReadLine());

                        Room roomToAdd = new Room()
                        {
                            Name = name,
                            MaxOccupancy = max
                        };

                        roomRepo.Insert(roomToAdd);

                        Console.WriteLine($"{roomToAdd.Name} has been added and assigned an Id of {roomToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Show all chores"):
                        List<Chore> chores = choreRepo.GetAll();
                        foreach (Chore chore in chores)
                        {
                            Console.WriteLine($"{chore.Id} - {chore.Name}");
                        }
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for a chore"):
                        Console.Write("Enter chore id: ");
                        int choreId = int.Parse(Console.ReadLine());
                        Chore searchedChore = choreRepo.GetById(choreId);
                        if (searchedChore != null)
                        {
                            Console.WriteLine($"{searchedChore.Id} - {searchedChore.Name}");
                        }
                        else
                        {
                            Console.WriteLine("No such chore");
                        }
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Show all unassigned chores"):
                        List<Chore> unassignedChores = choreRepo.GetUnassignedChores();
                        if (unassignedChores.Count == 0)
                        {
                            Console.WriteLine("All chores are assigned");
                        }
                        else
                        {
                            foreach(Chore chore in unassignedChores)
                            {
                                Console.WriteLine($"\"{chore.Name}\" is unassigned");
                            }
                        }
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Assign chore to roommate"):
                        List<Chore> allChores = choreRepo.GetAll();
                        List<Roomate> allRoomates = roomateRepository.GetAll();
                        foreach(Chore chore in allChores)
                        {
                            Console.WriteLine($"{chore.Id} - \"{chore.Name}\"");
                        }
                        Console.Write("Enter a chore id: ");
                        int enteredChoreId = int.Parse(Console.ReadLine());
                        foreach(Roomate roomate in allRoomates)
                        {
                            Console.WriteLine($"{roomate.Id} - {roomate.FirstName} {roomate.LastName}");
                        }
                        Console.Write("Enter a roommate id: ");
                        int enteredRoommateId = int.Parse(Console.ReadLine());

                        int? responseInt = choreRepo.AssignChore(enteredRoommateId, enteredChoreId);
                        if (responseInt != null)
                        {
                            Console.WriteLine($"Successfully added RoomateChore with id of {responseInt}");
                        }
                        else
                        {
                            Console.WriteLine("There was an error assigning the chore.");
                        }
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Add a chore"):
                        Chore choreToAdd = new Chore();
                        Console.Write("Enter chore name: ");
                        choreToAdd.Name = Console.ReadLine();
                        choreRepo.Insert(choreToAdd);
                        Console.WriteLine($"{choreToAdd.Name} has been added and assigned an id of {choreToAdd.Id}");
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for a roommate"):
                        Console.Write("Enter roommate id: ");
                        int rId = int.Parse(Console.ReadLine());
                        Roomate searchedRoomate = roomateRepository.GetById(rId);
                        if (searchedRoomate != null)
                        {
                            Console.WriteLine($"{searchedRoomate.FirstName} - Rent Portion: {searchedRoomate.RentPortion} - Room: {searchedRoomate.Room.Name}");

                        }
                        else
                        {
                            Console.WriteLine("No such roommate");
                        }
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Exit"):
                        runProgram = false;
                        break;
                }
            }

        }

        static string GetMenuSelection()
        {
            Console.Clear();

            List<string> options = new List<string>()
            {
                "Show all rooms",
                "Search for room",
                "Add a room",
                "Show all chores",
                "Search for a chore",
                "Show all unassigned chores",
                "Assign chore to roommate",
                "Add a chore",
                "Search for a roommate",
                "Exit"
            };

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Select an option > ");

                    string input = Console.ReadLine();
                    int index = int.Parse(input) - 1;
                    return options[index];
                }
                catch (Exception)
                {

                    continue;
                }
            }
        }
    }
}
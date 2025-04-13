using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Models;

namespace MurderMystery
{
    class GameController
    {
        private GameState _state;

        public GameController(GameState state)
        {
            _state = state;
        }

        public void StartGame()
        {
            // Introduction
            Console.Clear();
            Console.WriteLine("==================================");
            Console.WriteLine("🔍 MURDER AT THE MANOR 🔍");
            Console.WriteLine("==================================\n");

            Console.WriteLine($"Tragedy has struck! {_state.CurrentMystery.Victim.Name} has been found dead.");
            Console.WriteLine("As the detective, it's your job to solve this murder case.");
            Console.WriteLine("Find clues, interview suspects, and identify the killer.\n");

            Console.WriteLine("Press any key to begin your investigation...");
            Console.ReadKey(true);

            // Main game loop
            GameLoop();

            // Game ending
            DisplayEnding();
        }

        private void GameLoop()
        {
            while (!_state.GameOver)
            {
                // Display current room and options
                DisplayCurrentState();

                // Get player command
                string command = GetPlayerCommand();

                // Process command
                ProcessCommand(command);

                if (!_state.GameOver)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                }
            }
        }

        private void DisplayCurrentState()
        {
            Console.Clear();

            var room = _state.Mansion[_state.CurrentRoom];
            Console.WriteLine($"\nLocation: {room.Name}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(room.Description);
            Console.WriteLine("----------------------------------------");

            // Show exits
            Console.WriteLine("\nExits:");
            foreach (var connection in room.Connections)
            {
                Console.WriteLine($"- {connection.Key}: {connection.Value}");
            }

            // Show people in room
            var peopleInRoom = _state.GetPeopleInCurrentRoom();
            if (peopleInRoom.Any())
            {
                Console.WriteLine("\nPeople present:");
                foreach (var person in peopleInRoom)
                {
                    string interviewedMarker = _state.InterviewedPeople.Contains(person.Name) ? " (Interviewed)" : "";
                    Console.WriteLine($"- {person.Name}{interviewedMarker}");
                }
            }
            else
            {
                Console.WriteLine("\nThere's no one else here.");
            }

            // Show visible clues
            var visibleClues = _state.GetVisibleCluesInCurrentRoom();
            if (visibleClues.Any())
            {
                Console.WriteLine("\nYou notice:");
                foreach (var clue in visibleClues)
                {
                    Console.WriteLine($"- {clue.Description}");
                }
            }
        }

        private string GetPlayerCommand()
        {
            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("1. Move to another room");
            Console.WriteLine("2. Talk to someone");
            Console.WriteLine("3. Search the room");
            Console.WriteLine("6. Quit game");
            Console.Write("\nEnter your choice (1-6): ");

            return Console.ReadLine();
        }

        private void ProcessCommand(string command)
        {
            switch (command)
            {
                case "1":
                    MoveToAnotherRoom();
                    break;
                case "2":
                    TalkToSomeone();
                    break;
                case "3":
                    SearchRoom();
                    break;
                case "6":
                    Console.WriteLine("\nAre you sure you want to quit? (y/n)");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        _state.GameOver = true;
                    }
                    break;
                default:
                    Console.WriteLine("\nInvalid command. Please try again.");
                    break;
            }
        }

        private void MoveToAnotherRoom()
        {
            var room = _state.Mansion[_state.CurrentRoom];
            var connections = room.Connections;

            Console.WriteLine("\nAvailable rooms:");
            var directions = connections.Keys.ToList();

            for (int i = 0; i < directions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {directions[i]} to {connections[directions[i]]}");
            }

            Console.Write("\nEnter room number (or 0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int roomIndex))
            {
                if (roomIndex == 0)
                {
                    Console.WriteLine("\nMovement cancelled.");
                    return;
                }

                if (roomIndex >= 1 && roomIndex <= directions.Count)
                {
                    Direction selectedDirection = directions[roomIndex - 1];
                    string newRoom = connections[selectedDirection];
                    _state.MoveToRoom(newRoom);
                    Console.WriteLine($"\nYou move to the {newRoom}.");
                }
                else
                {
                    Console.WriteLine("\nInvalid room selection.");
                }
            }
            else
            {
                Console.WriteLine("\nInvalid input.");
            }
        }

        private void TalkToSomeone()
        {
            var peopleInRoom = _state.GetPeopleInCurrentRoom();

            if (!peopleInRoom.Any())
            {
                Console.WriteLine("\nThere's no one here to talk to.");
                return;
            }

            Console.WriteLine("\nWho would you like to talk to?");
            for (int i = 0; i < peopleInRoom.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {peopleInRoom[i].Name}");
            }

            Console.Write("\nEnter person number (or 0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int personIndex))
            {
                if (personIndex == 0)
                {
                    Console.WriteLine("\nConversation cancelled.");
                    return;
                }

                if (personIndex >= 1 && personIndex <= peopleInRoom.Count)
                {
                    var selectedPerson = peopleInRoom[personIndex - 1];
                    _state.InterviewPerson(selectedPerson);

                    // Generate a statement based on what this person knows
                    string statement = GenerateStatement(selectedPerson);
                    Console.WriteLine($"\n{selectedPerson.Name} says: \"{statement}\"");

                    // If this is the murderer, they might seem suspicious
                    if (selectedPerson == _state.CurrentMystery.Murderer)
                    {
                        Console.WriteLine("\nYou notice they seem nervous while speaking.");
                    }
                }
                else
                {
                    Console.WriteLine("\nInvalid person selection.");
                }
            }
            else
            {
                Console.WriteLine("\nInvalid input.");
            }
        }

        private string GenerateStatement(Person speaker)
        {
            return speaker.GenerateStatement();
        }

        private void SearchRoom()
        {
            Console.WriteLine("\nYou carefully search the room...");

            var visibleClues = _state.GetVisibleCluesInCurrentRoom();

            if (visibleClues.Any())
            {
                Console.WriteLine("\nYou find:");
                foreach (var clue in visibleClues)
                {
                    Console.WriteLine($"- {clue.Description}");
                    _state.DiscoverClue(clue);
                }
            }
            else
            {
                Console.WriteLine("\nYou don't find anything of interest.");
            }
        }


        private void DisplayEnding()
        {
            Console.Clear();
                Console.WriteLine("🏆 CASE SOLVED! 🏆");
                Console.WriteLine("=================");
                Console.WriteLine("\nCongratulations, detective! You've cracked the case!");
                Console.WriteLine($"\nYou correctly identified that {_state.CurrentMystery.Murderer.Name} killed {_state.CurrentMystery.Victim.Name}");
                Console.WriteLine($"using the {_state.CurrentMystery.Weapon} in the {_state.CurrentMystery.Room}");
                Console.WriteLine($"with a motive of {_state.CurrentMystery.Motive}.");

            // Game statistics
            Console.WriteLine($"\nTurns taken: {_state.TurnCount}");
            Console.WriteLine($"Clues discovered: {_state.DiscoveredClues.Count}/{_state.CurrentMystery.Clues.Count}");
            Console.WriteLine($"People interviewed: {_state.InterviewedPeople.Count}/{_state.CurrentMystery.People.Count - 1}");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey(true);
        }
    }
}

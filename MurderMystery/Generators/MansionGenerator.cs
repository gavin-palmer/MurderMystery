using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Models;

namespace MurderMystery.Generators
{
    public class MansionGenerator
    {
        private readonly new Dictionary<string, Room> _mansion = new Dictionary<string, Room>();
        public Dictionary<string, Room> GenerateMansionLayout(Mystery mystery)
        {
            var nonePlayerRooms = DataProviderFactory.Rooms.LoadNonePlayerRooms();
            foreach (var room in nonePlayerRooms)
            {
                room.IsSpecialRoom = true;
                _mansion.Add(room.Name, room);
            }
            var rooms = mystery.Timeline.Select(x => x.Location).Distinct().ToList();
            var randomRooms = DataProviderFactory.Rooms.GetAll()
                .Where(x => rooms.Contains(x.Name))
                .Take(10)
                .Select(r => new Room(r.Name, r.Description))
                .ToList();

            foreach (var room in randomRooms)
            {
                _mansion.Add(room.Name, room);
            }

            CreateConnections();

            return _mansion;
        }

        private void CreateConnections()
        {
            var roomList = _mansion.Values.ToList();
            var foyer = _mansion["Foyer"];
            var securityRoom = _mansion["Security Room"];

            roomList.Remove(foyer);         // We'll insert it manually
            roomList.Remove(securityRoom);  // Still random

            var rng = new Random();
            roomList = roomList.OrderBy(_ => rng.Next()).ToList();

            // Add Security Room back into a random spot
            int secRoomIndex = rng.Next(roomList.Count + 1);
            roomList.Insert(secRoomIndex, securityRoom);

            int totalRooms = roomList.Count + 1; // +1 for Foyer
            int cols = 4;
            int rows = (int)Math.Ceiling((double)totalRooms / cols);

            var roomGrid = new Room[rows, cols];

            // Choose a better spot for the Foyer (somewhere near center)
            int foyerRow = Math.Min(1, rows - 1);
            int foyerCol = Math.Min(1, cols - 1);
            roomGrid[foyerRow, foyerCol] = foyer;

            // Place the rest of the rooms into the grid, skipping foyer slot
            int listIndex = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (roomGrid[row, col] != null) continue; // already filled by foyer

                    if (listIndex < roomList.Count)
                    {
                        roomGrid[row, col] = roomList[listIndex++];
                    }
                }
            }

            // Build connections
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var currentRoom = roomGrid[row, col];
                    if (currentRoom == null) continue;

                    if (row > 0 && roomGrid[row - 1, col] != null)
                        currentRoom.Connections[Direction.North] = roomGrid[row - 1, col].Name;

                    if (row < rows - 1 && roomGrid[row + 1, col] != null)
                        currentRoom.Connections[Direction.South] = roomGrid[row + 1, col].Name;

                    if (col > 0 && roomGrid[row, col - 1] != null)
                        currentRoom.Connections[Direction.West] = roomGrid[row, col - 1].Name;

                    if (col < cols - 1 && roomGrid[row, col + 1] != null)
                        currentRoom.Connections[Direction.East] = roomGrid[row, col + 1].Name;
                }
            }

            // Log Foyer & Security Room positions
            Console.WriteLine($"Foyer placed at: ({foyerRow}, {foyerCol})");
            var secIndex = roomList.IndexOf(securityRoom);
            var secRow = (secIndex + (secIndex >= (foyerRow * cols + foyerCol) ? 1 : 0)) / cols;
            var secCol = (secIndex + (secIndex >= (foyerRow * cols + foyerCol) ? 1 : 0)) % cols;
            Console.WriteLine($"Security Room is hidden at ({secRow}, {secCol})");
        }


        public void ExploreRoom(string roomName)
        {
            var room = _mansion[roomName];
            Console.WriteLine($"\n{room.Name}");
            Console.WriteLine(room.Description);

            Console.WriteLine("\nExits:");
            foreach (var connection in room.Connections)
            {
                Console.WriteLine($"- {connection.Key}: {connection.Value}");
            }

            Console.WriteLine("\nWhich direction would you like to go?");
        }

        public void DistributeClues(List<Clue> clues)
        {
            foreach (var clue in clues)
            {
                // If the clue's location exists in the mansion, place it there
                if (clue.Location != null && _mansion.ContainsKey(clue.Location))
                {
                    _mansion[clue.Location].Clues.Add(clue);
                }
                else
                {
                    // Otherwise, place it in a random room
                    var randomRoom = GetRandomRoom();
                    clue.Location = randomRoom;
                    _mansion[randomRoom].Clues.Add(clue);
                }
            }
        }

        public void PlacePeople(List<Person> people, List<TimelineEvent> timeline)
        {
            var victim = people.FirstOrDefault(x => x.IsVictim);
            // Get the last events of the timeline to determine final positions
            var lastTimeSlot = timeline.Select(e => e.Time).Max();
            var lastEvents = timeline.Where(e => e.Time == lastTimeSlot).ToList();

            foreach (var person in people)
            {
                // Skip placing the victim
                if (person == victim) continue;

                // Find where this person ended up
                var finalEvent = lastEvents.FirstOrDefault(e => e.Person == person);
                string location = finalEvent?.Location ?? GetRandomRoom();

                // Make sure the location exists in our mansion
                if (!_mansion.ContainsKey(location))
                {
                    location = GetRandomRoom();
                }

                // Add them to that room's list of people
                person.CurrentRoom = location;
            }
        }
        private string GetRandomRoom()
        {
            var randomRoom = RandomHelper.PickRandom<Room>(
                _mansion.Values.Where(r => r.Name != "Foyer").ToList()
            );
            return randomRoom.Name;

        }

    }
}

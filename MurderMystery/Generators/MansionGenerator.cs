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
        public Dictionary<string, Room> GenerateMansionLayout()
        {
            var nonePlayerRooms = DataProviderFactory.Rooms.LoadNonePlayerRooms();
            foreach (var room in nonePlayerRooms)
            {
                room.IsSpecialRoom = true;
                _mansion.Add(room.Name, room);
            }
            // Add 10 random rooms
            var randomRooms = DataProviderFactory.Rooms.GetAll()
                .OrderBy(r => Guid.NewGuid())
                .Take(10)
                .Select(r => new Room(r.Name, r.Description))
                .ToList();

            foreach (var room in randomRooms)
            {
                _mansion.Add(room.Name, room);
            }

            // Generate a coherent layout
            CreateConnections();

            return _mansion;
        }

        private void CreateConnections()
        {
            var roomList = _mansion.Values.ToList();

            var entranceHall = _mansion["Foyer"];
            var securityRoom = _mansion["Security Room"];

            entranceHall.Connections[Direction.North] = roomList[0].Name;
            entranceHall.Connections[Direction.East] = roomList[1].Name;
            entranceHall.Connections[Direction.West] = roomList[2].Name;

            securityRoom.Connections[Direction.South] = roomList[3].Name;


            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].IsSpecialRoom) continue;

                if (i >= 4)
                {
                    roomList[i].Connections[Direction.North] = roomList[i - 4].Name;
                    roomList[i - 4].Connections[Direction.South] = roomList[i].Name;
                }

                if (i % 4 != 3 && i < roomList.Count - 1)
                {
                    roomList[i].Connections[Direction.East] = roomList[i + 1].Name;
                    roomList[i + 1].Connections[Direction.West] = roomList[i].Name;
                }
            }
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
                if (_mansion.ContainsKey(clue.Location))
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

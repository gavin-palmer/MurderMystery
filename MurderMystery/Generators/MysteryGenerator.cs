using System;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Helpers.MurderMystery.Utilities;
using MurderMystery.Models;

namespace MurderMystery.Generators
{
    public static class MysteryGenerator
    {
        private static Random _random = new Random();
        public static TimelineContext context;
        public static Mystery CreateMystery()
        {
            var timeSlots = TimeFrames.GetTimeFrames("6:00pm", "9:00pm", 15);

            var people = PeopleGenerator.GeneratePeople();

            var victim = RandomHelper.PickRandom(people);
            var murderer = RandomHelper.PickRandom(people.Where(p => p != victim).ToList());

            var weaponData = DataProviderFactory.Weapons.GetRandom();

            int roomCount = _random.Next(9, 14);

            var roomData = DataProviderFactory.Rooms.GetRandom();
            var motiveData = DataProviderFactory.Motives.GetRandom();

            string weapon = weaponData.Name;
            string room = roomData.Name;
            string motive = motiveData.Name;

            var context = new TimelineContext(people, victim, murderer, weapon, room, motive, timeSlots);

            context.GenerateTimeline();

            var mystery = new Mystery
            {
                Victim = victim,
                Murderer = murderer,
                Weapon = weapon,
                Room = room,
                Motive = motive,
                People = people,
                Timeline = context.Events,
                Clues = context.Clues,
                Rooms = context.Rooms
            };
            SecretGenerator.AssignSecrets(mystery);
            Console.ReadLine();
            return mystery;

        }

        public static void PrintMysteryDetails(TimelineContext context)
        {
            Console.WriteLine("=== MURDER MYSTERY DETAILS ===\n");

            // Print core mystery elements
            Console.WriteLine("=== CORE MYSTERY ELEMENTS ===");
            Console.WriteLine($"Victim: {context.Victim.Name}");
            Console.WriteLine($"Murderer: {context.Murderer.Name}");
            Console.WriteLine($"Weapon: {context.Weapon}");
            Console.WriteLine($"Murder Room: {context.Room}");
            Console.WriteLine($"Motive: {context.Motive}");
            Console.WriteLine();

            // Print people
            Console.WriteLine("=== PEOPLE ===");
            foreach (var person in context.People)
            {
                Console.WriteLine($"- {person.Name}");
            }
            Console.WriteLine();

            // Print security information
            Console.WriteLine("=== SECURITY SYSTEM ===");
            Console.WriteLine($"Security Room: {context.SecurityRoom}");
            Console.WriteLine($"Security PIN: {context.SecurityPinCode}");
            Console.WriteLine("Rooms with Cameras:");
            foreach (var cameraRoom in context.RoomsWithCameras)
            {
                Console.WriteLine($"- {cameraRoom}");
            }
            Console.WriteLine();

            // Print timeline (where everyone is at each time)
            Console.WriteLine("=== TIMELINE ===");
            foreach (var timeSlot in context.TimeSlots)
            {
                Console.WriteLine($"\nTime: {timeSlot}");
                Console.WriteLine("People Locations:");
                foreach (var person in context.People)
                {
                    if (context.LocationsByTime.ContainsKey(timeSlot) &&
                        context.LocationsByTime[timeSlot].ContainsKey(person))
                    {
                        Console.WriteLine($"- {person.Name}: {context.LocationsByTime[timeSlot][person]}");
                    }
                }

                // Print events for this time
                var timeEvents = context.Events.Where(e => e.Time == timeSlot).ToList();
                if (timeEvents.Any())
                {
                    Console.WriteLine("Events:");
                    foreach (var evt in timeEvents)
                    {
                        string secretOrLie = "";
                        if (evt.IsSecret) secretOrLie += " [SECRET]";
                        if (evt.IsLie) secretOrLie += " [LIE]";

                        Console.WriteLine($"- {evt.Person.Name} in {evt.Location}: {evt.Action}{secretOrLie}");
                    }
                }
            }
            Console.WriteLine();

            // Print relationships
            Console.WriteLine("=== RELATIONSHIPS ===");
            foreach (var relationship in context.Relationships)
            {
                if (relationship.Value != RelationshipType.Unknown)
                {
                    Console.WriteLine($"{relationship.Key.Item1.Name} → {relationship.Key.Item2.Name}: {relationship.Value}");
                }
            }
            Console.WriteLine();

            // Print all clues
            Console.WriteLine("=== CLUES ===");
            foreach (var clue in context.Clues)
            {
                Console.WriteLine($"- [{clue.Type}] {clue.Description} (Location: {clue.Location})");
            }
            Console.WriteLine();

            // Print the murder sequence
            Console.WriteLine("=== MURDER SEQUENCE ===");
            var murderEvent = context.Events.FirstOrDefault(e =>
                e.Person == context.Murderer &&
                e.Location == context.Room &&
                e.Action.Contains("killed") &&
                !e.IsLie);

            if (murderEvent != null)
            {
                Console.WriteLine($"Time: {murderEvent.Time}");
                Console.WriteLine($"Location: {murderEvent.Location}");
                Console.WriteLine($"Action: {context.Murderer.Name} {murderEvent.Action}");

                // Print murderer's alibi
                var alibiEvent = context.Events.FirstOrDefault(e =>
                    e.Person == context.Murderer &&
                    e.Time == murderEvent.Time &&
                    e.IsLie);

                if (alibiEvent != null)
                {
                    Console.WriteLine($"False Alibi: {context.Murderer.Name} claims to have been in {alibiEvent.Location}, {alibiEvent.Action}");
                }
            }

            Console.WriteLine("\n=== END OF MYSTERY DETAILS ===");
        }
    }
}
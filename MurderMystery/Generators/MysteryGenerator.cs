using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Helpers.MurderMystery.Utilities;
using MurderMystery.Models;
using MurderMystery.MurderMystery.Generators;

namespace MurderMystery.Generators
{
    public static class MysteryGenerator
    {
        private static Random _random = new Random();
        public static TimelineContext context;
        public static void GenerateMystery()
        {
            var timeSlots = TimeFrames.GetTimeFrames("6:00pm", "9:00pm", 15);

            int characterCount = _random.Next(5, 9);
            var names = DataProviderFactory.Names.GetRandomSelection(characterCount);

            var people = names.Select(n => new Person { Name = n }).ToList();

            var victim = RandomHelper.PickRandom(people);
            victim.IsVictim = true;
            var murderer = RandomHelper.PickRandom(people.Where(p => p != victim).ToList());

            var weaponData = DataProviderFactory.Weapons.GetRandom();
            var roomData = DataProviderFactory.Rooms.GetRandom();
            var motiveData = DataProviderFactory.Motives.GetRandom();

            string weapon = weaponData.Name;
            string room = roomData.Name;
            string motive = motiveData.Name;

            context = new TimelineContext(people, victim, murderer, weapon, room, motive, timeSlots);

            context.GenerateTimeline();


        }

        /// <summary>
        /// Generates a full mystery object
        /// </summary>
        public static Mystery CreateMystery()
        {
            // Get time slots
            var timeSlots = TimeFrames.GetTimeFrames("6:00pm", "9:00pm", 15);

            // Generate between 5-8 characters
            int characterCount = _random.Next(5, 9);
            var names = DataProviderFactory.Names.GetRandomSelection(characterCount);

            // Create people
            var people = names.Select(n => new Person { Name = n }).ToList();

            // Randomly select victim, murderer, weapon, room, and motive
            var victim = RandomHelper.PickRandom(people);
            var murderer = RandomHelper.PickRandom(people.Where(p => p != victim).ToList());

            var weaponData = DataProviderFactory.Weapons.GetRandom();
            var roomData = DataProviderFactory.Rooms.GetRandom();
            var motiveData = DataProviderFactory.Motives.GetRandom();

            string weapon = weaponData.Name;
            string room = roomData.Name;
            string motive = motiveData.Name;

            // Create your context
            var context = new TimelineContext(people, victim, murderer, weapon, room, motive, timeSlots);

            // Generate the timeline
            context.GenerateTimeline();

            // Create and return the mystery object
            return new Mystery
            {
                Victim = victim,
                Murderer = murderer,
                Weapon = weapon,
                Room = room,
                Motive = motive,
                People = people,
                Timeline = context.Events,
                Clues = context.Clues
            };
        }
    }
}
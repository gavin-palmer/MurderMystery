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
        public static Mystery CreateMystery()
        {
            var timeSlots = TimeFrames.GetTimeFrames("6:00pm", "9:00pm", 15);

            var people = PeopleGenerator.GeneratePeople();

            var victim = RandomHelper.PickRandom(people);
            var murderer = RandomHelper.PickRandom(people.Where(p => p != victim).ToList());

            var weaponData = DataProviderFactory.Weapons.GetRandom();
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
                Clues = context.Clues
            };
            SecretGenerator.AssignSecrets(mystery);
            return mystery;

        }
    }
}
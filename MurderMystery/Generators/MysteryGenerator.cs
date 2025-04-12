using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Models;

namespace MurderMystery.Generators
{

    public static class MysteryGenerator
    {

        public static Mystery GenerateMystery()
        {
            var names = Names.Get();
            var weapons = Weapons.Get();
            var rooms = Rooms.Get();
            var motives = Motives.Get();

            var people = names.Select(n => new Person { Name = n }).ToList();
            var victim = RandomHelper.PickRandom(people);
            var murderer = RandomHelper.PickRandom(people.Where(p => p != victim).ToList());
            var weapon = RandomHelper.PickRandom(weapons.ToList());
            var room = RandomHelper.PickRandom(rooms.ToList());
            var motive = RandomHelper.PickRandom(motives.ToList());

            var timeline = TimelineGenerator.GenerateTimeline(people, victim, murderer, room, weapon);

            return new Mystery
            {
                Victim = victim,
                Murderer = murderer,
                Weapon = weapon,
                Room = room,
                Motive = motive,
                People = people,
                Timeline = timeline
            };
        }

    }
}

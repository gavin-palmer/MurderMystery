using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Interfaces;
using MurderMystery.Models;

namespace MurderMystery.Storylines
{
    public class TheftStoryline : IStorylineStrategy
    {
        public string Name => "Theft";

        public bool Execute(TimelineContext context)
        {
            var availablePeople = context.People
                .Where(p => p != context.Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                .ToList();

            if (!availablePeople.Any()) return false;

            var thief = RandomHelper.PickRandom(availablePeople);

            var availableTimes = context.TimeSlots
                .Where(t => t != context.MurderTime && t != context.PreMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return false;

            string theftTime = RandomHelper.PickRandom(availableTimes);

            var stolenItems = new List<string>
            {
                "valuable jewelry",
                "important document",
                "sentimental keepsake",
                "antique heirloom",
                "cash",
                "digital device",
                "rare collectible",
                "access card",
                "prescription medication",
                "personal diary"
            };

            string stolenItem = RandomHelper.PickRandom(stolenItems);

            var theftRoom = context.LocationsByTime[theftTime][thief];

            var theftEvent = new TimelineEvent()
            {
                Time = theftTime,
                Person = thief,
                Location = theftRoom,
                Action = $"stole {stolenItem} from {theftRoom}",
                IsSecret = true,
                IsLie = false
            };
            context.Events.Add(theftEvent);
            thief.TimelineEvents.Add(theftEvent);

            var falseAlibiRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll()
                    .Where(r => r.Name != theftRoom)
                    .Select(r => r.Name)
                    .ToList());

            var falseAlibiEvent = new TimelineEvent()
            {
                Time = theftTime,
                Person = thief,
                Location = falseAlibiRoom,
                Action = "claims to have been alone",
                IsSecret = false,
                IsLie = true
            };
            context.Events.Add(falseAlibiEvent);
            thief.TimelineEvents.Add(falseAlibiEvent);

            context.Clues.Add(new Clue(
                $"{stolenItem} was reported missing from {theftRoom}",
                ClueType.Testimony
            )
            { Location = theftRoom });
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MurderMystery.Interfaces;
using MurderMystery.Models;

namespace MurderMystery.Storylines
{
    public class DrunkennessStoryline : IStorylineStrategy
    {
        public string Name => "Drunkenness";

        public bool Execute(TimelineContext context)
        {
            var availablePeople = context.People
                 .Where(p => p != context.Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                 .ToList();

            if (!availablePeople.Any()) return false;

            var drunkPerson = RandomHelper.PickRandom(availablePeople);

            var availableTimes = context.TimeSlots
                .Where(t => t != context.MurderTime && t != context.PreMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return false;

            string drunkTime = RandomHelper.PickRandom(availableTimes);

            var drunkenActions = new List<string>
            {
                "got very drunk and started singing loudly",
                "drank too much and fell asleep on the floor",
                "became intoxicated and told inappropriate stories",
                "had one too many drinks and tried to dance on the table",
                "got drunk and made a scene",
                "became inebriated and confessed their crush on someone",
                "drank heavily and accidentally broke something valuable",
                "got tipsy and revealed embarrassing secrets about themselves",
                "had too much to drink and challenged people to arm wrestling",
                "became drunk and attempted to recite poetry"
            };

            string drunkenAction = RandomHelper.PickRandom(drunkenActions);

            var drunkRoom = context.LocationsByTime[drunkTime][drunkPerson];

            var drunkEvent = new TimelineEvent()
            {
                Time = drunkTime,
                Person = drunkPerson,
                Location = drunkRoom,
                Action = drunkenAction,
                IsSecret = false,
                IsLie = false
            };
            context.Events.Add(drunkEvent);
            drunkPerson.TimelineEvents.Add(drunkEvent);

            var potentialWitnesses = context.People
                .Where(p => p != drunkPerson && p != context.Victim)
                .ToList();

            if (!potentialWitnesses.Any()) return false;

            var witness = RandomHelper.PickRandom(potentialWitnesses);
            context.LocationsByTime[drunkTime][witness] = drunkRoom;

            var witnessEvent = new TimelineEvent()
            {
                Time = drunkTime,
                Person = witness,
                Location = drunkRoom,
                Action = $"witnessed {drunkPerson.Name}'s drunken behavior",
                IsSecret = false,
                IsLie = false
            };
            context.Events.Add(witnessEvent);
            witness.TimelineEvents.Add(witnessEvent);

            if (availableTimes.Count > 1)
            {
                availableTimes.Remove(drunkTime);
                string memoryGapTime = RandomHelper.PickRandom(availableTimes);

                var memoryGapEvent = new TimelineEvent()
                {
                    Time = memoryGapTime,
                    Person = drunkPerson,
                    Location = context.LocationsByTime[memoryGapTime][drunkPerson],
                    Action = "can't remember what happened due to intoxication",
                    IsSecret = false,
                    IsLie = false
                };
                context.Events.Add(memoryGapEvent);
                drunkPerson.TimelineEvents.Add(memoryGapEvent);
            }
            return true;
        }
    }
}

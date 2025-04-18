using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MurderMystery.Enums;
using MurderMystery.Interfaces;
using MurderMystery.Models;

namespace MurderMystery.Storylines
{
    public class SecretDiscoveryStoryline : IStorylineStrategy
    {
        public string Name => "Secret Discovery";

        public bool Execute(TimelineContext context)
        {
            var availablePeople = context.People
                .Where(p => p != context.Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                .ToList();

            if (availablePeople.Count < 2) return false;

            var secretHolder = RandomHelper.PickRandom(availablePeople);
            availablePeople.Remove(secretHolder);
            var discoverer = RandomHelper.PickRandom(availablePeople);

            var availableTimes = context.TimeSlots
                .Where(t => t != context.MurderTime && t != context.PreMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return false;

            string discoveryTime = RandomHelper.PickRandom(availableTimes);

            var secrets = new List<string>
            {
                "hidden financial problems",
                "secret identity",
                "past criminal record",
                "embarrassing personal history",
                "medical condition",
                "family secret",
                "professional misconduct",
                "secret addiction",
                "hidden inheritance",
                "double life"
            };

            string secretType = RandomHelper.PickRandom(secrets);

            var discoveryEvent = new TimelineEvent()
            {
                Time = discoveryTime,
                Person = discoverer,
                Location = context.LocationsByTime[discoveryTime][discoverer],
                Action = $"discovered {secretHolder.Name}'s {secretType}",
                IsSecret = false,
                IsLie = false
            };
            context.Events.Add(discoveryEvent);
            discoverer.TimelineEvents.Add(discoveryEvent);

            context.Clues.Add(new Clue(
                $"Evidence of {secretHolder.Name}'s {secretType} was found",
                ClueType.Testimony
            )
            { Location = context.LocationsByTime[discoveryTime][discoverer] });
            return true;
        }
    }
}

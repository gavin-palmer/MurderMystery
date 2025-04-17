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
    public class MysteriousPhoneCallStoryline : IStorylineStrategy
    {
        public string Name => "Mysterious Phonecall";

        public bool Execute(TimelineContext context)
        {
            var availablePeople = context.People
                .Where(p => p != context.Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                .ToList();

            if (!availablePeople.Any()) return false;

            var receiver = RandomHelper.PickRandom(availablePeople);
            
            var availableTimes = context.TimeSlots
                .Where(t => t != context.MurderTime && t != context.PreMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return false;

            string callTime = RandomHelper.PickRandom(availableTimes);

            var callContents = new List<string>
            {
                "a mysterious threat",
                "shocking news from an unknown caller",
                "a hushed and urgent conversation",
                "an emotional and tense discussion",
                "what sounded like blackmail",
                "an argument with someone they refused to name",
                "disturbing information from an anonymous source",
                "a tearful conversation",
                "an offer they seemed reluctant to accept",
                "news that clearly upset them"
            };

            string callContent = RandomHelper.PickRandom(callContents);

            var callRoom = context.LocationsByTime[callTime][receiver];

            var callEvent = new TimelineEvent()
            {
                Time = callTime,
                Person = receiver,
                Location = callRoom,
                Action = $"received a phone call about {callContent}",
                IsSecret = true,
                IsLie = false
            };
            context.Events.Add(callEvent);
            receiver.TimelineEvents.Add(callEvent);

            var publicEvent = new TimelineEvent()
            {
                Time = callTime,
                Person = receiver,
                Location = callRoom,
                Action = "took a phone call and seemed disturbed afterward",
                IsSecret = false,
                IsLie = false
            };
            context.Events.Add(publicEvent);
            receiver.TimelineEvents.Add(publicEvent);

            var potentialOverhearers = context.People
                .Where(p => p != receiver && p != context.Victim)
                .ToList();

            if (!potentialOverhearers.Any()) return false;

            var overhearer = RandomHelper.PickRandom(potentialOverhearers);
            context.LocationsByTime[callTime][overhearer] = callRoom;

            var overhearEvent = new TimelineEvent()
            {
                Time = callTime,
                Person = overhearer,
                Location = callRoom,
                Action = $"overheard part of {receiver.Name}'s phone conversation",
                IsSecret = false,
                IsLie = false
            };
            context.Events.Add(overhearEvent);
            overhearer.TimelineEvents.Add(overhearEvent);

            context.Clues.Add(new Clue(
                $"Phone records show {receiver.Name} received a call at {callTime}",
                ClueType.Testimony
            )
            { Location = "Study" });
            return true;
        }
    }
}

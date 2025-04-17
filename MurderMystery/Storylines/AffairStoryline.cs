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
    public class AffairStoryline : IStorylineStrategy
    {
        public string Name => "Affair";

        public bool Execute(TimelineContext context)
        {
            var availablePeople = context.People
                .Where(p => p != context.Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                .ToList();

            if (availablePeople.Count < 2) return false;

            var lover1 = RandomHelper.PickRandom(availablePeople);
            availablePeople.Remove(lover1);
            var lover2 = RandomHelper.PickRandom(availablePeople);

            var availableTimes = context.TimeSlots
                .Where(t => t != context.MurderTime && t != context.PreMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return false;

            string meetingTime = RandomHelper.PickRandom(availableTimes);

            string meetingRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());

            context.LocationsByTime[meetingTime][lover1] = meetingRoom;
            context.LocationsByTime[meetingTime][lover2] = meetingRoom;

            var affair1Event = new TimelineEvent()
            {
                Time = meetingTime,
                Person = lover1,
                Location = meetingRoom,
                Action = $"had a secret romantic rendezvous with {lover2.Name}",
                IsSecret = true,
                IsLie = false
            };
            context.Events.Add(affair1Event);
            lover1.TimelineEvents.Add(affair1Event);

            var affair2Event = new TimelineEvent()
            {
                Time = meetingTime,
                Person = lover2,
                Location = meetingRoom,
                Action = $"met secretly with {lover1.Name} for a romantic affair",
                IsSecret = true,
                IsLie = false
            };
            context.Events.Add(affair2Event);
            lover2.TimelineEvents.Add(affair2Event);

            context.Relationships[(lover1, lover2)] = RelationshipType.Lover;
            context.Relationships[(lover2, lover1)] = RelationshipType.Lover;

            var potentialGossips = context.People
                .Where(p => p != lover1 && p != lover2 && p != context.Victim)
                .ToList();

            if (!potentialGossips.Any()) return false;

            var gossiper = RandomHelper.PickRandom(potentialGossips);

            var gossipEvent = new TimelineEvent()
            {
                Time = RandomHelper.PickRandom(availableTimes),
                Person = gossiper,
                Location = RandomHelper.PickRandom(DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList()),
                Action = $"heard rumors about {lover1.Name} and {lover2.Name} having a secret relationship",
                IsSecret = false,
                IsLie = false
            };
            context.Events.Add(gossipEvent);
            gossiper.TimelineEvents.Add(gossipEvent);
            return true;
        }
    }

}

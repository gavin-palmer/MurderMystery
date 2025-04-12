using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Models;

namespace MurderMystery.Generators
{
    public static class TimelineGenerator
    {

        public static List<TimelineEvent> GenerateTimeline(List<Person> people, Person victim, Person murderer, string murderRoom, string weapon)
        {
            var timeline = new List<TimelineEvent>();
            foreach (var p in people)
            {
                if (p == murderer)
                {
                    timeline.Add(new TimelineEvent
                    {
                        Time = "7:15pm",
                        Person = p,
                        Location = murderRoom,
                        Action = $"Committed the murder with the {weapon}"
                    });
                }
                else if (p != victim)
                {
                    var location = RandomHelper.PickRandom(Rooms.Get());
                    timeline.Add(new TimelineEvent
                    {
                        Time = "7:15pm",
                        Person = p,
                        Location = location,
                        Action = "Was having a conversation"
                    });
                }
            }

            return timeline;
        }

        public static string CreateStatement(Person speaker, List<TimelineEvent> timeline)
        {
            var seen = RandomHelper.PickRandom(timeline.Where(e => e.Person != speaker).ToList());
            return $"I saw {seen.Person.Name} in the {seen.Location} around {seen.Time}.";
        }

    }
}

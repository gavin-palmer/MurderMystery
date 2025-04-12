using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Enums;
using MurderMystery.Generators;

namespace MurderMystery.Models
{
    public class Mystery
    {
        public Person Victim { get; set; }
        public Person Murderer { get; set; }
        public string Weapon { get; set; }
        public string Room { get; set; }
        public string Motive { get; set; }
        public List<Person> People { get; set; }
        public List<TimelineEvent> Timeline { get; set; }
        public List<Clue> Clues { get; set; }
        public Dictionary<(Person, Person), RelationshipType> Relationships { get; set; }

        public void PrintSummary(bool spoilers = true)
        {
            Console.WriteLine("🎭 Mystery Generated:");
            if (spoilers)
            {
                Console.WriteLine($"Victim: {Victim.Name}");
                Console.WriteLine($"Murderer: {Murderer.Name}");
                Console.WriteLine($"Weapon: {Weapon}");
                Console.WriteLine($"Room: {Room}");
                Console.WriteLine($"Motive: {Motive}");
            }
            Console.WriteLine($"Number of Characters: {People.Count}");
            Console.WriteLine($"Number of Clues: {Clues?.Count ?? 0}");
            Console.WriteLine($"Timeline Events: {Timeline?.Count ?? 0}");
            Console.WriteLine();
        }

        public void PrintTimeline(bool spoilers = true)
        {
            if (Timeline == null || !Timeline.Any())
            {
                Console.WriteLine("No timeline events available.");
                return;
            }

            Console.WriteLine("📅 Timeline of Events:");

            // Group events by time
            var timeGroups = Timeline
                .GroupBy(e => e.Time)
                .OrderBy(g => g.Key);

            foreach (var timeGroup in timeGroups)
            {
                Console.WriteLine($"\n=== {timeGroup.Key} ===");

                foreach (var evt in timeGroup)
                {
                    if (spoilers || !evt.IsSecret)
                    {
                        var lieMarker = spoilers && evt.IsLie ? " [LIE]" : "";
                        string secretMarker = spoilers && evt.IsSecret ? " [SECRET]" : "";
                        Console.WriteLine($"{evt.Person.Name} in {evt.Location}: {evt.Action}{secretMarker}{lieMarker}");
                    }
                    
                }
            }

            Console.WriteLine();
        }

        public void PrintClues(bool spoilers = true)
        {
            if (Clues == null || !Clues.Any())
            {
                Console.WriteLine("No clues available.");
                return;
            }

            Console.WriteLine("🔍 Clues:");

            // Group clues by type
            var clueGroups = Clues
                .GroupBy(c => c.Type);

            foreach (var clueGroup in clueGroups)
            {
                Console.WriteLine($"\n=== {clueGroup.Key} Clues ===");

                foreach (var clue in clueGroup)
                {
                    string redHerringMarker = spoilers && clue.IsRedHerring ? " [RED HERRING]" : "";
                    Console.WriteLine($"- {clue.Description} (Found in: {clue.Location}){redHerringMarker}");
                }
            }

            Console.WriteLine();
        }

        public void PrintRelationships()
        {
            if (Relationships == null || !Relationships.Any())
            {
                Console.WriteLine("No relationships available.");
                return;
            }

            Console.WriteLine("👥 Relationships:");

            foreach (var person in People)
            {
                Console.WriteLine($"\n{person.Name}'s Relationships:");

                bool hasRelationships = false;

                foreach (var otherPerson in People.Where(p => p != person))
                {
                    var key = (person, otherPerson);

                    if (Relationships.ContainsKey(key) && Relationships[key] != RelationshipType.Unknown)
                    {
                        Console.WriteLine($"- Feels {Relationships[key]} towards {otherPerson.Name}");
                        hasRelationships = true;
                    }
                }

                if (!hasRelationships)
                {
                    Console.WriteLine("- No significant relationships");
                }
            }

            Console.WriteLine();
        }

        public void PrintNPCStatements()
        {
            Console.WriteLine("🗣️ NPC Statements:");

            foreach (var p in People.Where(p => p != Victim))
            {
                // Find events this person witnessed
                var eventsWitnessed = Timeline
                    .Where(e => e.Person == p && !e.IsSecret)
                    .ToList();

                // Add events they might have seen (same room, same time)
                foreach (var ownEvent in Timeline.Where(e => e.Person == p && !e.IsSecret))
                {
                    eventsWitnessed.AddRange(Timeline.Where(e =>
                        e.Time == ownEvent.Time &&
                        e.Location == ownEvent.Location &&
                        e.Person != p &&
                        !e.IsSecret));
                }

                // Generate a statement
                string statement;

                if (eventsWitnessed.Any())
                {
                    var eventToShare = RandomHelper.PickRandom(eventsWitnessed);

                    if (eventToShare.Person == p)
                    {
                        statement = $"I was in the {eventToShare.Location} at {eventToShare.Time}. I {eventToShare.Action}.";
                    }
                    else
                    {
                        statement = $"I saw {eventToShare.Person.Name} in the {eventToShare.Location} at {eventToShare.Time}. They {eventToShare.Action}.";
                    }
                }
                else
                {
                    statement = $"I'm afraid I don't have much to share.";
                }

                Console.WriteLine($"{p.Name} says: \"{statement}\"");
            }
        }


        public void PrintAll(bool spoilers = true)
        {
            PrintSummary(spoilers);
            PrintTimeline(spoilers);
            PrintClues(spoilers);
            PrintRelationships();
            PrintNPCStatements();

        }
    }
}
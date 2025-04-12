using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Generators;

namespace MurderMystery.Models
{
    public class Mystery
    {
            public Person Victim;
            public Person Murderer;
            public string Weapon;
            public string Room;
            public string Motive;
            public List<Person> People;
            public List<TimelineEvent> Timeline;

            public void PrintSummary()
            {
                Console.WriteLine("🎭 Mystery Generated:");
                Console.WriteLine($"Victim: {Victim.Name}");
                Console.WriteLine($"Murderer: {Murderer.Name}");
                Console.WriteLine($"Weapon: {Weapon}");
                Console.WriteLine($"Room: {Room}");
                Console.WriteLine($"Motive: {Motive}");
                Console.WriteLine();
            }

            public void PrintNPCStatements()
            {
                Console.WriteLine("🗣️ NPC Statements:");
                foreach (var p in People.Where(p => p != Victim))
                {
                    var statement = TimelineGenerator.CreateStatement(p, Timeline);
                    Console.WriteLine($"{p.Name} says: \"{statement}\"");
                }
            }
    }
}

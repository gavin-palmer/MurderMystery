using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Enums;

namespace MurderMystery.Models
{
    public class Person
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string FunFact { get; set; }
        public List<Relationship> Relationships { get; set; }
        public bool IsVictim { get; set; }
        public string CurrentRoom { get; set; }
        public Footwear Footwear { get; set; }
        public Occupation Occupation { get; set; }
        public List<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
        public string GenerateStatement()
        {
            var statements = TimelineEvents.Where(x => x.IsLie || !x.IsSecret).Select(x => x.Description()).ToList();
            return string.Join(Environment.NewLine, statements);
        }
    }

}

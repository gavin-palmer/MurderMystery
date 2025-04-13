using System;
using MurderMystery.Enums;

namespace MurderMystery.Models
{
    public class Clue
    {
        public Clue() { }
        public Clue(string description, ClueType type, bool isRedHerring = false)
        {
            Description = description;
            Type = type;
            IsRedHerring = isRedHerring;
        }
        public string Description { get; set; }
        public ClueType Type { get; set; }
        public string RelatedTo { get; set; }
        public string Location { get; set; }
        public bool IsHidden { get; set; } = false;
        public bool IsFound { get; set; } = false; 
        public bool IsRedHerring { get; set; } = false; 
    }
}

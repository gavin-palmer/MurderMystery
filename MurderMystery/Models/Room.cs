using System;
using MurderMystery.Enums;
using System.Collections.Generic;
namespace MurderMystery.Models
{
    public class Room
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<Direction, string> Connections { get; set; } = new Dictionary<Direction, string>();
        public bool IsSpecialRoom { get; set; } = false;
        public List<Clue> Clues { get; set; } = new List<Clue>();
        public Room(string name, string description = null)
        {
            Name = name;
            Description = description ?? $"The {name.ToLower()} of the mansion";
        }
    }
}

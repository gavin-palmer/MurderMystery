using System;
namespace MurderMystery.Models
{
    public class Room
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Room(string name, string description = null)
        {
            Name = name;
            Description = description ?? $"The {name.ToLower()} of the mansion";
        }
    }
}

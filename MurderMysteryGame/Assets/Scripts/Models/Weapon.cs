using System;
namespace MurderMystery.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Weapon(string name, string description = null)
        {
            Name = name;
            Description = description ?? $"A {name.ToLower()} that could be used to commit murder";
        }
    }
}

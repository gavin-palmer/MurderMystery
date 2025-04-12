using System;
namespace MurderMystery.Models
{
    public class Motive
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Templates for clue text based on this motive
        public string[] Clues { get; set; }

        public Motive(string name, string description = null, string[] clues = null)
        {
            Name = name;
            Description = description ?? $"{name}";
            Clues = clues ?? new string[]
            {
                "Evidence suggests {0} might have had a {1} motive regarding {2}"
            };
        }
    }
}

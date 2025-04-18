using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Enums;
using MurderMystery.Models;

namespace MurderMystery.Data.Providers
{
    public class ClueProvider : BaseDataProvider<Clue>
    {
        protected override List<Clue> LoadItems()
        {
            return new List<Clue>
            {
                // Weapon clues
                new Clue("Evidence shows the {0} was used in the murder", ClueType.Weapon),
                new Clue("Traces of blood were found on a {0}", ClueType.Weapon),
                new Clue("Forensic analysis confirms the murder weapon was a {0}", ClueType.Weapon),
                new Clue("The {0} appears to have been recently cleaned", ClueType.Weapon),
                new Clue("An impression matching a {0} was found at the scene", ClueType.Weapon),
                
                // Location clues
                new Clue("Signs of a struggle indicate this is where {0} was killed", ClueType.Location),
                new Clue("Blood spatter analysis points to the murder occurring in the {0}", ClueType.Location),
                new Clue("The {0} was recently cleaned, but traces of evidence remain", ClueType.Location),
                new Clue("A witness heard a commotion coming from the {0}", ClueType.Location),
                new Clue("Items in the {0} appear to have been disturbed during a struggle", ClueType.Location),
                
                // Time of death clues
                new Clue("The medical examiner estimates death occurred around {0}", ClueType.TimeOfDeath),
                new Clue("Body temperature suggests {0} was killed at approximately {1}", ClueType.TimeOfDeath),
                new Clue("A broken watch on the victim stopped at {0}", ClueType.TimeOfDeath),
                new Clue("Analysis of stomach contents indicates death shortly after {0}", ClueType.TimeOfDeath),
                new Clue("The coroner's report places time of death at {0}", ClueType.TimeOfDeath),
                
                // Alibi clues
                new Clue("{0} confirms seeing {1} in the {2} at {3}", ClueType.Alibi),
                new Clue("{0} and {1} were together in the {2} during {3}", ClueType.Alibi),
                new Clue("Multiple witnesses confirm {0} was in the {1} at {2}", ClueType.Alibi),
                
                // Behavioral clues
                new Clue("{0} seemed unusually nervous when discussing {1}", ClueType.Behavioral),
                new Clue("{0} was seen washing their hands repeatedly", ClueType.Behavioral),
                new Clue("{0} refused to discuss their whereabouts at the time of the murder", ClueType.Behavioral),
                new Clue("{0} asked suspicious questions about police procedures", ClueType.Behavioral),
                
                // Physical evidence
                new Clue("A fingerprint matching {0} was found on {1}", ClueType.Physical),
                new Clue("A fiber from {0}'s clothing was found near the body", ClueType.Physical),
                new Clue("A shoe print matching {0}'s shoes was found in the {1}", ClueType.Physical),
                new Clue("{0}'s monogrammed handkerchief was found with traces of the victim's blood", ClueType.Physical),
                
                // Red herrings
                new Clue("Suspicious marks suggest a {0} might have been involved", ClueType.Weapon, true),
                new Clue("Something suspicious happened in the {0}", ClueType.Location, true),
                new Clue("A clock in the {0} was stopped at {1}", ClueType.TimeOfDeath, true),
                new Clue("{0} can't account for their whereabouts at {1}", ClueType.Alibi, true),
                new Clue("{0} was seen hurriedly leaving the {1} around {2}", ClueType.Behavioral, true)
            };
        }

        /// <summary>
        /// Gets clue templates of a specific type
        /// </summary>
        public List<Clue> GetByType(ClueType type, bool includeRedHerrings = true)
        {
            return GetAll()
                .Where(c => c.Type == type && (includeRedHerrings || !c.IsRedHerring))
                .ToList();
        }

        /// <summary>
        /// Gets clue templates that are genuine (not red herrings)
        /// </summary>
        public List<Clue> GetGenuineClues()
        {
            return GetAll().Where(c => !c.IsRedHerring).ToList();
        }

        /// <summary>
        /// Gets clue templates that are red herrings
        /// </summary>
        public List<Clue> GetRedHerrings()
        {
            return GetAll().Where(c => c.IsRedHerring).ToList();
        }

        /// <summary>
        /// Creates a weapon clue for the murder weapon
        /// </summary>
        public Clue CreateWeaponClue(string weapon, string location)
        {
            var template = GetRandomFromType(ClueType.Weapon, false);
            string description = string.Format(template.Description, weapon);

            return new Clue
            {
                Description = description,
                Type = ClueType.Weapon,
                RelatedTo = weapon,
                Location = location,
                IsRedHerring = false
            };
        }

        /// <summary>
        /// Creates a location clue for the murder room
        /// </summary>
        public Clue CreateLocationClue(string room, string victimName)
        {
            var template = GetRandomFromType(ClueType.Location, false);
            string description = string.Format(template.Description, victimName, room);

            return new Clue
            {
                Description = description,
                Type = ClueType.Location,
                RelatedTo = room,
                Location = room,
                IsRedHerring = false
            };
        }

        /// <summary>
        /// Creates a time of death clue
        /// </summary>
        public Clue CreateTimeOfDeathClue(string time, string victimName, string location)
        {
            var template = GetRandomFromType(ClueType.TimeOfDeath, false);
            string description = string.Format(template.Description, time, victimName);

            return new Clue
            {
                Description = description,
                Type = ClueType.TimeOfDeath,
                RelatedTo = time,
                Location = location,
                IsRedHerring = false
            };
        }

        /// <summary>
        /// Creates a motive clue
        /// </summary>
        public Clue CreateMotiveClue(Motive motive, Person murderer, Person victim, string location)
        {
            // Get a random template from the motive's templates
            string template = motive.Clues[_random.Next(motive.Clues.Length)];
            string description = string.Format(template, murderer.Name, victim.Name);

            return new Clue
            {
                Description = description,
                Type = ClueType.Motive,
                RelatedTo = motive.Name,
                Location = location,
                IsRedHerring = false
            };
        }

        /// <summary>
        /// Creates an alibi clue
        /// </summary>
        public Clue CreateAlibiClue(Person witness, Person subject, string location, string time, string clueLocation)
        {
            var template = GetRandomFromType(ClueType.Alibi, false);
            string description = string.Format(template.Description, witness.Name, subject.Name, location, time);

            return new Clue
            {
                Description = description,
                Type = ClueType.Alibi,
                RelatedTo = subject.Name,
                Location = clueLocation,
                IsRedHerring = false
            };
        }

        /// <summary>
        /// Creates a red herring weapon clue
        /// </summary>
        public Clue CreateRedHerringWeaponClue(string falseWeapon, string location)
        {
            var template = GetRandomFromType(ClueType.Weapon, true);
            string description = string.Format(template.Description, falseWeapon);

            return new Clue
            {
                Description = description,
                Type = ClueType.Weapon,
                RelatedTo = falseWeapon,
                Location = location,
                IsRedHerring = true
            };
        }

        /// <summary>
        /// Creates a red herring location clue
        /// </summary>
        public Clue CreateRedHerringLocationClue(string falseRoom)
        {
            var template = GetRandomFromType(ClueType.Location, true);
            string description = string.Format(template.Description, falseRoom);

            return new Clue
            {
                Description = description,
                Type = ClueType.Location,
                RelatedTo = falseRoom,
                Location = falseRoom,
                IsRedHerring = true
            };
        }

        /// <summary>
        /// Creates a red herring motive clue
        /// </summary>
        public Clue CreateRedHerringMotiveClue(Motive falseMotive, Person falseSuspect, Person victim, string location)
        {
            string description = string.Format(
                "Evidence suggests {0} might have had a {1} motive regarding {2}",
                falseSuspect.Name,
                falseMotive.Name.ToLower(),
                victim.Name);

            return new Clue
            {
                Description = description,
                Type = ClueType.Motive,
                RelatedTo = falseMotive.Name,
                Location = location,
                IsRedHerring = true
            };
        }

        private Clue GetRandomFromType(ClueType type, bool redHerring)
        {
            var templates = GetAll()
                .Where(t => t.Type == type && t.IsRedHerring == redHerring)
                .ToList();

            if (!templates.Any())
            {
                // Fallback
                return new Clue("Evidence related to " + type.ToString(), type, redHerring);
            }

            return templates[_random.Next(templates.Count)];
        }
    }
}
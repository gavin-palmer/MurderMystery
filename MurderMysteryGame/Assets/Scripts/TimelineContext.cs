using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Generators;
using MurderMystery.Helpers;
using MurderMystery.Models;

namespace MurderMystery
{
    public class TimelineContext
    {
        public List<Person> People { get; private set; }
        public Person Victim { get; private set; }
        public Person Murderer { get; private set; }
        public Person FalseAccused { get; set; } 
        public string Weapon { get; private set; }
        public string Room { get; private set; }
        public string Motive { get; private set; } 
        public SecurityInfo SecurityInfo { get; set; } 

        public List<string> TimeSlots { get; private set; }

        public Dictionary<string, Dictionary<Person, string>> LocationsByTime { get; private set; }
        public List<TimelineEvent> Events { get; private set; }
        public List<Clue> Clues { get; set; }
        public Dictionary<(Person, Person), RelationshipType> Relationships { get; private set; }

        public List<Room> Rooms { get; set; }
        public List<string> RoomsWithCameras { get; set; }
        public string SecurityRoom { get; set; }
        public string SecurityPinCode { get; set; }

        public string MurderTime { get; set; }
        public string PreMurderArgumentTime { get; set; }

        public Random Random { get; private set; }

        private LocationGenerator _locationGenerator;
        private SecuritySystemGenerator _securityGenerator;
        private MurderSetupGenerator _murderGenerator;
        private StorylineManager _storylineManager;
        private ClueGenerator _clueGenerator;

        public TimelineContext(
            List<Person> people,
            Person victim,
            Person murderer,
            string weapon,
            string room,
            string motive,
            List<string> timeSlots)
        {
            People = people;
            Victim = victim;
            Murderer = murderer;
            Weapon = weapon;
            Room = room;
            Motive = motive;
            TimeSlots = timeSlots;

            LocationsByTime = new Dictionary<string, Dictionary<Person, string>>();
            Events = new List<TimelineEvent>();
            Clues = new List<Clue>();
            Relationships = new Dictionary<(Person, Person), RelationshipType>();
            RoomsWithCameras = new List<string>();
            Random = new Random();
            InitializeRelationships();
            InitializeGenerators();
        }

        private void InitializeRelationships()
        {
            foreach (var person1 in People)
            {
                foreach (var person2 in People)
                {
                    if (person1 != person2)
                    {
                        Relationships[(person1, person2)] = RelationshipType.Unknown;
                    }
                }
            }
        }

        private void InitializeGenerators()
        {
            _locationGenerator = new LocationGenerator(this, Random);
            _securityGenerator = new SecuritySystemGenerator(this, Random);
            _murderGenerator = new MurderSetupGenerator(this, Random);
            _storylineManager = new StorylineManager(Random);
            _clueGenerator = new ClueGenerator(this, Random);
        }

        public void GenerateTimeline()
        {
            // Step 1: Initialize locations
            _locationGenerator.InitializeLocations();

            // Step 2: Setup security systems
            _securityGenerator.SetupSecuritySystem();

            // Step 3: Setup murder and related events
            _murderGenerator.SetupMurder();
            _murderGenerator.CreatePreMurderArgument();
            _murderGenerator.CreateFalseAccusation();
            _murderGenerator.CreateWeaponMovement();
            _murderGenerator.GenerateAlibis();

            // Step 4: Create additional storylines
            int storylineCount = 2 + Random.Next(2);
            _storylineManager.ExecuteStorylines(this, storylineCount);

            // Step 5: Fill remaining events
            _locationGenerator.FillRemainingEvents();

            // Step 6: Generate and distribute clues
            _clueGenerator.GenerateClues();
            _clueGenerator.AddRedHerrings();
            _clueGenerator.DistributeClues();
        }

        // Helper methods for storylines and generators to use
        public void AddEvent(TimelineEvent evt)
        {
            Events.Add(evt);
        }

        public void AddClue(Clue clue)
        {
            Clues.Add(clue);
        }

        public void SetRelationship(Person person1, Person person2, RelationshipType type)
        {
            Relationships[(person1, person2)] = type;
        }

        public RelationshipType GetRelationship(Person person1, Person person2)
        {
            if (Relationships.TryGetValue((person1, person2), out var relationship))
                return relationship;

            return RelationshipType.Unknown;
        }

        public List<Person> FindPeopleExcept(Func<Person, bool> predicate)
        {
            return People.Where(predicate).ToList();
        }

        public IEnumerable<string> GetTimesExcept(Func<string, bool> predicate)
        {
            return TimeSlots.Where(predicate);
        }

        public string GetRandomRoom()
        {
            return RandomHelper.PickRandom(Rooms.Select(r => r.Name).ToList());
        }
    }

    // Extension methods for easier access to common patterns
    public static class TimelineExtensions
    {
        public static bool HasSecretEvents(this Person person)
        {
            return person.TimelineEvents.Any(e => e.IsSecret);
        }
    }
}
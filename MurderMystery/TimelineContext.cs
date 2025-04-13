using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Helpers;
using MurderMystery.Models;

namespace MurderMystery.MurderMystery.Generators
{
    public class TimelineContext
    {
        // Mystery core elements
        public List<Person> People { get; private set; }
        public Person Victim { get; private set; }
        public Person Murderer { get; private set; }
        public string Weapon { get; private set; }
        public string Room { get; private set; }
        public string Motive { get; private set; }

        // Time tracking
        public List<string> TimeSlots { get; private set; }

        // Data collections
        public Dictionary<string, Dictionary<Person, string>> LocationsByTime { get; private set; }
        public List<TimelineEvent> Events { get; private set; }
        public List<Clue> Clues { get; private set; }
        public Dictionary<(Person, Person), RelationshipType> Relationships { get; private set; }

        // Random for generation
        private Random _random = new Random();

        public TimelineContext(
            List<Person> people,
            Person victim,
            Person murderer,
            string weapon,
            string room,
            string motive,
            List<string> timeSlots)
        {
            // Set the core elements
            People = people;
            Victim = victim;
            Murderer = murderer;
            Weapon = weapon;
            Room = room;
            Motive = motive;
            TimeSlots = timeSlots;

            // Initialize collections
            LocationsByTime = new Dictionary<string, Dictionary<Person, string>>();
            Events = new List<TimelineEvent>();
            Clues = new List<Clue>();
            Relationships = new Dictionary<(Person, Person), RelationshipType>();

            // Initialize relationships (start with Unknown)
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

        public void GenerateTimeline()
        {
            GenerateLocations();

            foreach (var time in TimeSlots)
            {
                ProcessTimeSlot(time);
            }

            AddRedHerrings();
        }

        private void GenerateLocations()
        {
            // Get all available rooms from our provider
            var availableRooms = DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList();

            // Assign random initial locations
            foreach (var time in TimeSlots)
            {
                LocationsByTime[time] = new Dictionary<Person, string>();

                foreach (var person in People)
                {
                    // First time slot is completely random
                    if (time == TimeSlots.First())
                    {
                        LocationsByTime[time][person] = RandomHelper.PickRandom(availableRooms);
                    }
                    else
                    {
                        // 70% chance to stay in the same room
                        string previousLocation = LocationsByTime[TimeSlots[TimeSlots.IndexOf(time) - 1]][person];

                        if (_random.NextDouble() < 0.7)
                        {
                            LocationsByTime[time][person] = previousLocation;
                        }
                        else
                        {
                            LocationsByTime[time][person] = RandomHelper.PickRandom(availableRooms);
                        }
                    }
                }
            }

            // Schedule the murder (between 7:00pm and 8:30pm)
            int startIndex = TimeSlots.IndexOf("7:00pm");
            int endIndex = TimeSlots.IndexOf("8:30pm");

            if (startIndex < 0) startIndex = TimeSlots.Count / 3;  // Fallback
            if (endIndex < 0) endIndex = (TimeSlots.Count * 2) / 3;  // Fallback

            int murderTimeIndex = _random.Next(startIndex, endIndex + 1);
            string murderTime = TimeSlots[murderTimeIndex];

            // Place victim and murderer in the murder room
            LocationsByTime[murderTime][Victim] = Room;
            LocationsByTime[murderTime][Murderer] = Room;

            // Victim doesn't move after murder
            for (int i = murderTimeIndex + 1; i < TimeSlots.Count; i++)
            {
                LocationsByTime[TimeSlots[i]][Victim] = Room;
            }
        }

        private void ProcessTimeSlot(string time)
        {
            // 1. Process interactions between people in the same rooms
            UpdateRelationships(time);

            // 2. Generate events for what each person is doing
            GenerateEvents(time);

            // 3. Create clues based on the events
            GenerateClues(time);
        }

        private void UpdateRelationships(string time)
        {
            // Group people by room
            var peopleByRoom = new Dictionary<string, List<Person>>();

            foreach (var person in People)
            {
                string location = LocationsByTime[time][person];

                if (!peopleByRoom.ContainsKey(location))
                {
                    peopleByRoom[location] = new List<Person>();
                }

                peopleByRoom[location].Add(person);
            }

            // Process each room
            foreach (var room in peopleByRoom.Keys)
            {
                var peopleInRoom = peopleByRoom[room];

                // Need at least 2 people for interaction
                if (peopleInRoom.Count < 2)
                    continue;

                // Process each pair
                for (int i = 0; i < peopleInRoom.Count; i++)
                {
                    for (int j = i + 1; j < peopleInRoom.Count; j++)
                    {
                        var person1 = peopleInRoom[i];
                        var person2 = peopleInRoom[j];

                        // Skip if one is victim and we're past murder
                        if ((person1 == Victim || person2 == Victim) && IsPastMurder(time))
                            continue;

                        // 10% chance for relationship to change
                        if (_random.NextDouble() < 0.1)
                        {
                            // If Unknown, give it a real relationship
                            if (Relationships[(person1, person2)] == RelationshipType.Unknown)
                            {
                                // Possible relationships with weights
                                var relationshipChoices = new List<(RelationshipType, int)>
                                {
                                    (RelationshipType.Friend, 70),
                                    (RelationshipType.Enemy, 20),
                                    (RelationshipType.Lover, 5),
                                    (RelationshipType.Spouse, 5)
                                };

                                // Special case for murderer and victim - bias toward negative relationship
                                if ((person1 == Murderer && person2 == Victim) ||
                                    (person1 == Victim && person2 == Murderer))
                                {
                                    relationshipChoices = new List<(RelationshipType, int)>
                                    {
                                        (RelationshipType.Friend, 20),
                                        (RelationshipType.Enemy, 60),
                                        (RelationshipType.Lover, 15),
                                        (RelationshipType.Spouse, 5)
                                    };
                                }

                                // Weighted random selection
                                int totalWeight = relationshipChoices.Sum(r => r.Item2);
                                int randomValue = _random.Next(totalWeight);
                                int currentWeight = 0;

                                foreach (var choice in relationshipChoices)
                                {
                                    currentWeight += choice.Item2;
                                    if (randomValue < currentWeight)
                                    {
                                        Relationships[(person1, person2)] = choice.Item1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GenerateEvents(string time)
        {
            var actionProvider = DataProviderFactory.Actions;

            foreach (var person in People)
            {
                if (person == Victim && IsPastMurder(time))
                    continue;

                string location = LocationsByTime[time][person];

                if (person == Murderer && IsAtMurder(time) && location == Room)
                {
                    Events.Add(new TimelineEvent
                    {
                        Time = time,
                        Person = person,
                        Location = location,
                        Action = $"killed {Victim.Name} using the {Weapon}",
                        IsSecret = true,
                        IsLie = false
                    }) ;
                    var availableRooms = DataProviderFactory.Rooms.GetAll().OrderBy(r => Guid.NewGuid()).Take(10).Select(r => r.Name).ToList();

                    var emptyRoom = availableRooms.FirstOrDefault(room => !LocationsByTime[time].Select(x => x.Value).Contains(room));
  
                    var actionData = actionProvider.GetRandomSoloAction();
                    var action = actionData.Description;
                    Events.Add(new TimelineEvent
                    {
                        Time = time,
                        Person = person,
                        Location = emptyRoom,
                        Action = action,
                        IsSecret = true,
                        IsLie = true
                    });
                }
                else
                {
                    var othersInRoom = People
                        .Where(p => p != person && p != Victim && LocationsByTime[time][p] == location)
                        .ToList();

                    string action;
                    Proof proof = Proof.None;
                    if (othersInRoom.Any() && _random.NextDouble() < 0.5)
                    {
                        var otherPerson = RandomHelper.PickRandom(othersInRoom);
                        proof = Proof.Alibi;
                        var relationship = Relationships[(person, otherPerson)];

                        var actionData = actionProvider.GetActionForRelationship(relationship);
                        action = string.Format(actionData.Description, otherPerson.Name);
                    }
                    else
                    {
                        var actionData = actionProvider.GetRandomSoloAction();
                        action = actionData.Description;
                        if (IsAtMurder(time) || _random.NextDouble() < 0.1)
                        {
                            proof = GenerateProof(location, person, actionData);
                        }
                    }

                    Events.Add(new TimelineEvent
                    {
                        Time = time,
                        Person = person,
                        Location = location,
                        Action = action,
                        IsSecret = false,
                        IsLie = false,
                        Proof = proof
                    });
                }
            }
        }

        private Proof GenerateProof(string location, Person person, Models.Action action)
        {
            if(ProofHelper.RoomHasSecurityCameraFootage(location)) {
                    var clue = new Clue($"The security cameras show {person.Name} in {location}.", ClueType.Alibi);
                clue.Location = "Security Room";
                Clues.Add(clue);

                return Proof.Surveillance;
            }
            else if (ProofHelper.IsOutside(location))
            {
                var clue = new Clue($"Footprints in the mud. It looks like someone wearing {person.Footwear.ToString()} walked through {location}", ClueType.Physical);
                clue.Location = location;
                Clues.Add(clue);
                return Proof.PhysicalEvidence;
            }
            else if (ProofHelper.LeftPhysicalEvidence(action))
            {
                var clue = new Clue($"a {action.PhysicalObject} was found in {location}", ClueType.Physical);
                clue.Location = location;
                Clues.Add(clue);
                return Proof.PhysicalEvidence;

            }
            return Proof.None;
        }
        private void GenerateClues(string time)
        {
            // Get clue provider
            var clueProvider = DataProviderFactory.Clues;

            // Get motive data
            var motiveData = DataProviderFactory.Motives.GetAll()
                .FirstOrDefault(m => m.Name == Motive) ??
                new Motive(Motive);

            // If this is murder time, create core clues
            if (IsAtMurder(time))
            {
                // Weapon clue
                Clues.Add(clueProvider.CreateWeaponClue(Weapon, Room));

                // Location clue
                Clues.Add(clueProvider.CreateLocationClue(Room, Victim.Name));

                // Time of death clue
                Clues.Add(clueProvider.CreateTimeOfDeathClue(time, Victim.Name, Room));

                // Generate alibis for people elsewhere
                foreach (var person in People.Where(p => p != Victim && p != Murderer))
                {
                    string personLocation = LocationsByTime[time][person];

                    if (personLocation != Room) // They were elsewhere
                    {
                        var witnesses = People
                            .Where(p => p != person && p != Victim &&
                                   LocationsByTime[time][p] == personLocation)
                            .ToList();

                        if (witnesses.Any())
                        {
                            var witness = RandomHelper.PickRandom(witnesses);
                            var randomRoom = DataProviderFactory.Rooms.GetRandom().Name;

                            Clues.Add(clueProvider.CreateAlibiClue(witness, person, personLocation, time, randomRoom));
                        }
                    }
                }
            }

            // Add motive clue halfway through
            if (time == TimeSlots[TimeSlots.Count / 2])
            {
                var randomRoom = DataProviderFactory.Rooms.GetRandom().Name;
                Clues.Add(clueProvider.CreateMotiveClue(motiveData, Murderer, Victim, randomRoom));
            }
        }

        private void AddRedHerrings()
        {
            // Get data providers
            var clueProvider = DataProviderFactory.Clues;
            var weaponProvider = DataProviderFactory.Weapons;
            var roomProvider = DataProviderFactory.Rooms;
            var motiveProvider = DataProviderFactory.Motives;

            // Add false weapon clue
            var falseWeapons = weaponProvider.GetAll()
                .Where(w => w.Name != Weapon)
                .Select(w => w.Name)
                .ToList();

            if (falseWeapons.Any())
            {
                var falseWeapon = RandomHelper.PickRandom(falseWeapons);
                var randomRoom = RandomHelper.PickRandom(
                    roomProvider.GetAll().Where(r => r.Name != Room).Select(r => r.Name).ToList());

                Clues.Add(clueProvider.CreateRedHerringWeaponClue(falseWeapon, randomRoom));
            }

            // Add false location clue
            var falseRooms = roomProvider.GetAll()
                .Where(r => r.Name != Room)
                .Select(r => r.Name)
                .ToList();

            if (falseRooms.Any())
            {
                var falseRoom = RandomHelper.PickRandom(falseRooms);
                Clues.Add(clueProvider.CreateRedHerringLocationClue(falseRoom));
            }

            // Add false motive clue
            var falseMotives = motiveProvider.GetAll()
                .Where(m => m.Name != Motive)
                .ToList();

            if (falseMotives.Any() && People.Count > 2)
            {
                var falseMotive = RandomHelper.PickRandom(falseMotives);
                var falseAccused = RandomHelper.PickRandom(
                    People.Where(p => p != Victim && p != Murderer).ToList());
                var randomRoom = RandomHelper.PickRandom(
                    roomProvider.GetAll().Select(r => r.Name).ToList());

                Clues.Add(clueProvider.CreateRedHerringMotiveClue(falseMotive, falseAccused, Victim, randomRoom));
            }
        }

        private bool IsAtMurder(string time)
        {
            return LocationsByTime[time][Victim] == Room &&
                   LocationsByTime[time][Murderer] == Room &&
                   !IsPastMurder(time);
        }


        private bool IsPastMurder(string time)
        {
            string murderTime = null;

            // Find murder time
            foreach (var t in TimeSlots)
            {
                if (LocationsByTime[t][Victim] == Room &&
                    LocationsByTime[t][Murderer] == Room)
                {
                    murderTime = t;
                    break;
                }
            }

            if (murderTime == null)
                return false;

            return TimeSlots.IndexOf(time) > TimeSlots.IndexOf(murderTime);
        }

   }
}
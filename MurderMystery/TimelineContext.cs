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
        public List<Person> People { get; private set; }
        public Person Victim { get; private set; }
        public Person Murderer { get; private set; }
        public string Weapon { get; private set; }
        public string Room { get; private set; }
        public string Motive { get; private set; }

        public List<string> TimeSlots { get; private set; }

        public Dictionary<string, Dictionary<Person, string>> LocationsByTime { get; private set; }
        public List<TimelineEvent> Events { get; private set; }
        public List<Clue> Clues { get; private set; }
        public Dictionary<(Person, Person), RelationshipType> Relationships { get; private set; }

        public List<Room> Rooms { get; set; }
        public List<string> RoomsWithCameras { get; private set; }
        public string SecurityRoom { get; private set; }
        public string SecurityPinCode { get; private set; }

        private string _murderTime;
        private string _preMurderArgumentTime;
        private Person _falseAccused;

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
            InitializeLocations();
            SetupSecuritySystem();
            SetupMurder();
            CreatePreMurderArgument();
            CreateFalseAccusation();
            CreateWeaponMovement();
            GenerateAlibis();
            CreateAdditionalStorylines();
            FillRemainingEvents();
            GenerateClues();
            AddRedHerrings();
            DistributeClues();
        }

        private void InitializeLocations()
        {
            int roomCount = _random.Next(9, 13);
            var nonePlayerRooms = DataProviderFactory.Rooms.LoadNonePlayerRooms();
            foreach (var room in nonePlayerRooms)
            {
                room.IsSpecialRoom = true;
            }
            var allRooms = DataProviderFactory.Rooms.GetAll();
            var murderRoom = allRooms.FirstOrDefault(x => x.Name == Room);
            var rooms = allRooms
                .Where(x => x.Name != murderRoom.Name)
                .Take(10)
                .Select(r => new Room(r.Name, r.Description))
                .ToList();
            rooms.Add(murderRoom);
            rooms.AddRange(nonePlayerRooms);
            rooms = rooms.OrderBy(x => Guid.NewGuid()).ToList();
            Rooms = rooms;
            foreach (var time in TimeSlots)
            {
                LocationsByTime[time] = new Dictionary<Person, string>();

                foreach (var person in People)
                {
                    LocationsByTime[time][person] = RandomHelper.PickRandom(Rooms.Select(x => x.Name).ToList());
                }
            }
        }

        private void SetupSecuritySystem()
        {
            SecurityRoom = "Security Room";

            SecurityPinCode = _random.Next(1000, 10000).ToString();

            var pinHintClue = new Clue($"Security room pin code: {SecurityPinCode}", ClueType.Security);
            pinHintClue.Location = RandomHelper.PickRandom(DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());
            Clues.Add(pinHintClue);

            var availableRooms = DataProviderFactory.Rooms.GetAll()
                .Where(r => r.Name != Room && r.Name != SecurityRoom)
                .Select(r => r.Name)
                .ToList();

            int cameraCount = Math.Min(3, availableRooms.Count);
            RoomsWithCameras = RandomHelper.PickMultipleRandom(availableRooms, cameraCount);

            foreach (var cameraRoom in RoomsWithCameras)
            {
                var cameraClue = new Clue($"Security camera installed in {cameraRoom}", ClueType.Security);
                cameraClue.Location = SecurityRoom;
                Clues.Add(cameraClue);
            }
        }

        private void SetupMurder()
        {
            int startIndex = TimeSlots.IndexOf("7:00pm");
            int endIndex = TimeSlots.IndexOf("8:30pm");

            if (startIndex < 0) startIndex = TimeSlots.Count / 3;
            if (endIndex < 0) endIndex = (TimeSlots.Count * 2) / 3;

            int murderTimeIndex = _random.Next(startIndex, endIndex + 1);
            _murderTime = TimeSlots[murderTimeIndex];

            LocationsByTime[_murderTime][Victim] = Room;
            LocationsByTime[_murderTime][Murderer] = Room;

            for (int i = murderTimeIndex + 1; i < TimeSlots.Count; i++)
            {
                LocationsByTime[TimeSlots[i]][Victim] = Room;
            }

            var murderEvent = new TimelineEvent()
            {
                Time = _murderTime,
                Person = Murderer,
                Location = Room,
                Action = $"killed {Victim.Name} using the {Weapon}",
                IsSecret = true,
                IsLie = false
            };
            Events.Add(murderEvent);
            Murderer.TimelineEvents.Add(murderEvent);

            var availableRooms = DataProviderFactory.Rooms.GetAll()
                .Where(r => r.Name != Room)
                .Select(r => r.Name)
                .ToList();

            var falseLocation = RandomHelper.PickRandom(availableRooms);
            var actionProvider = DataProviderFactory.Actions;
            var actionData = actionProvider.GetRandomSoloAction();

            var lieEvent = new TimelineEvent
            {
                Time = _murderTime,
                Person = Murderer,
                Location = falseLocation,
                Action = actionData.Description,
                IsSecret = false,
                IsLie = true
            };
            Events.Add(lieEvent);
            Murderer.TimelineEvents.Add(lieEvent);
        }

        private void CreatePreMurderArgument()
        {
            int murderIndex = TimeSlots.IndexOf(_murderTime);
            int earliestPossible = Math.Max(0, murderIndex - 4);
            int argumentIndex = _random.Next(earliestPossible, murderIndex);
            _preMurderArgumentTime = TimeSlots[argumentIndex];

            string argumentRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());

            LocationsByTime[_preMurderArgumentTime][Murderer] = argumentRoom;
            LocationsByTime[_preMurderArgumentTime][Victim] = argumentRoom;

            var argumentReasons = new List<string>
            {
                "money owed",
                "romantic jealousy",
                "work rivalry",
                "family dispute",
                "betrayed trust",
                "stolen property",
                "revealed secrets",
                "threatened blackmail",
                "political disagreement",
                "business competition"
            };

            string argumentReason = RandomHelper.PickRandom(argumentReasons);

            var argumentEvent = new TimelineEvent()
            {
                Time = _preMurderArgumentTime,
                Person = Murderer,
                Location = argumentRoom,
                Action = $"had a heated argument with {Victim.Name} about {argumentReason}",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(argumentEvent);
            Murderer.TimelineEvents.Add(argumentEvent);

            var victimArgumentEvent = new TimelineEvent()
            {
                Time = _preMurderArgumentTime,
                Person = Victim,
                Location = argumentRoom,
                Action = $"argued furiously with {Murderer.Name} regarding {argumentReason}",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(victimArgumentEvent);
            Victim.TimelineEvents.Add(victimArgumentEvent);

            bool isRoomWithCamera = RoomsWithCameras.Contains(argumentRoom);

            if (isRoomWithCamera)
            {
                var cameraClue = new Clue(
                    $"Security footage shows {Murderer.Name} and {Victim.Name} having a heated argument in {argumentRoom} at {_preMurderArgumentTime}",
                    ClueType.Testimony
                );
                cameraClue.Location = SecurityRoom;
                Clues.Add(cameraClue);
            }
            else
            {
                var potentialWitnesses = People
                    .Where(p => p != Murderer && p != Victim)
                    .ToList();

                if (potentialWitnesses.Any())
                {
                    var witness = RandomHelper.PickRandom(potentialWitnesses);

                    LocationsByTime[_preMurderArgumentTime][witness] = argumentRoom;

                    var witnessEvent = new TimelineEvent()
                    {
                        Time = _preMurderArgumentTime,
                        Person = witness,
                        Location = argumentRoom,
                        Action = $"overheard {Murderer.Name} and {Victim.Name} arguing about {argumentReason}",
                        IsSecret = false,
                        IsLie = false
                    };
                    Events.Add(witnessEvent);
                    witness.TimelineEvents.Add(witnessEvent);
                }
            }

            Relationships[(Murderer, Victim)] = RelationshipType.Enemy;
            Relationships[(Victim, Murderer)] = RelationshipType.Enemy;
        }

        private void CreateFalseAccusation()
        {
            var potentialFalseAccused = People
                .Where(p => p != Murderer && p != Victim)
                .ToList();

            if (!potentialFalseAccused.Any()) return;

            _falseAccused = RandomHelper.PickRandom(potentialFalseAccused);

            var availableTimes = TimeSlots
                .Where(t => t != _murderTime && t != _preMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return;

            string confrontationTime = RandomHelper.PickRandom(availableTimes);

            string confrontationRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());

            LocationsByTime[confrontationTime][Murderer] = confrontationRoom;
            LocationsByTime[confrontationTime][_falseAccused] = confrontationRoom;

            var confrontationReasons = new List<string>
            {
                "past debt",
                "romantic interest in the same person",
                "professional rivalry",
                "social status",
                "overheard secrets",
                "unfair treatment",
                "political disagreement",
                "rude behavior",
                "personality clash",
                "broken promise"
            };

            string confrontationReason = RandomHelper.PickRandom(confrontationReasons);

            var murdererConfrontation = new TimelineEvent()
            {
                Time = confrontationTime,
                Person = Murderer,
                Location = confrontationRoom,
                Action = $"had a public disagreement with {_falseAccused.Name} about {confrontationReason}",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(murdererConfrontation);
            Murderer.TimelineEvents.Add(murdererConfrontation);

            var accusedConfrontation = new TimelineEvent()
            {
                Time = confrontationTime,
                Person = _falseAccused,
                Location = confrontationRoom,
                Action = $"was involved in a heated exchange with {Murderer.Name} regarding {confrontationReason}",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(accusedConfrontation);
            _falseAccused.TimelineEvents.Add(accusedConfrontation);

            bool isRoomWithCamera = RoomsWithCameras.Contains(confrontationRoom);

            if (isRoomWithCamera)
            {
                var cameraClue = new Clue(
                    $"Security footage shows {Murderer.Name} and {_falseAccused.Name} having a confrontation in {confrontationRoom}",
                    ClueType.Testimony
                );
                cameraClue.Location = SecurityRoom;
                Clues.Add(cameraClue);
            }
            else
            {
                var potentialWitnesses = People
                    .Where(p => p != Murderer && p != Victim && p != _falseAccused)
                    .ToList();

                if (potentialWitnesses.Any())
                {
                    var witness = RandomHelper.PickRandom(potentialWitnesses);

                    LocationsByTime[confrontationTime][witness] = confrontationRoom;

                    var witnessEvent = new TimelineEvent()
                    {
                        Time = confrontationTime,
                        Person = witness,
                        Location = confrontationRoom,
                        Action = $"witnessed {Murderer.Name} confronting {_falseAccused.Name}",
                        IsSecret = false,
                        IsLie = false
                    };
                    Events.Add(witnessEvent);
                    witness.TimelineEvents.Add(witnessEvent);
                }
            }

            Relationships[(Murderer, _falseAccused)] = RelationshipType.Enemy;
            Relationships[(_falseAccused, Murderer)] = RelationshipType.Enemy;

            EnsureStrongAlibi(_falseAccused, _murderTime);
        }

        private void EnsureStrongAlibi(Person person, string time)
        {
            string currentLocation = LocationsByTime[time][person];
            if (RoomsWithCameras.Contains(currentLocation))
            {
                return;
            }

            if (RoomsWithCameras.Any())
            {
                string alibiRoom = RandomHelper.PickRandom(RoomsWithCameras);
                LocationsByTime[time][person] = alibiRoom;
                return;
            }

            var potentialWitnesses = People
                .Where(p => p != person && p != Murderer && p != Victim)
                .ToList();

            if (potentialWitnesses.Any())
            {
                var witness = RandomHelper.PickRandom(potentialWitnesses);
                string alibiRoom = RandomHelper.PickRandom(
                    DataProviderFactory.Rooms.GetAll()
                        .Where(r => r.Name != Room)
                        .Select(r => r.Name)
                        .ToList());

                LocationsByTime[time][person] = alibiRoom;
                LocationsByTime[time][witness] = alibiRoom;

                var actionProvider = DataProviderFactory.Actions;
                var actionData = actionProvider.GetRandomSoloAction();

                var alibiEvent = new TimelineEvent()
                {
                    Time = time,
                    Person = person,
                    Location = alibiRoom,
                    Action = actionData.Description,
                    IsSecret = false,
                    IsLie = false,
                    Proof = Proof.Alibi
                };
                Events.Add(alibiEvent);
                person.TimelineEvents.Add(alibiEvent);

                var witnessEvent = new TimelineEvent()
                {
                    Time = time,
                    Person = witness,
                    Location = alibiRoom,
                    Action = $"was with {person.Name} in {alibiRoom}",
                    IsSecret = false,
                    IsLie = false
                };
                Events.Add(witnessEvent);
                witness.TimelineEvents.Add(witnessEvent);
            }
            else
            {
                string alibiRoom = RandomHelper.PickRandom(
                    DataProviderFactory.Rooms.GetAll()
                        .Where(r => r.Name != Room)
                        .Select(r => r.Name)
                        .ToList());

                LocationsByTime[time][person] = alibiRoom;

                var actionProvider = DataProviderFactory.Actions;
                var actionData = actionProvider.GetRandomSoloAction();

                var alibiEvent = new TimelineEvent()
                {
                    Time = time,
                    Person = person,
                    Location = alibiRoom,
                    Action = actionData.Description,
                    IsSecret = false,
                    IsLie = false,
                    Proof = Proof.PhysicalEvidence
                };
                Events.Add(alibiEvent);
                person.TimelineEvents.Add(alibiEvent);

                Clues.Add(new Clue(
                    $"{person.Name}'s fingerprints were found on items in {alibiRoom} around the time of the murder",
                    ClueType.Physical
                )
                { Location = alibiRoom });
            }
        }

        private void CreateWeaponMovement()
        {
            int murderIndex = TimeSlots.IndexOf(_murderTime);
            if (murderIndex >= TimeSlots.Count - 1) return;

            string afterMurderTime = TimeSlots[murderIndex + 1];

            var potentialMovers = People
                .Where(p => p != Victim && p != Murderer)
                .ToList();

            if (!potentialMovers.Any()) return;

            var weaponMover = RandomHelper.PickRandom(potentialMovers);

            LocationsByTime[afterMurderTime][weaponMover] = Room;

            string destinationRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll()
                    .Where(r => r.Name != Room)
                    .Select(r => r.Name)
                    .ToList());

            var weaponMovementEvent = new TimelineEvent()
            {
                Time = afterMurderTime,
                Person = weaponMover,
                Location = Room,
                Action = $"found a {Weapon} and moved it to {destinationRoom} without realizing its significance",
                IsSecret = true,
                IsLie = false
            };
            Events.Add(weaponMovementEvent);
            weaponMover.TimelineEvents.Add(weaponMovementEvent);

            Clues.Add(new Clue(
                $"A {Weapon} with blood traces was found in {destinationRoom}",
                ClueType.Physical
            )
            { Location = destinationRoom });

            var falseMemoryEvent = new TimelineEvent()
            {
                Time = afterMurderTime,
                Person = weaponMover,
                Location = Room,
                Action = "found something odd but can't quite remember what it was",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(falseMemoryEvent);
            weaponMover.TimelineEvents.Add(falseMemoryEvent);
        }

        private void GenerateAlibis()
        {
            foreach (var person in People.Where(p => p != Victim && p != Murderer && p != _falseAccused))
            {
                if (Events.Any(e => e.Person == person && e.Time == _murderTime))
                    continue;

                EnsureStrongAlibi(person, _murderTime);
            }
        }

        private void CreateAdditionalStorylines()
        {
            var storylines = new List<System.Action>
            {
                CreateAffairStoryline,
                CreateSecretDiscoveryStoryline,
                CreateTheftStoryline,
                CreateDrunkennessStoryline,
                CreateMysteriousPhoneCallStoryline
            };

            int storylineCount = Math.Min(2 + _random.Next(2), storylines.Count);
            var selectedStorylines = RandomHelper.PickMultipleRandom(storylines, storylineCount);

            foreach (var storyline in selectedStorylines)
            {
                storyline();
            }
        }

        private void CreateAffairStoryline()
        {
            var availablePeople = People
                .Where(p => p != Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                .ToList();

            if (availablePeople.Count < 2) return;

            var lover1 = RandomHelper.PickRandom(availablePeople);
            availablePeople.Remove(lover1);
            var lover2 = RandomHelper.PickRandom(availablePeople);

            var availableTimes = TimeSlots
                .Where(t => t != _murderTime && t != _preMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return;

            string meetingTime = RandomHelper.PickRandom(availableTimes);

            string meetingRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());

            LocationsByTime[meetingTime][lover1] = meetingRoom;
            LocationsByTime[meetingTime][lover2] = meetingRoom;

            var affair1Event = new TimelineEvent()
            {
                Time = meetingTime,
                Person = lover1,
                Location = meetingRoom,
                Action = $"had a secret romantic rendezvous with {lover2.Name}",
                IsSecret = true,
                IsLie = false
            };
            Events.Add(affair1Event);
            lover1.TimelineEvents.Add(affair1Event);

            var affair2Event = new TimelineEvent()
            {
                Time = meetingTime,
                Person = lover2,
                Location = meetingRoom,
                Action = $"met secretly with {lover1.Name} for a romantic affair",
                IsSecret = true,
                IsLie = false
            };
            Events.Add(affair2Event);
            lover2.TimelineEvents.Add(affair2Event);

            Relationships[(lover1, lover2)] = RelationshipType.Lover;
            Relationships[(lover2, lover1)] = RelationshipType.Lover;

            var potentialGossips = People
                .Where(p => p != lover1 && p != lover2 && p != Victim)
                .ToList();

            if (!potentialGossips.Any()) return;

            var gossiper = RandomHelper.PickRandom(potentialGossips);

            var gossipEvent = new TimelineEvent()
            {
                Time = RandomHelper.PickRandom(availableTimes),
                Person = gossiper,
                Location = RandomHelper.PickRandom(DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList()),
                Action = $"heard rumors about {lover1.Name} and {lover2.Name} having a secret relationship",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(gossipEvent);
            gossiper.TimelineEvents.Add(gossipEvent);
        }

        private void CreateSecretDiscoveryStoryline()
        {
            var availablePeople = People
                .Where(p => p != Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                .ToList();

            if (availablePeople.Count < 2) return;

            var secretHolder = RandomHelper.PickRandom(availablePeople);
            availablePeople.Remove(secretHolder);
            var discoverer = RandomHelper.PickRandom(availablePeople);

            var availableTimes = TimeSlots
                .Where(t => t != _murderTime && t != _preMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return;

            string discoveryTime = RandomHelper.PickRandom(availableTimes);

            var secrets = new List<string>
            {
                "hidden financial problems",
                "secret identity",
                "past criminal record",
                "embarrassing personal history",
                "medical condition",
                "family secret",
                "professional misconduct",
                "secret addiction",
                "hidden inheritance",
                "double life"
            };

            string secretType = RandomHelper.PickRandom(secrets);

            var discoveryEvent = new TimelineEvent()
            {
                Time = discoveryTime,
                Person = discoverer,
                Location = LocationsByTime[discoveryTime][discoverer],
                Action = $"discovered {secretHolder.Name}'s {secretType}",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(discoveryEvent);
            discoverer.TimelineEvents.Add(discoveryEvent);

            Clues.Add(new Clue(
                $"Evidence of {secretHolder.Name}'s {secretType} was found",
                ClueType.Testimony
            )
            { Location = LocationsByTime[discoveryTime][discoverer] });
        }

        private void CreateTheftStoryline()
        {
            var availablePeople = People
                .Where(p => p != Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                .ToList();

            if (!availablePeople.Any()) return;

            var thief = RandomHelper.PickRandom(availablePeople);

            var availableTimes = TimeSlots
                .Where(t => t != _murderTime && t != _preMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return;

            string theftTime = RandomHelper.PickRandom(availableTimes);

            var stolenItems = new List<string>
            {
                "valuable jewelry",
                "important document",
                "sentimental keepsake",
                "antique heirloom",
                "cash",
                "digital device",
                "rare collectible",
                "access card",
                "prescription medication",
                "personal diary"
            };

            string stolenItem = RandomHelper.PickRandom(stolenItems);

            var theftRoom = LocationsByTime[theftTime][thief];

            var theftEvent = new TimelineEvent()
            {
                Time = theftTime,
                Person = thief,
                Location = theftRoom,
                Action = $"stole {stolenItem} from {theftRoom}",
                IsSecret = true,
                IsLie = false
            };
            Events.Add(theftEvent);
            thief.TimelineEvents.Add(theftEvent);

            var falseAlibiRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll()
                    .Where(r => r.Name != theftRoom)
                    .Select(r => r.Name)
                    .ToList());

            var falseAlibiEvent = new TimelineEvent()
            {
                Time = theftTime,
                Person = thief,
                Location = falseAlibiRoom,
                Action = "claims to have been alone",
                IsSecret = false,
                IsLie = true
            };
            Events.Add(falseAlibiEvent);
            thief.TimelineEvents.Add(falseAlibiEvent);

            Clues.Add(new Clue(
                $"{stolenItem} was reported missing from {theftRoom}",
                ClueType.Testimony
            )
            { Location = theftRoom });
        }

        private void CreateDrunkennessStoryline()
        {
            var availablePeople = People
                .Where(p => p != Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                .ToList();

            if (!availablePeople.Any()) return;

            var drunkPerson = RandomHelper.PickRandom(availablePeople);

            var availableTimes = TimeSlots
                .Where(t => t != _murderTime && t != _preMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return;

            string drunkTime = RandomHelper.PickRandom(availableTimes);

            var drunkenActions = new List<string>
            {
                "got very drunk and started singing loudly",
                "drank too much and fell asleep on the floor",
                "became intoxicated and told inappropriate stories",
                "had one too many drinks and tried to dance on the table",
                "got drunk and made a scene",
                "became inebriated and confessed their crush on someone",
                "drank heavily and accidentally broke something valuable",
                "got tipsy and revealed embarrassing secrets about themselves",
                "had too much to drink and challenged people to arm wrestling",
                "became drunk and attempted to recite poetry"
            };

            string drunkenAction = RandomHelper.PickRandom(drunkenActions);

            var drunkRoom = LocationsByTime[drunkTime][drunkPerson];

            var drunkEvent = new TimelineEvent()
            {
                Time = drunkTime,
                Person = drunkPerson,
                Location = drunkRoom,
                Action = drunkenAction,
                IsSecret = false,
                IsLie = false
            };
            Events.Add(drunkEvent);
            drunkPerson.TimelineEvents.Add(drunkEvent);

            var potentialWitnesses = People
                .Where(p => p != drunkPerson && p != Victim)
                .ToList();

            if (!potentialWitnesses.Any()) return;

            var witness = RandomHelper.PickRandom(potentialWitnesses);
            LocationsByTime[drunkTime][witness] = drunkRoom;

            var witnessEvent = new TimelineEvent()
            {
                Time = drunkTime,
                Person = witness,
                Location = drunkRoom,
                Action = $"witnessed {drunkPerson.Name}'s drunken behavior",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(witnessEvent);
            witness.TimelineEvents.Add(witnessEvent);

            if (availableTimes.Count > 1)
            {
                availableTimes.Remove(drunkTime);
                string memoryGapTime = RandomHelper.PickRandom(availableTimes);

                var memoryGapEvent = new TimelineEvent()
                {
                    Time = memoryGapTime,
                    Person = drunkPerson,
                    Location = LocationsByTime[memoryGapTime][drunkPerson],
                    Action = "can't remember what happened due to intoxication",
                    IsSecret = false,
                    IsLie = false
                };
                Events.Add(memoryGapEvent);
                drunkPerson.TimelineEvents.Add(memoryGapEvent);
            }
        }

        private void CreateMysteriousPhoneCallStoryline()
        {
            var availablePeople = People
                .Where(p => p != Victim && !p.TimelineEvents.Any(e => e.IsSecret))
                .ToList();

            if (!availablePeople.Any()) return;

            var receiver = RandomHelper.PickRandom(availablePeople);

            var availableTimes = TimeSlots
                .Where(t => t != _murderTime && t != _preMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return;

            string callTime = RandomHelper.PickRandom(availableTimes);

            var callContents = new List<string>
            {
                "a mysterious threat",
                "shocking news from an unknown caller",
                "a hushed and urgent conversation",
                "an emotional and tense discussion",
                "what sounded like blackmail",
                "an argument with someone they refused to name",
                "disturbing information from an anonymous source",
                "a tearful conversation",
                "an offer they seemed reluctant to accept",
                "news that clearly upset them"
            };

            string callContent = RandomHelper.PickRandom(callContents);

            var callRoom = LocationsByTime[callTime][receiver];

            var callEvent = new TimelineEvent()
            {
                Time = callTime,
                Person = receiver,
                Location = callRoom,
                Action = $"received a phone call about {callContent}",
                IsSecret = true,
                IsLie = false
            };
            Events.Add(callEvent);
            receiver.TimelineEvents.Add(callEvent);

            var publicEvent = new TimelineEvent()
            {
                Time = callTime,
                Person = receiver,
                Location = callRoom,
                Action = "took a phone call and seemed disturbed afterward",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(publicEvent);
            receiver.TimelineEvents.Add(publicEvent);

            var potentialOverhearers = People
                .Where(p => p != receiver && p != Victim)
                .ToList();

            if (!potentialOverhearers.Any()) return;

            var overhearer = RandomHelper.PickRandom(potentialOverhearers);
            LocationsByTime[callTime][overhearer] = callRoom;

            var overhearEvent = new TimelineEvent()
            {
                Time = callTime,
                Person = overhearer,
                Location = callRoom,
                Action = $"overheard part of {receiver.Name}'s phone conversation",
                IsSecret = false,
                IsLie = false
            };
            Events.Add(overhearEvent);
            overhearer.TimelineEvents.Add(overhearEvent);

            Clues.Add(new Clue(
                $"Phone records show {receiver.Name} received a call at {callTime}",
                ClueType.Testimony
            )
            { Location = "Study" });
        }

        private void FillRemainingEvents()
        {
            var actionProvider = DataProviderFactory.Actions;

            foreach (var person in People)
            {
                foreach (var time in TimeSlots)
                {
                    if (person == Victim && IsPastMurder(time))
                        continue;

                    if (Events.Any(e => e.Person == person && e.Time == time))
                        continue;

                    string location = LocationsByTime[time][person];

                    var othersInRoom = People
                        .Where(p => p != person && p != Victim &&
                               LocationsByTime[time][p] == location)
                        .ToList();

                    if (othersInRoom.Any() && _random.NextDouble() < 0.6)
                    {
                        var otherPerson = RandomHelper.PickRandom(othersInRoom);
                        var relationship = Relationships[(person, otherPerson)];

                        var actionData = actionProvider.GetActionForRelationship(relationship);
                        string action = string.Format(actionData.Description, otherPerson.Name);

                        var timelineEvent = new TimelineEvent
                        {
                            Time = time,
                            Person = person,
                            Location = location,
                            Action = action,
                            IsSecret = false,
                            IsLie = false,
                            Proof = Proof.Alibi
                        };
                        Events.Add(timelineEvent);
                        person.TimelineEvents.Add(timelineEvent);
                    }
                    else
                    {
                        var actionData = actionProvider.GetRandomSoloAction();
                        string action = actionData.Description;

                        var timelineEvent = new TimelineEvent
                        {
                            Time = time,
                            Person = person,
                            Location = location,
                            Action = action,
                            IsSecret = false,
                            IsLie = false,
                            Proof = DetermineProof(location, actionData)
                        };
                        Events.Add(timelineEvent);
                        person.TimelineEvents.Add(timelineEvent);
                    }
                }
            }
        }

        private Proof DetermineProof(string location, Models.Action action)
        {
            if (_random.NextDouble() < 0.1)
            {
                if (RoomsWithCameras.Contains(location))
                {
                    return Proof.Surveillance;
                }
                else if (ProofHelper.LeftPhysicalEvidence(action))
                {
                    return Proof.PhysicalEvidence;
                }
                else if (ProofHelper.IsOutside(location))
                {
                    return Proof.PhysicalEvidence;
                }
            }

            return Proof.None;
        }

        private void GenerateClues()
        {
            var clueProvider = DataProviderFactory.Clues;

            Clues.Add(clueProvider.CreateWeaponClue(Weapon, Room));
            Clues.Add(clueProvider.CreateLocationClue(Room, Victim.Name));
            Clues.Add(clueProvider.CreateTimeOfDeathClue(_murderTime, Victim.Name, Room));

            foreach (var evt in Events)
            {
                if (evt.IsLie || evt.IsSecret)
                    continue;

                switch (evt.Proof)
                {
                    case Proof.Surveillance:
                        if (RoomsWithCameras.Contains(evt.Location))
                        {
                            Clues.Add(new Clue(
                                $"Security footage shows {evt.Person.Name} in {evt.Location} at {evt.Time}",
                                ClueType.Alibi
                            )
                            { Location = SecurityRoom });
                        }
                        break;

                    case Proof.PhysicalEvidence:
                        if (ProofHelper.IsOutside(evt.Location))
                        {
                            Clues.Add(new Clue(
                                $"Footprints matching {evt.Person.Name}'s {evt.Person.Footwear} were found in {evt.Location}",
                                ClueType.Physical
                            )
                            { Location = evt.Location });
                        }
                        else
                        {
                            Clues.Add(new Clue(
                                $"{evt.Person.Name}'s fingerprints were found on items in {evt.Location}",
                                ClueType.Physical
                            )
                            { Location = evt.Location });
                        }
                        break;

                    case Proof.Alibi:
                        var witnesses = People
                            .Where(p => p != evt.Person && p != Victim &&
                                   LocationsByTime[evt.Time][p] == evt.Location)
                            .ToList();

                        if (witnesses.Any())
                        {
                            var witness = RandomHelper.PickRandom(witnesses);
                            Clues.Add(new Clue(
                                $"{witness.Name} confirms seeing {evt.Person.Name} in {evt.Location} at {evt.Time}",
                                ClueType.Alibi
                            )
                            { Location = "Anywhere" });
                        }
                        break;
                }
            }

            var motiveData = DataProviderFactory.Motives.GetAll()
                .FirstOrDefault(m => m.Name == Motive) ??
                new Motive(Motive);

            string motiveClueLocation = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());

            Clues.Add(clueProvider.CreateMotiveClue(motiveData, Murderer, Victim, motiveClueLocation));
        }

        private void AddRedHerrings()
        {
            var clueProvider = DataProviderFactory.Clues;
            var weaponProvider = DataProviderFactory.Weapons;
            var roomProvider = DataProviderFactory.Rooms;
            var motiveProvider = DataProviderFactory.Motives;

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

            var falseRooms = roomProvider.GetAll()
                .Where(r => r.Name != Room)
                .Select(r => r.Name)
                .ToList();

            if (falseRooms.Any())
            {
                var falseRoom = RandomHelper.PickRandom(falseRooms);
                Clues.Add(clueProvider.CreateRedHerringLocationClue(falseRoom));
            }

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

        private bool IsPastMurder(string time)
        {
            return TimeSlots.IndexOf(time) > TimeSlots.IndexOf(_murderTime);
        }

        private void DistributeClues()
        {
            GenerateClues();

            var assignedClues = new HashSet<Clue>();

            foreach (var clue in Clues.Where(c => c.Type == ClueType.Testimony || c.Type == ClueType.Alibi))
            {
                var knowledgeablePersons = FindPeopleWhoKnowClue(clue);

                if (knowledgeablePersons.Any())
                {
                    foreach (var person in knowledgeablePersons)
                    {
                        var personalClue = CloneClueWithPersonalPerspective(clue, person);
                        person.KnownClues.Add(personalClue);
                    }

                    assignedClues.Add(clue);
                }
            }

            var rooms = DataProviderFactory.Rooms.GetAll()
                .ToDictionary(r => r.Name, r => r);

            if (!rooms.ContainsKey(SecurityRoom))
            {
                rooms[SecurityRoom] = new Room(SecurityRoom, "A room filled with security monitors and equipment.");
            }

            AssignCluesToRooms(rooms);

            Clues = Clues.Except(assignedClues).ToList();
        }

        private List<Person> FindPeopleWhoKnowClue(Clue clue)
        {
            var knowledgeablePeople = new List<Person>();

            foreach (var person in People.Where(p => p != Victim))
            {
                var relatedEvents = person.TimelineEvents
                    .Where(e => !e.IsSecret && !e.IsLie)
                    .ToList();

                foreach (var evt in relatedEvents)
                {
                    if (ClueRelatesTo(clue, evt))
                    {
                        knowledgeablePeople.Add(person);
                        break;
                    }
                }

                if (clue.Type == ClueType.Alibi && clue.Location == SecurityRoom)
                {
                    if (person.TimelineEvents.Any(e => e.Location == SecurityRoom && !e.IsLie))
                    {
                        knowledgeablePeople.Add(person);
                    }
                }

                if (clue.Description.Contains("argument") || clue.Description.Contains("confrontation"))
                {
                    if (clue.Description.Contains(person.Name))
                    {
                        knowledgeablePeople.Add(person);
                    }
                }
            }

            return knowledgeablePeople;
        }

        private void AssignCluesToRooms(Dictionary<string, Room> rooms)
         {
            foreach (var clue in Clues.Where(c =>
                c.Type == ClueType.Physical ||
                c.Type == ClueType.Weapon ||
                c.Type == ClueType.Security))
            {
                if (string.IsNullOrEmpty(clue.Location))
                {
                    clue.Location = RandomHelper.PickRandom(rooms.Keys.ToList());
                }

                if (rooms.ContainsKey(clue.Location))
                {
                    rooms[clue.Location].PhysicalClues.Add(clue);
                }
            }

            if (rooms.ContainsKey(Room))
            {
                var bloodClue = new Clue($"Blood stains that appear to belong to {Victim.Name}.", ClueType.Physical);
                rooms[Room].PhysicalClues.Add(bloodClue);

                if (_random.NextDouble() < 0.7)
                {
                    var struggleClue = new Clue("Signs of a struggle - overturned furniture and broken items.", ClueType.Physical);
                    rooms[Room].PhysicalClues.Add(struggleClue);
                }
            }

            foreach (var evt in Events.Where(e => !e.IsLie && !e.IsSecret))
            {
                if (rooms.ContainsKey(evt.Location) && _random.NextDouble() < 0.2)
                {
                    var trace = CreatePhysicalTrace(evt);
                    if (trace != null)
                    {
                        rooms[evt.Location].PhysicalClues.Add(trace);
                    }
                }
            }
        }

        private Clue CreatePhysicalTrace(TimelineEvent evt)
        {
            if (evt.Action.Contains("drink") || evt.Action.Contains("eat"))
            {
                return new Clue($"Empty glass/plate that appears to have been used by {evt.Person.Name}.", ClueType.Physical);
            }
            else if (evt.Action.Contains("read") || evt.Action.Contains("write"))
            {
                return new Clue($"A book/paper with {evt.Person.Name}'s fingerprints on it.", ClueType.Physical);
            }
            else if (evt.Action.Contains("argument") || evt.Action.Contains("fight"))
            {
                return new Clue("Scratches on furniture suggesting a heated confrontation.", ClueType.Physical);
            }
            else if (evt.Action.Contains("phone"))
            {
                return new Clue("A phone with fingerprints that could be checked.", ClueType.Physical);
            }
            else if (evt.Action.Contains("sit") || evt.Action.Contains("sleep"))
            {
                return new Clue($"Indentation on furniture indicating someone was here. A hair matching {evt.Person.Name} was found.", ClueType.Physical);
            }

            if (_random.NextDouble() < 0.3)
            {
                var items = new List<string>
                {
                    "pen", "handkerchief", "button", "receipt", "ticket stub",
                    "note", "business card", "magazine", "bookmark", "coaster"
                };

                return new Clue($"A {RandomHelper.PickRandom(items)} that belongs to {evt.Person.Name}.", ClueType.Physical);
            }

            return null;
        }

        private bool ClueRelatesTo(Clue clue, TimelineEvent evt)
        {
            if (clue.Location == evt.Location && clue.Description.Contains(evt.Time))
                return true;

            if (clue.Description.Contains(evt.Person.Name))
                return true;

            if (clue.Description.Contains(evt.Action) || evt.Action.Contains(clue.Description))
                return true;

            return false;
        }

        private Clue CloneClueWithPersonalPerspective(Clue clue, Person person)
        {
            var personalClue = new Clue(clue.Description, clue.Type);

            if (clue.Type == ClueType.Testimony)
            {
                personalClue.Description = $"I saw {clue.Description.ToLower()}";
            }
            else if (clue.Type == ClueType.Alibi)
            {
                personalClue.Description = $"I can confirm that {clue.Description.ToLower()}";
            }

            if (_random.NextDouble() < 0.3)
            {
                personalClue.Description += " - at least that's what I remember.";
            }
            else if (_random.NextDouble() < 0.5)
            {
                personalClue.Description += " - I'm absolutely certain about this.";
            }

            return personalClue;
        }
    }
}
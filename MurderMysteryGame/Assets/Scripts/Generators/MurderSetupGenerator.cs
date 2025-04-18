using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Helpers;
using MurderMystery.Models;

namespace MurderMystery.Generators
{
    public class MurderSetupGenerator
    {
        private readonly TimelineContext _context;
        private readonly Random _random;

        public MurderSetupGenerator(TimelineContext context, Random random)
        {
            _context = context;
            _random = random;
        }

        public void SetupMurder()
        {
            // Choose murder time
            int startIndex = _context.TimeSlots.IndexOf("7:00pm");
            int endIndex = _context.TimeSlots.IndexOf("8:30pm");

            if (startIndex < 0) startIndex = _context.TimeSlots.Count / 3;
            if (endIndex < 0) endIndex = (_context.TimeSlots.Count * 2) / 3;

            int murderTimeIndex = _random.Next(startIndex, endIndex + 1);
            _context.MurderTime = _context.TimeSlots[murderTimeIndex];

            // Set locations for victim and murderer
            _context.LocationsByTime[_context.MurderTime][_context.Victim] = _context.Room;
            _context.LocationsByTime[_context.MurderTime][_context.Murderer] = _context.Room;

            // Keep victim in murder room after death
            for (int i = murderTimeIndex + 1; i < _context.TimeSlots.Count; i++)
            {
                _context.LocationsByTime[_context.TimeSlots[i]][_context.Victim] = _context.Room;
            }

            // Create the murder event (secret)
            var murderEvent = new TimelineEvent()
            {
                Time = _context.MurderTime,
                Person = _context.Murderer,
                Location = _context.Room,
                Action = $"killed {_context.Victim.Name} using the {_context.Weapon}",
                IsSecret = true,
                IsLie = false
            };
            _context.AddEvent(murderEvent);
            _context.Murderer.TimelineEvents.Add(murderEvent);

            // Create the murderer's alibi (lie)
            var availableRooms = DataProviderFactory.Rooms.GetAll()
                .Where(r => r.Name != _context.Room)
                .Select(r => r.Name)
                .ToList();

            var falseLocation = RandomHelper.PickRandom(availableRooms);
            var actionProvider = DataProviderFactory.Actions;
            var actionData = actionProvider.GetRandomSoloAction();

            var lieEvent = new TimelineEvent
            {
                Time = _context.MurderTime,
                Person = _context.Murderer,
                Location = falseLocation,
                Action = actionData.Description,
                IsSecret = false,
                IsLie = true
            };
            _context.AddEvent(lieEvent);
            _context.Murderer.TimelineEvents.Add(lieEvent);
        }

        public void CreatePreMurderArgument()
        {
            int murderIndex = _context.TimeSlots.IndexOf(_context.MurderTime);
            int earliestPossible = Math.Max(0, murderIndex - 4);
            int argumentIndex = _random.Next(earliestPossible, murderIndex);
            _context.PreMurderArgumentTime = _context.TimeSlots[argumentIndex];

            string argumentRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());

            _context.LocationsByTime[_context.PreMurderArgumentTime][_context.Murderer] = argumentRoom;
            _context.LocationsByTime[_context.PreMurderArgumentTime][_context.Victim] = argumentRoom;

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

            // Create argument events for both murderer and victim
            var argumentEvent = new TimelineEvent()
            {
                Time = _context.PreMurderArgumentTime,
                Person = _context.Murderer,
                Location = argumentRoom,
                Action = $"had a heated argument with {_context.Victim.Name} about {argumentReason}",
                IsSecret = false,
                IsLie = false
            };
            _context.AddEvent(argumentEvent);
            _context.Murderer.TimelineEvents.Add(argumentEvent);

            var victimArgumentEvent = new TimelineEvent()
            {
                Time = _context.PreMurderArgumentTime,
                Person = _context.Victim,
                Location = argumentRoom,
                Action = $"argued furiously with {_context.Murderer.Name} regarding {argumentReason}",
                IsSecret = false,
                IsLie = false
            };
            _context.AddEvent(victimArgumentEvent);
            _context.Victim.TimelineEvents.Add(victimArgumentEvent);

            // Add witness or camera evidence
            bool isRoomWithCamera = _context.RoomsWithCameras.Contains(argumentRoom);

            if (isRoomWithCamera)
            {
                var cameraClue = new Clue(
                    $"Security footage shows {_context.Murderer.Name} and {_context.Victim.Name} having a heated argument in {argumentRoom} at {_context.PreMurderArgumentTime}",
                    ClueType.Testimony
                );
                cameraClue.Location = _context.SecurityRoom;
                _context.AddClue(cameraClue);
            }
            else
            {
                var potentialWitnesses = _context.People
                    .Where(p => p != _context.Murderer && p != _context.Victim)
                    .ToList();

                if (potentialWitnesses.Any())
                {
                    var witness = RandomHelper.PickRandom(potentialWitnesses);

                    _context.LocationsByTime[_context.PreMurderArgumentTime][witness] = argumentRoom;

                    var witnessEvent = new TimelineEvent()
                    {
                        Time = _context.PreMurderArgumentTime,
                        Person = witness,
                        Location = argumentRoom,
                        Action = $"overheard {_context.Murderer.Name} and {_context.Victim.Name} arguing about {argumentReason}",
                        IsSecret = false,
                        IsLie = false
                    };
                    _context.AddEvent(witnessEvent);
                    witness.TimelineEvents.Add(witnessEvent);
                }
            }

            // Set relationship between murderer and victim
            _context.SetRelationship(_context.Murderer, _context.Victim, RelationshipType.Enemy);
            _context.SetRelationship(_context.Victim, _context.Murderer, RelationshipType.Enemy);
        }

        public void CreateFalseAccusation()
        {
            var potentialFalseAccused = _context.People
                .Where(p => p != _context.Murderer && p != _context.Victim)
                .ToList();

            if (!potentialFalseAccused.Any()) return;

            _context.FalseAccused = RandomHelper.PickRandom(potentialFalseAccused);

            var availableTimes = _context.TimeSlots
                .Where(t => t != _context.MurderTime && t != _context.PreMurderArgumentTime)
                .ToList();

            if (!availableTimes.Any()) return;

            string confrontationTime = RandomHelper.PickRandom(availableTimes);

            string confrontationRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());

            _context.LocationsByTime[confrontationTime][_context.Murderer] = confrontationRoom;
            _context.LocationsByTime[confrontationTime][_context.FalseAccused] = confrontationRoom;

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
                Person = _context.Murderer,
                Location = confrontationRoom,
                Action = $"had a public disagreement with {_context.FalseAccused.Name} about {confrontationReason}",
                IsSecret = false,
                IsLie = false
            };
            _context.AddEvent(murdererConfrontation);
            _context.Murderer.TimelineEvents.Add(murdererConfrontation);

            var accusedConfrontation = new TimelineEvent()
            {
                Time = confrontationTime,
                Person = _context.FalseAccused,
                Location = confrontationRoom,
                Action = $"was involved in a heated exchange with {_context.Murderer.Name} regarding {confrontationReason}",
                IsSecret = false,
                IsLie = false
            };
            _context.AddEvent(accusedConfrontation);
            _context.FalseAccused.TimelineEvents.Add(accusedConfrontation);

            bool isRoomWithCamera = _context.RoomsWithCameras.Contains(confrontationRoom);

            if (isRoomWithCamera)
            {
                var cameraClue = new Clue(
                    $"Security footage shows {_context.Murderer.Name} and {_context.FalseAccused.Name} having a confrontation in {confrontationRoom}",
                    ClueType.Testimony
                );
                cameraClue.Location = _context.SecurityRoom;
                _context.AddClue(cameraClue);
            }
            else
            {
                var potentialWitnesses = _context.People
                    .Where(p => p != _context.Murderer && p != _context.Victim && p != _context.FalseAccused)
                    .ToList();

                if (potentialWitnesses.Any())
                {
                    var witness = RandomHelper.PickRandom(potentialWitnesses);

                    _context.LocationsByTime[confrontationTime][witness] = confrontationRoom;

                    var witnessEvent = new TimelineEvent()
                    {
                        Time = confrontationTime,
                        Person = witness,
                        Location = confrontationRoom,
                        Action = $"witnessed {_context.Murderer.Name} confronting {_context.FalseAccused.Name}",
                        IsSecret = false,
                        IsLie = false
                    };
                    _context.AddEvent(witnessEvent);
                    witness.TimelineEvents.Add(witnessEvent);
                }
            }

            _context.SetRelationship(_context.Murderer, _context.FalseAccused, RelationshipType.Enemy);
            _context.SetRelationship(_context.FalseAccused, _context.Murderer, RelationshipType.Enemy);

            // Ensure false accused has strong alibi
            EnsureStrongAlibi(_context.FalseAccused, _context.MurderTime);
        }

        public void CreateWeaponMovement()
        {
            int murderIndex = _context.TimeSlots.IndexOf(_context.MurderTime);
            if (murderIndex >= _context.TimeSlots.Count - 1) return;

            string afterMurderTime = _context.TimeSlots[murderIndex + 1];

            var potentialMovers = _context.People
                .Where(p => p != _context.Victim && p != _context.Murderer)
                .ToList();

            if (!potentialMovers.Any()) return;

            var weaponMover = RandomHelper.PickRandom(potentialMovers);

            _context.LocationsByTime[afterMurderTime][weaponMover] = _context.Room;

            string destinationRoom = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll()
                    .Where(r => r.Name != _context.Room)
                    .Select(r => r.Name)
                    .ToList());

            var weaponMovementEvent = new TimelineEvent()
            {
                Time = afterMurderTime,
                Person = weaponMover,
                Location = _context.Room,
                Action = $"found a {_context.Weapon} and moved it to {destinationRoom} without realizing its significance",
                IsSecret = true,
                IsLie = false
            };
            _context.AddEvent(weaponMovementEvent);
            weaponMover.TimelineEvents.Add(weaponMovementEvent);

            _context.AddClue(new Clue(
                $"A {_context.Weapon} with blood traces was found in {destinationRoom}",
                ClueType.Physical
            )
            { Location = destinationRoom });

            var falseMemoryEvent = new TimelineEvent()
            {
                Time = afterMurderTime,
                Person = weaponMover,
                Location = _context.Room,
                Action = "found something odd but can't quite remember what it was",
                IsSecret = false,
                IsLie = false
            };
            _context.AddEvent(falseMemoryEvent);
            weaponMover.TimelineEvents.Add(falseMemoryEvent);
        }

        public void GenerateAlibis()
        {
            foreach (var person in _context.People.Where(p => p != _context.Victim && p != _context.Murderer && p != _context.FalseAccused))
            {
                if (_context.Events.Any(e => e.Person == person && e.Time == _context.MurderTime))
                    continue;

                EnsureStrongAlibi(person, _context.MurderTime);
            }
        }

        private void EnsureStrongAlibi(Person person, string time)
        {
            string currentLocation = _context.LocationsByTime[time][person];
            if (_context.RoomsWithCameras.Contains(currentLocation))
            {
                return;
            }

            if (_context.RoomsWithCameras.Any())
            {
                string alibiRoom = RandomHelper.PickRandom(_context.RoomsWithCameras);
                _context.LocationsByTime[time][person] = alibiRoom;
                return;
            }

            var potentialWitnesses = _context.People
                .Where(p => p != person && p != _context.Murderer && p != _context.Victim)
                .ToList();

            if (potentialWitnesses.Any())
            {
                var witness = RandomHelper.PickRandom(potentialWitnesses);
                string alibiRoom = RandomHelper.PickRandom(
                    DataProviderFactory.Rooms.GetAll()
                        .Where(r => r.Name != _context.Room)
                        .Select(r => r.Name)
                        .ToList());

                _context.LocationsByTime[time][person] = alibiRoom;
                _context.LocationsByTime[time][witness] = alibiRoom;

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
                _context.AddEvent(alibiEvent);
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
                _context.AddEvent(witnessEvent);
                witness.TimelineEvents.Add(witnessEvent);
            }
            else
            {
                string alibiRoom = RandomHelper.PickRandom(
                    DataProviderFactory.Rooms.GetAll()
                        .Where(r => r.Name != _context.Room)
                        .Select(r => r.Name)
                        .ToList());

                _context.LocationsByTime[time][person] = alibiRoom;

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
                _context.AddEvent(alibiEvent);
                person.TimelineEvents.Add(alibiEvent);

                _context.AddClue(new Clue(
                    $"{person.Name}'s fingerprints were found on items in {alibiRoom} around the time of the murder",
                    ClueType.Physical
                )
                { Location = alibiRoom });
            }
        }
    }
}
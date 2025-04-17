using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Helpers;
using MurderMystery.Models;

namespace MurderMystery.Generators
{
    public class ClueGenerator
    {
        private readonly TimelineContext _context;
        private readonly Random _random;

        public ClueGenerator(TimelineContext context, Random random)
        {
            _context = context;
            _random = random;
        }

        public void GenerateClues()
        {
            var clueProvider = DataProviderFactory.Clues;

            _context.AddClue(clueProvider.CreateWeaponClue(_context.Weapon, _context.Room));
            _context.AddClue(clueProvider.CreateLocationClue(_context.Room, _context.Victim.Name));
            _context.AddClue(clueProvider.CreateTimeOfDeathClue(_context.MurderTime, _context.Victim.Name, _context.Room));

            GenerateEventRelatedClues();
            GenerateMotiveClue();
        }

        public void AddRedHerrings()
        {
            var clueProvider = DataProviderFactory.Clues;
            var weaponProvider = DataProviderFactory.Weapons;
            var roomProvider = DataProviderFactory.Rooms;
            var motiveProvider = DataProviderFactory.Motives;

            // Add false weapon clue
            var falseWeapons = weaponProvider.GetAll()
                .Where(w => w.Name != _context.Weapon)
                .Select(w => w.Name)
                .ToList();

            if (falseWeapons.Any())
            {
                var falseWeapon = RandomHelper.PickRandom(falseWeapons);
                var randomRoom = RandomHelper.PickRandom(
                    roomProvider.GetAll().Where(r => r.Name != _context.Room).Select(r => r.Name).ToList());

                _context.AddClue(clueProvider.CreateRedHerringWeaponClue(falseWeapon, randomRoom));
            }

            // Add false location clue
            var falseRooms = roomProvider.GetAll()
                .Where(r => r.Name != _context.Room)
                .Select(r => r.Name)
                .ToList();

            if (falseRooms.Any())
            {
                var falseRoom = RandomHelper.PickRandom(falseRooms);
                _context.AddClue(clueProvider.CreateRedHerringLocationClue(falseRoom));
            }

            // Add false motive clue
            var falseMotives = motiveProvider.GetAll()
                .Where(m => m.Name != _context.Motive)
                .ToList();

            if (falseMotives.Any() && _context.People.Count > 2)
            {
                var falseMotive = RandomHelper.PickRandom(falseMotives);
                var falseAccused = RandomHelper.PickRandom(
                    _context.People.Where(p => p != _context.Victim && p != _context.Murderer).ToList());
                var randomRoom = RandomHelper.PickRandom(
                    roomProvider.GetAll().Select(r => r.Name).ToList());

                _context.AddClue(clueProvider.CreateRedHerringMotiveClue(falseMotive, falseAccused, _context.Victim, randomRoom));
            }
        }

        private void GenerateEventRelatedClues()
        {
            foreach (var evt in _context.Events)
            {
                if (evt.IsLie || evt.IsSecret)
                    continue;

                switch (evt.Proof)
                {
                    case Proof.Surveillance:
                        if (_context.RoomsWithCameras.Contains(evt.Location))
                        {
                            _context.AddClue(new Clue(
                                $"Security footage shows {evt.Person.Name} in {evt.Location} at {evt.Time}",
                                ClueType.Alibi
                            )
                            { Location = _context.SecurityRoom });
                        }
                        break;

                    case Proof.PhysicalEvidence:
                        if (ProofHelper.IsOutside(evt.Location))
                        {
                            _context.AddClue(new Clue(
                                $"Footprints matching {evt.Person.Name}'s {evt.Person.Footwear} were found in {evt.Location}",
                                ClueType.Physical
                            )
                            { Location = evt.Location });
                        }
                        else
                        {
                            _context.AddClue(new Clue(
                                $"{evt.Person.Name}'s fingerprints were found on items in {evt.Location}",
                                ClueType.Physical
                            )
                            { Location = evt.Location });
                        }
                        break;

                    case Proof.Alibi:
                        var witnesses = _context.People
                            .Where(p => p != evt.Person && p != _context.Victim &&
                                   _context.LocationsByTime[evt.Time][p] == evt.Location)
                            .ToList();

                        if (witnesses.Any())
                        {
                            var witness = RandomHelper.PickRandom(witnesses);
                            _context.AddClue(new Clue(
                                $"{witness.Name} confirms seeing {evt.Person.Name} in {evt.Location} at {evt.Time}",
                                ClueType.Alibi
                            )
                            { Location = "Anywhere" });
                        }
                        break;
                }
            }
        }

        private void GenerateMotiveClue()
        {
            var motiveData = DataProviderFactory.Motives.GetAll()
                .FirstOrDefault(m => m.Name == _context.Motive) ??
                new Motive(_context.Motive);

            string motiveClueLocation = RandomHelper.PickRandom(
                DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());

            _context.AddClue(DataProviderFactory.Clues.CreateMotiveClue(
                motiveData, _context.Murderer, _context.Victim, motiveClueLocation));
        }

        public void DistributeClues()
        {
            var assignedClues = new HashSet<Clue>();

            foreach (var clue in _context.Clues.Where(c => c.Type == ClueType.Testimony || c.Type == ClueType.Alibi))
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

            if (!rooms.ContainsKey(_context.SecurityRoom))
            {
                rooms[_context.SecurityRoom] = new Room(_context.SecurityRoom, "A room filled with security monitors and equipment.");
            }

            AssignCluesToRooms(rooms);

            _context.Clues = _context.Clues.Except(assignedClues).ToList();
        }

        private List<Person> FindPeopleWhoKnowClue(Clue clue)
        {
            var knowledgeablePeople = new List<Person>();

            foreach (var person in _context.People.Where(p => p != _context.Victim))
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

                if (clue.Type == ClueType.Alibi && clue.Location == _context.SecurityRoom)
                {
                    if (person.TimelineEvents.Any(e => e.Location == _context.SecurityRoom && !e.IsLie))
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
            foreach (var clue in _context.Clues.Where(c =>
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

            // Add murder scene clues
            if (rooms.ContainsKey(_context.Room))
            {
                var bloodClue = new Clue($"Blood stains that appear to belong to {_context.Victim.Name}.", ClueType.Physical);
                rooms[_context.Room].PhysicalClues.Add(bloodClue);

                if (_random.NextDouble() < 0.7)
                {
                    var struggleClue = new Clue("Signs of a struggle - overturned furniture and broken items.", ClueType.Physical);
                    rooms[_context.Room].PhysicalClues.Add(struggleClue);
                }
            }

            // Add traces for events
            foreach (var evt in _context.Events.Where(e => !e.IsLie && !e.IsSecret))
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
using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Helpers;
using MurderMystery.Models;

namespace MurderMystery.Generators
{
    public class LocationGenerator
    {
        private readonly TimelineContext _context;
        private readonly Random _random;

        public LocationGenerator(TimelineContext context, Random random)
        {
            _context = context;
            _random = random;
        }

        public void InitializeLocations()
        {
            int roomCount = _random.Next(9, 13);

            // Get special non-player rooms
            var nonePlayerRooms = DataProviderFactory.Rooms.LoadNonePlayerRooms();
            foreach (var room in nonePlayerRooms)
            {
                room.IsSpecialRoom = true;
            }

            // Get all available rooms and include the murder room
            var allRooms = DataProviderFactory.Rooms.GetAll();
            var murderRoom = allRooms.FirstOrDefault(x => x.Name == _context.Room);

            var rooms = allRooms
                .Where(x => x.Name != murderRoom.Name)
                .Take(10)
                .Select(r => new Room(r.Name, r.Description))
                .ToList();

            rooms.Add(murderRoom);
            rooms.AddRange(nonePlayerRooms);
            rooms = rooms.OrderBy(x => Guid.NewGuid()).ToList();
            _context.Rooms = rooms;

            // Initialize locations for each person at each time
            foreach (var time in _context.TimeSlots)
            {
                _context.LocationsByTime[time] = new Dictionary<Person, string>();

                foreach (var person in _context.People)
                {
                    _context.LocationsByTime[time][person] = RandomHelper.PickRandom(rooms.Select(x => x.Name).ToList());
                }
            }
        }

        public void FillRemainingEvents()
        {
            var actionProvider = DataProviderFactory.Actions;

            foreach (var person in _context.People)
            {
                foreach (var time in _context.TimeSlots)
                {
                    // Skip filling events for victim after murder
                    if (person == _context.Victim && IsPastMurder(time))
                        continue;

                    // Skip if this person already has an event at this time
                    if (_context.Events.Any(e => e.Person == person && e.Time == time))
                        continue;

                    string location = _context.LocationsByTime[time][person];

                    // Find others in the same room
                    var othersInRoom = _context.People
                        .Where(p => p != person && p != _context.Victim &&
                               _context.LocationsByTime[time][p] == location)
                        .ToList();

                    // Create social event or solo event
                    if (othersInRoom.Any() && _random.NextDouble() < 0.6)
                    {
                        var otherPerson = RandomHelper.PickRandom(othersInRoom);
                        var relationship = _context.GetRelationship(person, otherPerson);

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
                        _context.AddEvent(timelineEvent);
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
                        _context.AddEvent(timelineEvent);
                        person.TimelineEvents.Add(timelineEvent);
                    }
                }
            }
        }

        private Proof DetermineProof(string location, Models.Action action)
        {
            if (_random.NextDouble() < 0.1)
            {
                if (_context.RoomsWithCameras.Contains(location))
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

        private bool IsPastMurder(string time)
        {
            return _context.TimeSlots.IndexOf(time) > _context.TimeSlots.IndexOf(_context.MurderTime);
        }
    }
}
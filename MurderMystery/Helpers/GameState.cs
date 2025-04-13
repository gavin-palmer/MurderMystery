using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Models;
using MurderMystery.Enums;
using MurderMystery.Generators;

namespace MurderMystery
{
    public class GameState
    {
        // Mystery elements
        public Mystery CurrentMystery { get; private set; }

        // Mansion and navigation
        public Dictionary<string, Room> Mansion { get; private set; }
        public string CurrentRoom { get; set; } = "Foyer";

        // Player state
        public List<Clue> DiscoveredClues { get; private set; } = new List<Clue>();
        public string Interviewing { get; private set; }
        public List<string> InterviewedPeople { get; private set; } = new List<string>();

        // Game progress
        public bool GameOver { get; set; } = false;
        public int TurnCount { get; private set; } = 0;

        // Constructor
        public GameState(Mystery mystery, Dictionary<string, Room> mansion)
        {
            CurrentMystery = mystery;
            Mansion = mansion;

        }

        // Get people in the current room
        public List<Person> GetPeopleInCurrentRoom()
        {
            return CurrentMystery.People
                .Where(p => p != CurrentMystery.Victim && p.CurrentRoom == CurrentRoom)
                .ToList();
        }

        // Get visible clues in the current room
        public List<Clue> GetVisibleCluesInCurrentRoom()
        {
            return Mansion[CurrentRoom].Clues
                .Where(c => !c.IsHidden && !DiscoveredClues.Contains(c))
                .ToList();
        }

        // Discover a clue
        public void DiscoverClue(Clue clue)
        {
            if (!DiscoveredClues.Contains(clue))
            {
                DiscoveredClues.Add(clue);
            }
        }

        // Interview a person
        public void InterviewPerson(Person person)
        {
            Interviewing = person.Name;
            if (!InterviewedPeople.Contains(person.Name))
            {
                InterviewedPeople.Add(person.Name);
                if (person.DialogueState == null)
                {
                    person.DialogueState = new DialogueState(person.PersonalityType);
                }
            }

            TurnCount++;
        }
        public void TerminateInterview(Person person)
        {
            Interviewing = string.Empty;
        }

        public void MoveToRoom(string roomName)
        {
            if (Mansion.ContainsKey(roomName) &&
                Mansion[CurrentRoom].Connections.ContainsValue(roomName))
            {
                CurrentRoom = roomName;
                TurnCount++;
            }
        }

    }
}
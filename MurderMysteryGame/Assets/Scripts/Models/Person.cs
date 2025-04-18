using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Dialogue;
using MurderMystery.Enums;

namespace MurderMystery.Models
{
    public class Person
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string FunFact { get; set; }
        public List<Relationship> Relationships { get; set; }
        public bool IsVictim { get; set; }
        public string CurrentRoom { get; set; }
        public PersonalityType PersonalityType { get; set; }
        public Footwear Footwear { get; set; }
        public Occupation Occupation { get; set; }
        public int Fondness { get; set; } = 50;
        public List<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
        public List<string> Secrets { get; set; } = new List<string>();
        public List<Clue> KnownClues { get; set; } = new List<Clue>();
        public DialogueManager Dialogue { get; set; }

        public string GenerateStatement(DialogueOption question)
        {
            if (question != null && !string.IsNullOrEmpty(question.Tone))
            {
                if (question.Tone == PersonalityType.ToString())
                {
                    Fondness = Math.Min(100, Fondness + 20);
                }
                else
                {
                    Fondness = Math.Max(0, Fondness - 20);
                }
            }

            string nextNodeID = null;
            if (question != null)
            {
                if (!string.IsNullOrEmpty(question.NextNodeID))
                {
                    nextNodeID = question.NextNodeID;
                }
                else if (question.Variations != null && question.Variations.Any() &&
                         !string.IsNullOrEmpty(question.Variations.First().NextNodeID))
                {
                    nextNodeID = question.Variations.First().NextNodeID;
                }
            }

            try
            {
                return Dialogue.GetNPCTextResponse(nextNodeID, Fondness);
            }
            catch (Exception ex)
            {
                return "I'm sorry, I don't have anything to say about that.";
            }
        }
    }
}
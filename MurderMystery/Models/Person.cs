using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Dialogue;
using MurderMystery.Enums;
using MurderMystery.Generators;

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
        public int Fondness { get; set; } = 50; // how much they like the player (1-100);
        public List<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
        public List<string> Secrets { get; set; } = new List<string>();
        public DialogueManager Dialogue { get; set; }
        public string GenerateStatement(DialogueOption question)
        {
            var nextNode = question?.NextNodeID ?? question?.Variations?.FirstOrDefault().NextNodeID;
            if (question != null && question.Tone != null)
            {
                if (question.Tone == PersonalityType.ToString())
                {
                    Fondness += 20;
                }
                else
                {
                    Fondness -= 20;
                }
            }
                var test = Dialogue.GetCurrentNode();
            return Dialogue.GetNPCTextResponse(nextNode, Fondness);

        }
    }

}

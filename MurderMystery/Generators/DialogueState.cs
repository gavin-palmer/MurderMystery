using System;
using System.Collections.Generic;
using System.Text;
using MurderMystery.Enums;
using MurderMystery.Models;

namespace MurderMystery.Generators
{
    public class DialogueState
    {
        private readonly PersonalityType _personalityType;
        public DialogueState(PersonalityType personalityType)
        {
            _personalityType = personalityType;
        }
        public string NPCIntroduction()
        {
            switch (_personalityType)
            {
                case PersonalityType.Nervous:
                    return "Oh! H-hello there... I wasn't expecting to talk to anyone tonight. Is... is everything okay? You're not here about the... um... nevermind.";

                case PersonalityType.Suspicious:
                    return "Hello... Who sent you? I don't recall seeing you around before. Are you taking notes on what everyone's saying?";

                case PersonalityType.Arrogant:
                    return "Well, I suppose I can spare a moment if you absolutely must speak with me. Though I doubt you'll ask anything I haven't heard before.";

                case PersonalityType.Analytical:
                    return "Hello. Based on your approach and attire, I presume you're investigating the incident that occurred at approximately 9:42 PM in the east wing.";

                case PersonalityType.Sensitive:
                    return "Hello... It's just awful what happened, isn't it? Everyone seems so shaken up. How are you holding up through all of this?";

                case PersonalityType.Manipulative:
                    return "Well hello there! What a pleasure to meet someone new during such unfortunate circumstances. I've been hoping to speak with someone who might understand my perspective on tonight's events.";

                case PersonalityType.Defensive:
                    return "Hello. I've already told the others everything I know, which isn't much. I was nowhere near that part of the house.";

                case PersonalityType.ChattyGossipy:
                    return "Hello! Oh my goodness, can you believe what's happened? Everyone's talking about it! Did you see how pale Mrs. Whitmore looked? I heard from the butler that she and the victim had quite the argument last week over—oh, but you probably want to ask me something specific?";

                case PersonalityType.FormalReserved:
                    return "Good evening. I presume you wish to speak with me regarding the unfortunate events of this evening. I shall endeavor to be of assistance.";

                case PersonalityType.FlirtatousCharming:
                    return "Well hello there, detective. What a delightful surprise to be interviewed by someone with such... attentive eyes. Perhaps we could discuss this somewhere more comfortable?";

                default:
                    return "Hello there. What can I help you with?";
            }
        }

        public List<string> GeneratePlayerDialogueOptions(string npcDialogue)
        {
            var options = new List<string>();
            if (string.IsNullOrEmpty(npcDialogue))
            {
            }

            return options;
        }
    }
}

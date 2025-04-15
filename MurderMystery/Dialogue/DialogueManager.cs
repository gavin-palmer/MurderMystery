using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MurderMystery.Enums;
using MurderMysteryGame.Dialogue;
using Newtonsoft.Json;

namespace MurderMystery.Dialogue
{
    public class DialogueManager
    {
        private DialogueRoot _dialogueRoot;
        private string _currentNodeId = "intro";
        private DialogueNode _conversationNode; 
        private readonly PersonalityType _personalityType;
        public DialogueManager(PersonalityType personality)
        {

            _personalityType = personality;
             var dialogue = System.IO.File.ReadAllText("Dialogue.json");
            _dialogueRoot = JsonConvert.DeserializeObject<DialogueRoot>(dialogue);
            _conversationNode = GetDialogueNode("suspect_standard");

        }

        public string GetNPCTextResponse(string nodeId, int fondness)
        {
            if (!string.IsNullOrEmpty(nodeId))
            {
                _currentNodeId = nodeId;
            }
            var node = GetNodeByID(_currentNodeId);
            var personalityResponse = GetNpcOptionsByPersonality(node, _personalityType);
            var response = string.Empty;
            if (personalityResponse == null) 
            {
                throw new Exception("There isn't a response!!");
            }
            if (personalityResponse.Options != null && personalityResponse.Options.Any())
            {
                foreach(var option in personalityResponse.Options )
                {
                    if(option.Fondness <= fondness)
                    {
                        response = option.Text;
                    }
                }
            } 
            else
            {
                response = personalityResponse.Text;
            }
                return response;
        }
        public List<DialogueOption> GetPlayerTextOptions()
        {
            Random random = new Random();

            var node = GetNodeByID(_currentNodeId);
            var responses = GetRandomPlayerOptions(node);
            var correctResponse = GetPlayerOptionsByPersonality(node, _personalityType);
            responses.Add(correctResponse);
            _currentNodeId = correctResponse.NextNodeID ?? correctResponse.Variations.FirstOrDefault().NextNodeID;
            return responses.OrderBy(x => random.Next()).ToList();
        }

        public DialogueNode GetDialogueNode(string templateName)
        {
            if (_dialogueRoot.DialogueTemplates.ContainsKey(templateName))
            {
                return _dialogueRoot.DialogueTemplates[templateName];
            }
            return null;
        }

        public DialogueNode GetCurrentNode()
        {
            return GetNodeByID(_currentNodeId);
        }
        public DialogueNode GetNodeByID(string nodeID)
        {
            foreach (var template in _dialogueRoot.DialogueTemplates.Values)
            {
                if (template.NodeID == nodeID)
                {
                    return template;
                }
            }
            return null;
        }
        public List<DialogueOption> GetRandomPlayerOptions(DialogueNode node)
        {
            var variations = new List<DialogueOption>();
            var personalityTypes = Enum.GetValues(typeof(PersonalityType))
                 .Cast<PersonalityType>()
                 .ToList();
            var otherPeronalityTypes = personalityTypes.Where(x => x != _personalityType).OrderBy(_ => Guid.NewGuid()).Take(3).ToList();
            foreach (var personalityType in otherPeronalityTypes)
            {
                var variation = GetPlayerOptionsByPersonality(node, personalityType);
                variations.Add(variation);
            }
            return variations;

        }

        public DialogueOption GetPlayerOptionsByPersonality(DialogueNode node, PersonalityType personalityType)
        {

            foreach (var variation in node.PlayerOptions)
            {
                if (variation.Tone == personalityType.ToString())
                {
                    if(variation.Variations != null && variation.Variations.Any())
                    {
                        variation.Text = RandomHelper.PickRandom(variation.Variations.FirstOrDefault().TextOptions);
                    }
                    return variation;
                }
            }
            return node.PlayerOptions.Count > 0 ? node.PlayerOptions[0] : null;
        }
        public DialogueVariation GetNpcOptionsByPersonality(DialogueNode node, PersonalityType personalityType)
        {
            foreach (var variation in node.NpcOptions)
            {
                if (variation.Personality == personalityType.ToString())
                {
                    return variation;
                }
            }

            foreach (var variation in node.NpcOptions)
            {
                if (variation.Personality == "Default")
                {
                    return variation;
                }
            }

            return node.NpcOptions.Count > 0 ? node.NpcOptions[0] : null;
        }
        public ResponseOption GetResponseOption(DialogueVariation variation, int fondnessScore)
        {
            if (variation.Options == null || variation.Options.Count == 0)
            {
                return null;
            }

            ResponseOption bestMatch = variation.Options[0];
            foreach (var option in variation.Options)
            {
                if (option.Fondness <= fondnessScore && option.Fondness > bestMatch.Fondness)
                {
                    bestMatch = option;
                }
            }

            return bestMatch;
        }
    }
}

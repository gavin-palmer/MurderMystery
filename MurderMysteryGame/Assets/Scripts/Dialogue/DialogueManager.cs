

using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Enums;

namespace MurderMystery.Dialogue
{
    public class DialogueManager
    {
        private DialogueRoot _dialogueRoot;
        private string _currentNodeId = "intro";
        private readonly PersonalityType _personalityType;

        public DialogueManager(PersonalityType personality)
        {
            _personalityType = personality;
            var dialogue = System.IO.File.ReadAllText("Dialogue.json");
            _dialogueRoot = JsonConvert.DeserializeObject<DialogueRoot>(dialogue);
        }

        public string GetNPCTextResponse(string nodeId, int fondness)
        {
            if (!string.IsNullOrEmpty(nodeId))
            {
                _currentNodeId = nodeId;
            }

            var node = GetNodeByID(_currentNodeId);
            if (node == null)
            {
                return "I'm not sure what to say about that.";
            }

            var personalityResponse = GetNpcOptionsByPersonality(node, _personalityType);
            if (personalityResponse == null)
            {
                personalityResponse = GetNpcOptionsByPersonality(node, PersonalityType.Analytical);
                if (personalityResponse == null && node.NpcOptions.Count > 0)
                {
                    personalityResponse = node.NpcOptions[0];
                }

                if (personalityResponse == null)
                {
                    return "I'm not sure what to say about that.";
                }
            }

            string response = string.Empty;
            if (personalityResponse.Options != null && personalityResponse.Options.Any())
            {
                ResponseOption bestMatch = null;
                foreach (var option in personalityResponse.Options)
                {
                    if (option.Fondness <= fondness && (bestMatch == null || option.Fondness > bestMatch.Fondness))
                    {
                        bestMatch = option;
                    }
                }

                if (bestMatch != null)
                {
                    response = bestMatch.Text;

                    if (!string.IsNullOrEmpty(bestMatch.NextNodeID))
                    {
                        _currentNodeId = bestMatch.NextNodeID;
                    }
                }
                else
                {
                    response = personalityResponse.Options.FirstOrDefault()?.Text ??
                               "I don't have anything more to say about that.";
                }
            }
            else if (!string.IsNullOrEmpty(personalityResponse.Text))
            {
                response = personalityResponse.Text;
            }
            else
            {
                response = "I'm not sure what to say about that.";
            }

            return response;
        }

        public List<DialogueOption> GetPlayerTextOptions()
        {
            Random random = new Random();

            var node = GetNodeByID(_currentNodeId);
            if (node == null || node.PlayerOptions == null || !node.PlayerOptions.Any())
            {
                return new List<DialogueOption>
                {
                    new DialogueOption { Text = "I should go now.", NextNodeID = "exit" }
                };
            }

            var responses = GetRandomPlayerOptions(node);
            var correctResponse = GetPlayerOptionsByPersonality(node, _personalityType);

            if (correctResponse != null)
            {
                responses.Add(correctResponse);
            }

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
            if (string.IsNullOrEmpty(nodeID))
            {
                return null;
            }

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

            if (node == null || node.PlayerOptions == null || !node.PlayerOptions.Any())
            {
                return variations;
            }

            var personalityTypes = Enum.GetValues(typeof(PersonalityType))
                 .Cast<PersonalityType>()
                 .ToList();

            var otherPersonalityTypes = personalityTypes
                .Where(x => x != _personalityType)
                .OrderBy(_ => Guid.NewGuid())
                .Take(3)
                .ToList();

            foreach (var personalityType in otherPersonalityTypes)
            {
                var variation = GetPlayerOptionsByPersonality(node, personalityType);
                if (variation != null)
                {
                    if (string.IsNullOrEmpty(variation.Text))
                    {
                        if (variation.Variations != null && variation.Variations.Any() &&
                            variation.Variations.First().TextOptions != null &&
                            variation.Variations.First().TextOptions.Any())
                        {
                            variation.Text = RandomHelper.PickRandom(variation.Variations.First().TextOptions);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    variations.Add(variation);
                }
            }

            while (variations.Count < 3 && node.PlayerOptions.Count > 0)
            {
                var randomOption = node.PlayerOptions[new Random().Next(node.PlayerOptions.Count)].Clone();

                if (string.IsNullOrEmpty(randomOption.Text) && randomOption.Variations != null &&
                    randomOption.Variations.Any() && randomOption.Variations.First().TextOptions != null &&
                    randomOption.Variations.First().TextOptions.Any())
                {
                    randomOption.Text = RandomHelper.PickRandom(randomOption.Variations.First().TextOptions);
                }

                if (!string.IsNullOrEmpty(randomOption.Text))
                {
                    variations.Add(randomOption);
                }
            }

            return variations;
        }

        public DialogueOption GetPlayerOptionsByPersonality(DialogueNode node, PersonalityType personalityType)
        {
            if (node == null || node.PlayerOptions == null)
            {
                return null;
            }

            DialogueOption matchingOption = null;
            foreach (var option in node.PlayerOptions)
            {
                if (option.Tone == personalityType.ToString())
                {
                    matchingOption = option.Clone();
                    break;
                }
            }

            if (matchingOption != null && matchingOption.Variations != null && matchingOption.Variations.Any())
            {
                var variation = matchingOption.Variations.FirstOrDefault();
                if (variation != null && variation.TextOptions != null && variation.TextOptions.Any())
                {
                    matchingOption.Text = RandomHelper.PickRandom(variation.TextOptions);
                }
            }

            return matchingOption ?? (node.PlayerOptions.Count > 0 ? node.PlayerOptions[0].Clone() : null);
        }

        public DialogueVariation GetNpcOptionsByPersonality(DialogueNode node, PersonalityType personalityType)
        {
            if (node == null || node.NpcOptions == null)
            {
                return null;
            }

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
            if (variation == null || variation.Options == null || variation.Options.Count == 0)
            {
                return null;
            }

            ResponseOption bestMatch = null;
            foreach (var option in variation.Options)
            {
                if (option.Fondness <= fondnessScore && (bestMatch == null || option.Fondness > bestMatch.Fondness))
                {
                    bestMatch = option;
                }
            }

            return bestMatch ?? variation.Options.OrderBy(o => o.Fondness).FirstOrDefault();
        }
    }

    public static class DialogueExtensions
    {
        public static DialogueOption Clone(this DialogueOption original)
        {
            if (original == null) return null;

            return new DialogueOption
            {
                Text = original.Text,
                Tone = original.Tone,
                NextNodeID = original.NextNodeID,
                Variations = original.Variations
            };
        }
    }
}
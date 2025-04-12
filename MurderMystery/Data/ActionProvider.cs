using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Models;
using MurderMystery.Enums;
using Action = MurderMystery.Models.Action;

namespace MurderMystery.Data.Providers
{
    public class ActionProvider : BaseDataProvider<Action>
    {
        protected override List<Action> LoadItems()
        {
            return new List<Action>
            {
                // Solo actions
                new Action("was reading a book"),
                new Action("was having a drink"),
                new Action("was examining a painting"),
                new Action("was looking out the window"),
                new Action("was writing a letter"),
                new Action("was playing cards"),
                new Action("was pacing nervously"),
                new Action("was admiring the decor"),
                new Action("was checking their watch repeatedly"),
                new Action("was flipping through a magazine"),
                new Action("was cleaning their glasses"),
                new Action("was making notes in a small notebook"),
                new Action("was adjusting their clothing"),
                new Action("was examining the furniture"),
                new Action("was humming softly to themselves"),
                new Action("was lighting a cigarette"),
                new Action("was gazing thoughtfully at nothing in particular"),
                new Action("was checking their reflection"),
                
                // Social actions
                new Action("was talking with {0}", true),
                new Action("was having a conversation with {0}", true),
                new Action("was whispering to {0}", true),
                new Action("was arguing with {0}", true),
                new Action("was laughing with {0}", true),
                new Action("was discussing recent events with {0}", true),
                new Action("was sharing a drink with {0}", true),
                new Action("was trading gossip with {0}", true),
                new Action("was debating politics with {0}", true)
            };
        }

        /// <summary>
        /// Gets a random solo action (one that doesn't require another person)
        /// </summary>
        public Action GetRandomSoloAction()
        {
            var soloActions = GetAll().Where(a => !a.RequiresOtherPerson).ToList();
            return soloActions[_random.Next(soloActions.Count)];
        }

        /// <summary>
        /// Gets a random social action (one that involves another person)
        /// </summary>
        public Action GetRandomSocialAction()
        {
            var socialActions = GetAll().Where(a => a.RequiresOtherPerson).ToList();
            return socialActions[_random.Next(socialActions.Count)];
        }

        /// <summary>
        /// Gets an action appropriate for a specific relationship type
        /// </summary>
        public Action GetActionForRelationship(RelationshipType relationshipType)
        {
            // First try to find a relationship-specific action
            var relationshipActions = new List<Action>();

            switch (relationshipType)
            {
                case RelationshipType.Friend:
                    relationshipActions = new List<Action>
                    {
                        new Action("was chatting amicably with {0}", true),
                        new Action("was joking with {0}", true),
                        new Action("was reminiscing with {0}", true)
                    };
                    break;

                case RelationshipType.Enemy:
                    relationshipActions = new List<Action>
                    {
                        new Action("was arguing with {0}", true),
                        new Action("was glaring at {0}", true),
                        new Action("was making veiled threats to {0}", true)
                    };
                    break;

                case RelationshipType.Lover:
                    relationshipActions = new List<Action>
                    {
                        new Action("was having an intimate conversation with {0}", true),
                        new Action("was whispering sweet nothings to {0}", true),
                        new Action("was holding hands with {0}", true)
                    };
                    break;

                case RelationshipType.Spouse:
                    relationshipActions = new List<Action>
                    {
                        new Action("was discussing household matters with {0}", true),
                        new Action("was planning a holiday with {0}", true),
                        new Action("was sorting through finances with {0}", true)
                    };
                    break;
            }

            // If we have relationship-specific actions, use those
            if (relationshipActions.Count > 0)
            {
                return relationshipActions[_random.Next(relationshipActions.Count)];
            }

            // Otherwise fall back to a regular social action
            return GetRandomSocialAction();
        }
    }
}
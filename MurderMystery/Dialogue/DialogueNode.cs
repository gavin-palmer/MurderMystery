using System;
using System.Collections.Generic;
using System.Text;
using MurderMysteryGame.Dialogue;
using Newtonsoft.Json;

namespace MurderMystery.Dialogue
{
    public class DialogueNode
    {
        [JsonProperty("nodeID")]
        public string NodeID { get; set; }

        [JsonProperty("npcOptions")]
        public List<DialogueVariation> NpcOptions { get; set; }

        [JsonProperty("playerOptions", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogueOption> PlayerOptions { get; set; }
    }
}

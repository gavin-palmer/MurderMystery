using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DialogueNode
{
    [JsonProperty("nodeID")]
    public string NodeID { get; set; }

    [JsonProperty("npcOptions")]
    public List<DialogueVariation> NpcOptions { get; set; }

    [JsonProperty("playerOptions", NullValueHandling = NullValueHandling.Ignore)]
    public List<DialogueOption> PlayerOptions { get; set; }
}
 
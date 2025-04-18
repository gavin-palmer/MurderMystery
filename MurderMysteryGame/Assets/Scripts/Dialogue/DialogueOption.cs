using System;
using System.Collections.Generic;

public class DialogueOption
{
    [JsonProperty("tone", NullValueHandling = NullValueHandling.Ignore)]
    public string Tone { get; set; }

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    [JsonProperty("nextNodeID", NullValueHandling = NullValueHandling.Ignore)]
    public string NextNodeID { get; set; }

    [JsonProperty("variations", NullValueHandling = NullValueHandling.Ignore)]
    public List<TextVariation> Variations { get; set; }
}
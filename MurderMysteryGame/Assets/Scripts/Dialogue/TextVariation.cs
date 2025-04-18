using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TextVariation
{
    [JsonProperty("textOptions")]
    public List<string> TextOptions { get; set; }

    [JsonProperty("nextNodeID")]
    public string NextNodeID { get; set; }
}

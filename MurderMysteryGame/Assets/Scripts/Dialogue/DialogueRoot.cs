using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DialogueRoot
{
    [JsonProperty("dialogue_templates")]
    public Dictionary<string, DialogueNode> DialogueTemplates { get; set; }
}

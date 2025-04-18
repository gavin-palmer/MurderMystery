using System;
public class DialogueRoot
{
    [JsonProperty("dialogue_templates")]
    public Dictionary<string, DialogueNode> DialogueTemplates { get; set; }
}

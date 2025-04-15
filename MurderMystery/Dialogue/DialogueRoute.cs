using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MurderMystery.Dialogue;
using Newtonsoft.Json;

namespace MurderMysteryGame.Dialogue
{
    public class DialogueRoot
    {
        [JsonProperty("dialogue_templates")]
        public Dictionary<string, DialogueNode> DialogueTemplates { get; set; }
    }









}
using System;
using System.Collections.Generic;
using System.Text;
using MurderMysteryGame.Dialogue;
using Newtonsoft.Json;

namespace MurderMystery.Dialogue
{
    public class DialogueVariation
    {
        [JsonProperty("personality", NullValueHandling = NullValueHandling.Ignore)]
        public string Personality { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<ResponseOption> Options { get; set; }
    }
}

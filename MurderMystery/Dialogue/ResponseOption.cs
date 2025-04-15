using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MurderMystery.Dialogue
{
    public class ResponseOption
    {
        [JsonProperty("fondness")]
        public int Fondness { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("personality", NullValueHandling = NullValueHandling.Ignore)]
        public string Personality { get; set; }

        [JsonProperty("nextNodeID", NullValueHandling = NullValueHandling.Ignore)]
        public string NextNodeID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MurderMystery.Dialogue
{
    public class TextVariation
    {
        [JsonProperty("textOptions")]
        public List<string> TextOptions { get; set; }

        [JsonProperty("nextNodeID")]
        public string NextNodeID { get; set; }
    }
}

using Newtonsoft.Json;

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
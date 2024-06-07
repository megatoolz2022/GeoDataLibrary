using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GeoJSONData
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("features")]
    public List<Feature> Features { get; set; }
}

public class Feature
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("geometry")]
    public Geometry Geometry { get; set; }

    [JsonProperty("properties")]
    public Dictionary<string, object> Properties { get; set; }
}

public class Geometry
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("coordinates")]
    public JToken Coordinates { get; set; }
}

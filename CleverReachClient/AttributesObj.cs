using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;

namespace CleverReachClient
{
    public class AttributesObj
    {
        public int groups_id { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public AttributeType type { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AttributeType
    {
        text,
        number,
        gender,
        date,
    }
}

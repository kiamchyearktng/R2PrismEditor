using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace prismeditor
{
    public class RootDataTable
    {
        //[JsonPropertyName("Name")]
        public string? Name { get; set; }

        //public PrismStoneData[]? Rows;
        //public Dictionary<string, JsonElement>[]? Rows { get; set; }

        public Dictionary<string, JsonObject>? Rows { get; set; }
        //public Dictionary<string, JsonObject> ParsedRows { get; set; }

        //public void Init()
        //{
        //    ParsedRows = new Dictionary<string, JsonObject>();
        //}
    }

    public class PrismStoneData
    {

    }
}

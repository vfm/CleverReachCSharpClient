using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverReachClient
{
    public class ReceiverObj
    {
        public ReceiverObj()
        {
            attributes = new Dictionary<string, string>();
        }

        public string id { get; set; }
        public string email { get; set; }
        public int activated { get; set; }
        public int registered { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }
}

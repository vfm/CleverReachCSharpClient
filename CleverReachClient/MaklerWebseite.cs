using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CleverReachRestClient
{
    public static class MaklerWebseite
    {
        /// <summary>
        /// Gibt zu einer Makler Webseiten URL die Eigenschaften zurück
        /// </summary>
        /// <param name="url">funktioniert mit und ohne / (slash) abschließend</param>
        /// <returns>Dictionary mit den KeyPairValues der einzelnen Eigenschaften</returns>
        public static Dictionary<string, string> GetValues(string url)
        {
            var values = new Dictionary<string, string>();
            using (WebClient wc = new WebClient())
                foreach (JProperty prop in ((JObject)JsonConvert.DeserializeObject(wc.DownloadString(url.TrimEnd(new char[] { '/' }) + "/cleverreach.json"))).Children())
                   values.Add(prop.Name, prop.Value.ToString());
            return values;
        }
    }
}

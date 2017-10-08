using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace CleverReachClient
{
    public class CRClient
    {
        const string CLEVERREACH_API_URL = "https://rest.cleverreach.com/v2";

        /// <summary>
        /// int = groupID
        /// </summary>
        Dictionary<int, ICollection<AttributesObj>> attributeCache = new Dictionary<int, ICollection<AttributesObj>>();
        HttpClient client;
        string token = null;

        public CRClient()
        {
            client = new HttpClient { BaseAddress = new Uri(CLEVERREACH_API_URL) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        async Task<T> GetAsync<T>(string path)
        {
            T obj = default(T);
            if (!TokenRequired()) return obj;

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                if (result == "false")
                    return obj;
                else
                    obj = await response.Content.ReadAsAsync<T>();
            }
            return obj;
        }

        async Task<string> GetLoginAsync(LoginObj login)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("login", login);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<string>();
            return null;
        }

        async Task<T> PostAsync<T>(string path, T obj)
        {
            if (!TokenRequired()) return default(T);

            StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(path, content);
            if (response.IsSuccessStatusCode)
            {
                var a = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(a);
            }
            return default(T);
        }

        async Task<string> PostAsyncString(string path, object obj)
        {
            if (!TokenRequired()) return null;

            StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(path, content);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            return null;
        }

        async Task<ICollection<AttributesObj>> QueryAttributesFromGroup(GroupObj group)
        {
            return await GetAsync<ICollection<AttributesObj>>($"/attributes?group_id={group.id}");
        }

        bool TokenRequired()
        {
            if (token == null) return false;

            if (!client.DefaultRequestHeaders.TryGetValues("Authorization", out var tmp))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        /// <summary>
        /// Fügt ein nicht vorhandenes Attribute in eine Gruppe hinzu
        /// </summary>
        /// <param name="group">Gruppen Object</param>
        /// <param name="attribName">Attributsname</param>
        /// <param name="typ">Typ des Attributes (Text, Datum etc.)</param>
        /// <returns></returns>
        public async Task AddAttributeInGroup(GroupObj group, string attribName, AttributeType typ)
        {
            ICollection<AttributesObj> attribs = null;
            if (!attributeCache.TryGetValue(group.id, out var trash))
                attributeCache.Add(group.id, await QueryAttributesFromGroup(group));

            if (attribs == null)
                attributeCache.TryGetValue(group.id, out attribs);

            var attrib = attribs == null ? null : attribs.SingleOrDefault(a => a.name == attribName); // ATTRIBUTE SUCHEN
            if (attrib == null) // WENNS DAS ATTRIBUTE NOCH NICHT GIBT
                attrib = await PostAsync<AttributesObj>("/attributes", new AttributesObj() { name = attribName, type = typ, groups_id = group.id }); // GRUPPE ANLEGEN
        }

        /// <summary>
        /// Fügt einer bestehenden Gruppe einen Empfänger hinzu. Beliebige Attribute können verwendet werden
        /// </summary>
        /// <param name="group">Gruppe</param>
        /// <param name="mail">E-Mail Adresse</param>
        /// <param name="attribute">weitere Attribute des Empfängers</param>
        /// <returns></returns>
        public async Task AddReceiver(GroupObj group, string mail, Dictionary<string, string> attribute)
        {
            var rec = new ReceiverObj();
            rec.email = mail;
            rec.attributes = attribute;

            var receiver = await PostAsyncString($"/groups/{group.id}/receivers/upsert", rec); // EMPFÄNGER ANLEGEN
        }

        /// <summary>
        /// Erstellt eine Gruppe, falls noch keine Gruppe mit dem Namen existiert
        /// </summary>
        /// <param name="grpName">Gruppenname</param>
        /// <returns></returns>
        public async Task<GroupObj> CreateGroup(string grpName)
        {
            var groups = await GetAsync<ICollection<GroupObj>>("/groups"); // ALLE GROUPS LESEN
            var group = groups.SingleOrDefault(grp => grp.name == grpName); // GRUPPE SUCHEN
            if (group == null) // WENNS DIE GRUPPE NOCH NICHT GIBT
                group = await PostAsync<GroupObj>("/groups", new GroupObj() { name = grpName }); // GRUPPE ANLEGEN
            return group;
        }

        public async Task LoginAsync(int client_id, string login, string password)
        {
            var Login = new LoginObj() { client_id = client_id, login = login, password = password };
            token = await GetLoginAsync(Login);
            if (token == null)
                throw new AuthenticationException("CleverReach Zugangsdaten sind ungültig!");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleverReachClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private async static Task RunAsync()
        {
            CRClient crclient = new CRClient(99999999, "username", "pa55w0rd");
            GroupObj group = await crclient.CreateGroup("A_Group_Name_1");
            
            Console.WriteLine($"ID der Gruppe: {group.id}");
            
            Console.WriteLine($"Attribute anlegen:");
            for (int attrib_int = 1; attrib_int <= 40; attrib_int++)
            {
                await crclient.AddAttributeInGroup(group, $"attribute_{attrib_int}", AttributeType.text); // FAKE GENERIERUNG VON x ATTRIBUTEN
                Console.Write(".");
            }
            Console.WriteLine($" fertig");

            Console.WriteLine($"Empfänger anlegen:");
            for (int i = 0; i < 100; i++)
            {
                var mail = $"{Guid.NewGuid():n}@domain.com"; // FAKE email zum test generieren
                var attribute = new Dictionary<string, string>();
                for (int attrib_int = 1; attrib_int <= 40; attrib_int++)
                    attribute.Add($"attribute_{attrib_int}", $"{Guid.NewGuid():n}");// FAKE INHALT VON x ATTRIBUTEN

                await crclient.AddReceiver(group, mail, attribute);
                Console.Write(".");
            }
            Console.WriteLine($" fertig");

            Console.ReadLine();
        }
    }
}

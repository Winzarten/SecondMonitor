using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.R3EConnector
{
    public class R3RDatabase
    {
        private Dictionary<int, string> carNames;
        public R3RDatabase()
        {
            carNames = new Dictionary<int, string>();
        }
        public void Load()
        {
            //JsonTextReader reader = new JsonTextReader(File.OpenText("data.json"));
            var jsonDb = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("data.json"));
            var cars = jsonDb.GetValue("cars");
            LoadCarNames(cars);
            
            /*while (reader.Read())
            {
                if (reader.Value != null)
                {
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                }
                else
                {
                    Console.WriteLine("Token: {0}", reader.TokenType);
                }
            }*/
        }

        private void LoadCarNames(JToken carsJson)
        {
            carNames.Clear();
            foreach(var carJson in carsJson)
            {
                var carDefinition = carJson.First;
                int id = carDefinition.Value<int>("Id");
                string name = carDefinition.Value<string>("Name");
                carNames[id] = name;
            }
        }

        public string GetCarName(int id)
        {
            if (!carNames.ContainsKey(id))
                carNames[id] = "Unknown";
            return carNames[id];
            
        }
    }
}


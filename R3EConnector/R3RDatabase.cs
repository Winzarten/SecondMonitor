using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.R3EConnector
{
    public class R3RDatabase
    {
        private Dictionary<int, string> _carNames;
        public R3RDatabase()
        {
            _carNames = new Dictionary<int, string>();
        }
        public void Load()
        {
            //JsonTextReader reader = new JsonTextReader(File.OpenText("data.json"));
            var jsonDb = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(Path.Combine(AssemblyDirectory, "data.json")));
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


        private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private void LoadCarNames(JToken carsJson)
        {
            _carNames.Clear();
            foreach(var carJson in carsJson)
            {
                var carDefinition = carJson.First;
                int id = carDefinition.Value<int>("Id");
                string name = carDefinition.Value<string>("Name");
                _carNames[id] = name;
            }
        }

        public string GetCarName(int id)
        {
            if (!_carNames.ContainsKey(id))
                _carNames[id] = "Unknown";
            return _carNames[id];
            
        }
    }
}


namespace SecondMonitor.R3EConnector
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class R3RDatabase
    {
        private readonly Dictionary<int, string> _carNames;

        public R3RDatabase()
        {
            _carNames = new Dictionary<int, string>();
        }

        public void Load()
        {            
            JObject jsonDb = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(Path.Combine(AssemblyDirectory, "data.json")));
            JToken cars = jsonDb.GetValue("cars");
            LoadCarNames(cars);        
        }

        private static string AssemblyDirectory
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
            {
                _carNames[id] = "Unknown";
            }
            return _carNames[id];
        }
    }
}


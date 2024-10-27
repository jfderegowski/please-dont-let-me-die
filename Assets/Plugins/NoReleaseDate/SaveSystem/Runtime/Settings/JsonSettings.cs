using System;
using Newtonsoft.Json;
using SaveSystem.Runtime.DataStructure;

namespace SaveSystem.Runtime.Settings
{
    [Serializable]
    public class JsonSettings
    {
        public TypeNameHandling TypeNameHandling = TypeNameHandling.All;
        public Formatting Formatting = Formatting.Indented;
        
        private static JsonConverter[] _converters = {
            new SaveData.JsonConverter()
        };

        public JsonSerializerSettings JsonSerializerSettings => new() {
            TypeNameHandling = TypeNameHandling,
            Formatting = Formatting,
            Converters = _converters
        };
    }
}
using System.Collections.Generic;
using Plugins.SaveSystem.DataStructure;
using UnityEngine;

namespace SaveSystem.Runtime.DataStructure
{
    public class BaseData<T>
    {
        public Dictionary<SaveKey, T> Data { get; set; }

        public TV GetKey<TV>(SaveKey key, TV defaultValue)
        {
            if (!Data.TryGetValue(key, out var saveValue)) return defaultValue;
            
            if (saveValue is TV tVValue) 
                return tVValue;
            
            return defaultValue;
        }

        public BaseData<T> SetKey(SaveKey key, T value)
        {
            Data[key] = value;

            return this;
        }

        public bool IsKeyExist(SaveKey key) =>
            Data.ContainsKey(key);
        
        public void RemoveKey(SaveKey key) =>
            Data.Remove(key);
        
        public void ClearData() =>
            Data.Clear();
    }
}
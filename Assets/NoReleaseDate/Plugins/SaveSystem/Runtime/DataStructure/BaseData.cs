using System.Collections.Generic;
using System.Linq;
using NoReleaseDate.Common.Runtime.Serializable;

namespace NoReleaseDate.Plugins.SaveSystem.Runtime.DataStructure
{
    public class BaseData<T>
    {
        public Dictionary<SaveKey, T> Data { get; set; }

        public TV GetKey<TV>(string key, TV defaultValue)
        {
            if (SerializableGuid.IsHexString(key))
            {
                var guid = SerializableGuid.FromHexString(key);
                
                return GetKey(guid, defaultValue);
            }
            
            foreach (var (saveKey, value) in Data)
            {
                if (saveKey.StringKey != key) continue;
                
                if (value is TV tVValue) 
                    return tVValue;
            }
            
            return defaultValue;
        }
        
        public TV GetKey<TV>(SerializableGuid key, TV defaultValue)
        {
            foreach (var (saveKey, value) in Data)
            {
                if (saveKey.Key != key) continue;
                
                if (value is TV tVValue) 
                    return tVValue;
            }
            
            return defaultValue;
        }
        
        public TV GetKey<TV>(SaveKey key, TV defaultValue)
        {
            if (!Data.TryGetValue(key, out var saveValue)) return defaultValue;
            
            if (saveValue is TV tVValue) 
                return tVValue;
            
            return defaultValue;
        }

        public BaseData<T> SetKey(string key, T value)
        {
            if (SerializableGuid.IsHexString(key))
            {
                var guid = SerializableGuid.FromHexString(key);
                
                return SetKey(guid, value);
            }
            
            foreach (var kvp in Data.Where(kvp => kvp.Key.StringKey == key))
            {
                Data[kvp.Key] = value;
                
                return this;
            }

            return this;
        }
        
        public BaseData<T> SetKey(SerializableGuid key, T value)
        {
            foreach (var kvp in Data.Where(kvp => kvp.Key.Key == key))
            {
                Data[kvp.Key] = value;
                
                return this;
            }

            return this;
        }
        
        public BaseData<T> SetKey(SaveKey key, T value)
        {
            Data[key] = value;

            return this;
        }

        public bool IsKeyExist(string key)
        {
            if (!SerializableGuid.IsHexString(key)) 
                return Data.Any(kvp => kvp.Key.StringKey == key);
            
            var guid = SerializableGuid.FromHexString(key);
                
            return IsKeyExist(guid);
        }
        
        public bool IsKeyExist(SerializableGuid key) => Data.Any(kvp => kvp.Key.Key == key);
        
        public bool IsKeyExist(SaveKey key) => Data.ContainsKey(key);
        
        public void RemoveKey(string key)
        {
            if (!SerializableGuid.IsHexString(key))
            {
                foreach (var kvp in Data.Where(kvp => kvp.Key.StringKey == key))
                {
                    Data.Remove(kvp.Key);
                    
                    return;
                }
            }
            else
            {
                var guid = SerializableGuid.FromHexString(key);
                
                RemoveKey(guid);
            }
        }
        
        public void RemoveKey(SerializableGuid key)
        {
            foreach (var kvp in Data.Where(kvp => kvp.Key.Key == key))
            {
                Data.Remove(kvp.Key);
                
                return;
            }
        }
        
        public void RemoveKey(SaveKey key) => Data.Remove(key);
        
        public void ClearData() => Data.Clear();
    }
}
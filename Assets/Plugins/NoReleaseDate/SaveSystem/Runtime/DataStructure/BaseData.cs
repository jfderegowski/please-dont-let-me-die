using System.Collections.Generic;
using Plugins.SaveSystem.DataStructure;
using UnityEngine;

namespace SaveSystem.Runtime.DataStructure
{
    public class BaseData<T>
    {
        public Dictionary<SaveKey, T> Data { get; set; }

        public BaseData() => Data = new Dictionary<SaveKey, T>();

        public BaseData(BaseData<T> baseData) => Data = baseData.Data;

        public BaseData(Dictionary<SaveKey, T> data)
        {
            Data = data;
        }

        public TV GetKey<TV>(SaveKey key, TV defaultValue)
        {
            if (!Data.TryGetValue(key, out var saveValue)) return defaultValue;
            
            if (saveValue is TV tVValue) 
                return tVValue;
            
            return defaultValue;
        }

        public BaseData<T> SetKey(SaveKey key, T value, string comment = "")
        {
            Data[key] = value;

            return this;
        }

        public string GetComment(SaveKey key)
        {
            if (Data.TryGetValue(key, out var saveValue))
                return key.Comment;
            
            Debug.LogWarning("[SAVE-MANAGER] Trying to get comment for non-existing key");
            
            return string.Empty;
        }

        public void SetComment(SaveKey key, string comment) 
        {
            if (IsKeyExist(key)) key.Comment.value = comment;
            else Debug.LogWarning("[SAVE-MANAGER] Trying to set comment for non-existing key");
        }

        public bool IsKeyExist(SaveKey key) =>
            Data.ContainsKey(key);
        
        public void RemoveKey(SaveKey key) =>
            Data.Remove(key);
        
        public void ClearData() =>
            Data.Clear();
    }
}
using System;
using NoReleaseDate.Common.Runtime.Properties;
using SaveSystem.Runtime.Serializable;
using UnityEngine;

namespace Plugins.SaveSystem.DataStructure
{
    [Serializable]
    public struct SaveKey : IEquatable<SaveKey>
    {
        [Tooltip("Use this key to have fast compare")] 
        public SerializableGuid Key;
        
        [Tooltip("If the value is not empty, it will be used instead of the SerializableGuid key.")]
        public HasValue<string> StringKey;
        
        [Tooltip("Comment for the key. If its empty it will no serialize in file")] 
        public HasValue<string> Comment;
        
        public static SaveKey RandomKey => new(SerializableGuid.NewGuid());

        public SaveKey(SerializableGuid key, string comment = "")
        {
            Key = key;
            StringKey = new HasValue<string>(string.Empty);
            Comment = new HasValue<string>(comment, !string.IsNullOrEmpty(comment));
        }
        
        public SaveKey(string key, string comment = "")
        {
            Key = SerializableGuid.Empty;
            StringKey = new HasValue<string>(key, true);
            Comment = new HasValue<string>(comment, !string.IsNullOrEmpty(comment));
        }

        public SaveKey(SaveKey saveKey)
        {
            Key = saveKey.Key;
            StringKey = saveKey.StringKey;
            Comment = saveKey.Comment;
        }

        public SaveKey WithKey(SerializableGuid key)
        {
            Key = key;
            return this;
        }

        public SaveKey WithComment(string comment)
        {
            Comment = new HasValue<string>(comment, !string.IsNullOrEmpty(comment));
            return this;
        }

        public bool Equals(SaveKey other)
        {
            if (StringKey.hasValue && other.StringKey.hasValue)
                return StringKey == other.StringKey;
            
            return Key == other.Key; 
        }

        public override bool Equals(object obj) => obj is SaveKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Key);

        public static bool operator ==(SaveKey left, SaveKey right) => left.Equals(right);

        public static bool operator !=(SaveKey left, SaveKey right) => !(left == right);
    }
}

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoReleaseDate.Common.Runtime.Properties
{
    [Serializable]
    public class HasValue<T>
    {
        public T Value;
        public bool hasValue;

        public HasValue(T value, bool hasValue = false)
        {
            Value = value;
            this.hasValue = hasValue;
        }

        public static implicit operator T(HasValue<T> hasValue)
        {
            if (!hasValue.hasValue)
                Debug.LogWarning("You using the value when the 'hasValue' is set to false");

            return hasValue.Value;
        }
    }
}
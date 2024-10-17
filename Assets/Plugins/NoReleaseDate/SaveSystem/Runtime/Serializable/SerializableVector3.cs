using System;
using UnityEngine;

namespace SaveSystem.Runtime.Serializable
{
    /// <summary>
    /// Represents a serializable version of the Unity Vector3 struct.
    /// </summary>
    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    
        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public override string ToString() => $"[{x}, {y}, {z}]";

        public static implicit operator Vector3(SerializableVector3 vector) => 
            new(vector.x, vector.y, vector.z);

        public static implicit operator SerializableVector3(Vector3 vector) => 
            new(vector.x, vector.y, vector.z);
    }
    
    public struct SerializableVector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableVector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public SerializableVector4(Vector4 vector4)
        {
            x = vector4.x;
            y = vector4.y;
            z = vector4.z;
            w = vector4.w;
        }

        public override string ToString() =>
            $"[{x}, {y}, {z}, {w}]";

        public static implicit operator Vector4(SerializableVector4 serializableVector4) => 
            new(serializableVector4.x, serializableVector4.y, serializableVector4.z, serializableVector4.w);

        public static implicit operator SerializableVector4(Vector4 vector4) => 
            new(vector4.x, vector4.y, vector4.z, vector4.w);
    }
}

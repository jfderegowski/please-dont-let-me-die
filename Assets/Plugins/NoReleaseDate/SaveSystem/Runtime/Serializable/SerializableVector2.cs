using UnityEngine;

namespace SaveSystem.Runtime.Serializable
{
    public struct SerializableVector2
    {
        public float x;
        public float y;

        public SerializableVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    
        public SerializableVector2(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }

        public override string ToString() => $"[{x}, {y}]";

        public static implicit operator Vector2(SerializableVector2 vector) => 
            new(vector.x, vector.y);

        public static implicit operator SerializableVector2(Vector2 vector) => 
            new(vector.x, vector.y);
    }
}
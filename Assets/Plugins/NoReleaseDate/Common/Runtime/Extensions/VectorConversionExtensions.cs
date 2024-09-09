using System.Runtime.CompilerServices;
using UnityEngine;

namespace NoReleaseDate.Common.Runtime.Extensions
{
    public static class VectorConversionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToUnityVector(this System.Numerics.Vector2 vector) => new(vector.X, vector.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector2 ToSystemVector(this Vector2 vector) => new(vector.x, vector.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToUnityVector(this System.Numerics.Vector3 vector) => new(vector.X, vector.Y, vector.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 ToSystemVector(this Vector3 vector) => new(vector.x, vector.y, vector.z);
    }
}
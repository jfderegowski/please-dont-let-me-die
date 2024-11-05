using System.Runtime.CompilerServices;
using UnityEngine;

namespace NoReleaseDate.Common.Runtime.Extensions
{
    public static class QuaternionConversionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ToUnityQuaternion(this System.Numerics.Quaternion quaternion) => 
            new(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Quaternion ToSystemQuaternion(this Quaternion quaternion) => 
            new(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
}
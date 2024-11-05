using System;
using UnityEngine;

namespace NoReleaseDate.Plugins.DetectorSystem
{
    [Serializable]
    public struct DetectedArgs<T>
    {
        public T detected;
        public RaycastHit hit;
        
        public DetectedArgs(T detected, RaycastHit hit)
        {
            this.detected = detected;
            this.hit = hit;
        }
    }
}
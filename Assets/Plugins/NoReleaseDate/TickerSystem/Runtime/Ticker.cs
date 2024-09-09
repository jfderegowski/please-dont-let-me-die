using System;
using System.Collections.Generic;
using NoReleaseDate.SingletonSystem.Runtime;
using UnityEngine;

namespace NoReleaseDate.TickerSystem.Runtime
{
    /// <summary>
    /// A class to handle tick actions
    /// </summary>
    public class Ticker : Singleton<Ticker>
    {
        private readonly List<TickerEntry> _entries = new();

        private void Update()
        {
            var currentTime = Time.time;

            foreach (var entry in _entries)
            {
                if (!(currentTime >= entry.nextTick)) continue;
                
                var deltaTime = currentTime - entry.lastTickTime;
                entry.tickAction?.Invoke(deltaTime);
                entry.lastTickTime = currentTime;
                entry.nextTick = currentTime + entry.interval;
            }
        }

        /// <summary>
        /// Register a tick action with an interval
        /// </summary>
        /// <param name="tickAction"> The action to be called every interval </param>
        /// <param name="interval"> The interval between each tick in seconds </param>
        public void Register(Action<float> tickAction, float interval) => 
            _entries.Add(new TickerEntry(tickAction, interval, Time.time + interval, Time.time));

        /// <summary>
        /// Unregister a tick action
        /// </summary>
        /// <param name="tickAction"> The action to be unregistered </param>
        public void Unregister(Action<float> tickAction) => 
            _entries.RemoveAll(entry => entry.tickAction == tickAction);

        /// <summary>
        /// Reset the interval of a tick action
        /// </summary>
        /// <param name="tickAction"> The action to be reset </param>
        /// <param name="newInterval"> The new interval between each tick in seconds </param>
        public void ResetTicker(Action<float> tickAction, float newInterval)
        {
            var entry = _entries.Find(entry => entry.tickAction == tickAction);
            
            if (entry == null) return;
            
            entry.interval = newInterval;
            entry.nextTick = Time.time + newInterval;
            entry.lastTickTime = Time.time;
        }

        /// <summary>
        /// A class to hold the tick action and its interval
        /// </summary>
        private class TickerEntry
        {
            /// <summary>
            /// The action to be called every interval
            /// </summary>
            internal readonly Action<float> tickAction;

            /// <summary>
            /// The interval between each tick
            /// </summary>
            internal float interval;

            /// <summary>
            /// The time when the next tick should occur
            /// </summary>
            internal float nextTick;

            /// <summary>
            /// The time when the last tick occurred
            /// </summary>
            internal float lastTickTime;

            /// <summary>
            /// Constructor for the TickerEntry class
            /// </summary>
            /// <param name="tickAction"> The action to be called every interval </param>
            /// <param name="interval"> The interval between each tick </param>
            /// <param name="nextTick"> The time when the next tick should occur </param>
            /// <param name="lastTickTime"> The time when the last tick occurred </param>
            public TickerEntry(Action<float> tickAction, float interval, float nextTick, float lastTickTime)
            {
                this.tickAction = tickAction;
                this.interval = interval;
                this.nextTick = nextTick;
                this.lastTickTime = lastTickTime;
            }
        }
    }
}

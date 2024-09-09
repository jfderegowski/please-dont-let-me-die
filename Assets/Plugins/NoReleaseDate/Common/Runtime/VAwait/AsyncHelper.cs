using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace NoReleaseDate.Common.Runtime.VAwait
{
    public static class Wait
    {
        public static UPlayStateMode playMode { get; set; } = UPlayStateMode.None;
        public static int poolLength { get; set; } = 15;

        private static CancellationTokenSource awaitTokenSource { get; set; }

        public static (VWaitComponent component, GameObject gameObject) runtimeInstance;

        private static ConcurrentQueue<SignalAwaiter> _signalPool;
        private static readonly Dictionary<int, SignalAwaiter> _id = new();
        private static SynchronizationContext _unityContext;

        public static void RemoveID(int id) => _id.Remove(id);

        /// <summary>
        ///Triggers reinitialization.
        /// </summary>
        public static void ReInit() => StartAwait();

        private static void PrepareAsyncHelper()
        {
            _unityContext ??= SynchronizationContext.Current;

            if (awaitTokenSource != null)
            {
                awaitTokenSource.Cancel();
                awaitTokenSource.Dispose();
            }

            if (_signalPool is { Count: > 0 })
            {
                for (var i = 0; i < _signalPool.Count; i++)
                    if (_signalPool.TryDequeue(out var func))
                        func.Cancel();
            }

            awaitTokenSource = new CancellationTokenSource();
            _signalPool = new ConcurrentQueue<SignalAwaiter>();

            for (var i = 0; i < poolLength; i++)
            {
                var ins = new SignalAwaiter(awaitTokenSource);
                _signalPool.Enqueue(ins);
            }
        }

        /// <summary>
        /// Gets an instance of SignalAwaiter from the pool.
        /// </summary>
        private static SignalAwaiter GetPooled() =>
            _signalPool.TryDequeue(out var signalAwaiter) ? signalAwaiter : new SignalAwaiter(awaitTokenSource);

        /// <summary>
        /// Returns back to pool.
        /// </summary>
        /// <param name="signal"></param>
        public static void ReturnAwaiterToPool(SignalAwaiter signal)
        {
            signal.Reset();

            if (_signalPool.Count < poolLength) 
                _signalPool.Enqueue(signal);
        }

        /// <summary>
        /// Pooled awaiter, awaits for next frame. Can't be awaited more than once. This awaiter won't be affected by Time.timeScale, use Wait.Null or Wait.FixedUpdate instead.
        /// </summary>
        public static SignalAwaiter NextFrame()
        {
            var signalAwaiter = GetPooled();

            try
            {
                return signalAwaiter;
            }
            finally
            {
                PlayerLoopUpdate.playerLoopUtil.QueueNextFrame(signalAwaiter);
            }
        }

        /// <summary>
        /// Wait until end of frame. Can't be awaited more than once. This awaiter will not be affected by Time.timeScale. 
        /// </summary>
        public static SignalAwaiter EndOfFrame()
        {
            var signalAwaiter = GetPooled();

            try
            {
                return signalAwaiter;
            }
            finally
            {
                PlayerLoopUpdate.playerLoopUtil.QueueEndOfFrame(signalAwaiter);
            }
        }

        /// <summary>
        /// Reusable awaiter, can be awaited multiple times. This awaiter won't be affected by Time.timeScale, use Wait.Null or Wait.FixedUpdate instead.
        /// </summary>
        public static SignalAwaiterReusable NextFrameReusable() => new(awaitTokenSource);

        /// <summary>
        /// Equals to NextFrame but respects the Time.timeScale.
        /// </summary>
        /// <returns></returns>
        public static SignalAwaiterReusable Null()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                throw new Exception("VAwait Error : NullAlloc can't be used for edit mode.");
#endif

            var signalAwaiterReusable = new SignalAwaiterReusable(awaitTokenSource);

            try
            {
                return signalAwaiterReusable;
            }
            finally
            {
                PlayerLoopUpdate.playerLoopUtil.QueueFixedUpdate(signalAwaiterReusable);
            }
        }

        /// <summary>
        /// This calls Unity's coroutine.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static SignalAwaiter SecondsScaled(float time)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                throw new Exception("VAwait Error : SecondsScaled can't be used for edit mode.");
#endif

            var signalAwaiter = GetPooled();

            try
            {
                return signalAwaiter;
            }
            finally
            {
                runtimeInstance.component.TriggerSecondsCoroutine(signalAwaiter, time);
            }
        }

        /// <summary>
        /// Awaits for the next FixedUpdate.
        /// </summary>
        public static SignalAwaiterReusable FixedUpdate()
        {
            var signalAwaiterReusable = new SignalAwaiterReusable(awaitTokenSource);

            try
            {
                return signalAwaiterReusable;
            }
            finally
            {
                PlayerLoopUpdate.playerLoopUtil.QueueFixedUpdate(signalAwaiterReusable);
            }
        }

        /// <summary>
        /// Fixed 1 frame value meant to be used for frame waiting while in edit-mode. This is not accurate,\njust a very rough estimation based on screen's refresh rate.
        /// </summary>
        /// <returns>Double value.</returns>
        public static double oneFrameFixed
        {
            get
            {
                var refValue = Screen.currentResolution.refreshRateRatio.value;
                return (1d / refValue) * 1000;
            }
        }

        /// <summary>
        /// Waits for n duration in seconds.
        /// </summary>
        /// <param name="duration"></param>
        public static SignalAwaiter Seconds(float duration)
        {
            var signalAwaiter = GetPooled();

            try
            {
                return signalAwaiter;
            }
            finally
            {
                _ = WaitSeconds(duration, signalAwaiter, false);
            }
        }

        /// <summary>
        /// Waits for n duration in seconds.
        /// </summary>
        /// <param name="duration"></param>
        public static SignalAwaiter SecondsRealtime(float duration)
        {
            var signalAwaiter = GetPooled();

            try
            {
                return signalAwaiter;
            }
            finally
            {
                _ = WaitSeconds(duration, signalAwaiter, true);
            }
        }

        /// <summary>
        /// Waits for n duration in seconds.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="setId">Unique id for the awaiter.</param>
        public static SignalAwaiter Seconds(float duration, int setId)
        {
            if (setId < 0)
                throw new Exception("VAwait Error : Id can't be negative number");

            var signalAwaiter = GetPooled();
            signalAwaiter.id = setId;
            _id.TryAdd(setId, signalAwaiter);

            try
            {
                return signalAwaiter;
            }
            finally
            {
                _ = WaitSeconds(duration, signalAwaiter, false);
            }
        }

        /// <summary>
        /// Waits for n duration in unscaledTime in seconds.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="setId">Unique id for the awaiter.</param>
        public static SignalAwaiter SecondsRealtime(float duration, int setId)
        {
            if (setId < 0)
                throw new Exception("VAwait Error : Id can't be negative number");

            var signalAwaiter = GetPooled();
            signalAwaiter.id = setId;
            _id.TryAdd(setId, signalAwaiter);

            try
            {
                return signalAwaiter;
            }
            finally
            {
                _ = WaitSeconds(duration, signalAwaiter, true);
            }
        }

        /// <summary>
        /// Waits for coroutine.
        /// </summary>
        /// <param name="coroutine"></param>
        public static SignalAwaiter Coroutine(IEnumerator coroutine)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                throw new Exception("VAwait Error : Coroutine can't be used for edit mode.");
#endif

            var signalAwaiter = GetPooled();
            signalAwaiter.AssignEnumerator(coroutine);

            try
            {
                return signalAwaiter;
            }
            finally
            {
                runtimeInstance.component.TriggerCoroutine(coroutine, signalAwaiter);
            }
        }

        /// <summary>
        /// Init.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void StartAwait()
        {
            PrepareAsyncHelper();

            if (runtimeInstance.gameObject) return;
            
            var go = new GameObject
            {
                name = "VAwait-instance"
            };
            
            go.AddComponent<VWaitComponent>();
            
            runtimeInstance = (go.GetComponent<VWaitComponent>(), go);
        }

        /// <summary>
        /// Internal use for wait for Seconds.
        /// </summary>
        private static async ValueTask WaitSeconds(float duration, SignalAwaiter signal, bool realtime)
        {
            if (!realtime)
            {
                var timeScale = Timing(out var val);

                await NextFrame();

                if (timeScale)
                    if (Mathf.Approximately(1, val))
                        await Task.Delay(TimeSpan.FromSeconds(duration - 0.051f), getToken);
                    else
                    {
                        var calcTime = duration + (duration * (1 - val));

                        if (Mathf.Approximately(0, val) || val < 0)
                        {
                            var signalAwaiterReusable = NextFrameReusable();

                            await Task.Run(async () =>
                            {
                                while (Mathf.Approximately(0, Time.timeScale))
                                {
                                    await signalAwaiterReusable;

                                    if (awaitTokenSource.IsCancellationRequested) return;
                                }
                            });
                        }
                        else if (val > 1)
                        {
                            var calc = duration - (duration * (val - 1f));
                            var invalid = calc is 0 or < 0;

                            if (!invalid)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(calc), getToken);

                                if (awaitTokenSource.IsCancellationRequested) return;
                            }

                            PlayerLoopUpdate.playerLoopUtil
                                .QueueFixedUpdate(new SignalAwaiterReusable(awaitTokenSource));

                            return;
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(calcTime), getToken);

                            if (awaitTokenSource.IsCancellationRequested) return;
                        }
                    }
                else await Task.Delay(TimeSpan.FromSeconds(duration - 0.051f), getToken);
            }
            else await Task.Delay(TimeSpan.FromSeconds(duration - 0.051f), getToken);

            PlayerLoopUpdate.playerLoopUtil.QueueEndOfFrame(signal);
        }

        private static bool Timing(out float value)
        {
            var time = Time.timeScale;

            if (Mathf.Approximately(time, 1f))
            {
                value = time;
                return false;
            }

            value = time;
            return true;
        }

        public static double NormalizeTime(double value, double min, double max)
        {
            if (Math.Abs(max - min) < double.Epsilon) return 0.5;

            // Apply normalization formula
            var normalizedValue = (value - min) / (max - min);

            // Ensure normalized value stays within 0 to 1 range
            return Math.Max(0, Math.Min(1, normalizedValue));
        }

        /// <summary>
        /// Waits until Predicate&lt;bool&gt; is True. Can't be awaited multiple times.
        /// </summary>
        /// <param name="predicate">Condition.</param>
        /// <param name="tokenSource">The token source.</param>
        public static async Task<bool> WaitUntil(Predicate<bool> predicate, CancellationTokenSource tokenSource)
        {
            var signalAwaiterReusable = NextFrameReusable();

            while (predicate.Invoke(false))
            {
                await signalAwaiterReusable;

                if (tokenSource.IsCancellationRequested) 
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Behaves similar to PeriodicTimer in c#. Tick count will increase the next frame.
        /// </summary>
        public static async ValueTask PeriodicTimer(float interval, int maxTickCount, Action<int> tick,
            CancellationTokenSource tokenSource)
        {
            if (maxTickCount < 1)
                throw new Exception("VAwait Error : maxTickCount can't be less than 1.");

            var count = 0;
            var signalAwaiterReusable = NextFrameReusable();

            while (maxTickCount != count)
            {
                await Task.Delay(TimeSpan.FromSeconds(interval), tokenSource.Token);

                if (tokenSource.IsCancellationRequested)
                {
                    var signalAwaiter = GetPooled();

                    PlayerLoopUpdate.playerLoopUtil.QueueEndOfFrame(signalAwaiter);
                    break;
                }

                await signalAwaiterReusable;

                if (awaitTokenSource.IsCancellationRequested) return;

                count++;
                tick.Invoke(count);
            }
        }

        /// <summary>
        /// Cancels an await.
        /// </summary>
        public static void TryCancel(int id)
        {
            if (!_id.TryGetValue(id, out var func)) return;
            
            func.Cancel();
            ReturnAwaiterToPool(func);
        }

        /// <summary>
        /// Destroy and cancel awaits, will re-initialize.
        /// </summary>
        public static void ForceCancelAll()
        {
            DestroyAwaits();
            ReInit();
        }

        /// <summary>
        /// Do this everytime getting out of playmode while in edit-mode or 
        /// </summary>
        public static void DestroyAwaits()
        {
            if (awaitTokenSource == null) return;
            
            awaitTokenSource.Cancel();
            awaitTokenSource.Dispose();
            awaitTokenSource = null;
        }

        public static (VWaitComponent component, GameObject gameObject) GetRuntimeInstance() => runtimeInstance;

        //The life time of this token is based on your application's lifetime and can't be cancelled.
        public static CancellationToken getToken => awaitTokenSource.Token;

        public static async Task<AwaitChain> TaskChain(Func<Task> func, CancellationTokenSource cts)
        {
            var chain = new AwaitChain
            {
                cts = cts
            };
            
            await func();

            if (cts.IsCancellationRequested)
            {
                chain.completed = false;
                return chain;
            }

            chain.completed = true;
            return chain;
        }

        public static async Task<AwaitChain> TaskChain(SignalAwaiter func, CancellationTokenSource cts)
        {
            var chain = new AwaitChain
            {
                cts = cts
            };
            
            await func;

            if (cts.IsCancellationRequested)
            {
                chain.completed = false;
                return chain;
            }

            chain.completed = true;
            return chain;
        }

        public static async Task<AwaitChain> Next(this Task<AwaitChain> signal, Func<Task> func)
        {
            await func();

            if (signal.Result.cts.IsCancellationRequested) 
                signal.Result.completed = false;

            signal.Result.completed = true;
            return signal.Result;
        }

        public static async Task<AwaitChain> Next(this Task<AwaitChain> signal, Func<SignalAwaiter> func)
        {
            await func();

            if (signal.Result.cts.IsCancellationRequested) 
                signal.Result.completed = false;

            signal.Result.completed = true;
            return signal.Result;
        }

        public static async Task<T> Await<T>(this T signal, T wait) where T : Task<T>
        {
            await wait;
            return wait;
        }

        /// <summary>
        /// Invokes on threadPool.
        /// </summary>
        /// <param name="func"></param>
        public static void InvokeOnThreadPool(Action func) => Task.Run(func);

        /// <summary>
        /// Invokes on threadPool and await for completion. 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static SignalAwaiter AwaitOnThreadPool(Func<Task> func)
        {
            var signalAwaiter = GetPooled();

            try
            {
                return signalAwaiter;
            }
            finally
            {
                Task.Run(async () =>
                {
                    await func.Invoke();
                    await EndOfFrame();
                    signalAwaiter.TrySetResult(true);
                    ReturnAwaiterToPool(signalAwaiter);
                });
            }
        }
    }

    public enum UPlayStateMode
    {
        PlayMode,
        None
    }
}
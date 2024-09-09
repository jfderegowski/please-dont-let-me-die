using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace NoReleaseDate.Common.Runtime.VAwait
{
    public sealed class AwaitUpdate { }
    public sealed class AwaitEndOfFrame { }
    public sealed class AwaitFixedUpdate { }
    public class PlayerLoopUpdate
    {
        private PlayerLoopSystem _playerLoop;
        public static PlayerLoopUpdate playerLoopUtil { get; set; }
        private static ConcurrentQueue<SignalAwaiter> _signalQueue = new();
        private static ConcurrentQueue<SignalAwaiter> _signalEndOfFrameQueue = new();
        private static ConcurrentQueue<SignalAwaiterReusable> _signalQueueReusableFrame = new();
        private static ConcurrentQueue<SignalAwaiterReusable> _signalQueueFixedUpdate = new();
        private static ConcurrentQueue<SignalAwaiter> _signalQueueFixedUpdateRealtime = new();
        public static int mainthreadID { get; private set; }
        private double _screenRate;
        public PlayerLoopUpdate()
        {
            if (playerLoopUtil == null)
            {
                var refValue = Screen.currentResolution.refreshRateRatio.value;
                _screenRate = (1f / (float)refValue);

                mainthreadID = Thread.CurrentThread.ManagedThreadId;
                Application.wantsToQuit += OnQuit;
                playerLoopUtil = this;
                AssignPlayerLoop(true);
            }
        }
        public Func<int> getCurrentFrame;
        public int dummyFrame = 0;

#if UNITY_EDITOR

        private double _lastTime = 0;

        public void EditModeRunner()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating || Wait.playMode == UPlayStateMode.PlayMode)
            {
                return;
            }

            var time = EditorApplication.timeSinceStartup;

            if (time < (_lastTime + _screenRate))
            {
                return;
            }

            _lastTime = time;
            dummyFrame++;

            if (dummyFrame == int.MaxValue - 1)
            {
                dummyFrame = 1;
            }

            UpdateRun();
        }
#endif

        private void AssignPlayerLoop(bool addElseRemove)
        {
            _playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var update = GetUpdate(_playerLoop);

            if (addElseRemove)
            {
                var customUpdate = new PlayerLoopSystem()
                {
                    updateDelegate = UpdateRun,
                    type = typeof(AwaitUpdate)
                };

                var customEndOfFrameUpdate = new PlayerLoopSystem()
                {
                    updateDelegate = EndFrameUpdateRun,
                    type = typeof(AwaitEndOfFrame)
                };

                var customFixedUpdate = new PlayerLoopSystem()
                {
                    updateDelegate = FixedUpdateRun,
                    type = typeof(AwaitFixedUpdate)
                };

                var copy = ReplaceUpdateRoot(ref _playerLoop, ref customUpdate, true);
                var end = ReplaceEndOfFrameRoot(ref copy, ref customEndOfFrameUpdate, true);
                var fixedupdate = ReplaceFixedUpdateRoot(ref end, ref customFixedUpdate, true);
                PlayerLoop.SetPlayerLoop(fixedupdate);
            }
            else
            {
                var dummy = new PlayerLoopSystem();
                var copy = ReplaceUpdateRoot(ref _playerLoop, ref dummy, false);
                var end = ReplaceEndOfFrameRoot(ref copy, ref dummy, false);
                var fixedupdate = ReplaceFixedUpdateRoot(ref end, ref dummy, false);
                PlayerLoop.SetPlayerLoop(fixedupdate);
            }
        }

        private bool OnQuit()
        {
            AssignPlayerLoop(false);
            return true;
        }

        private PlayerLoopSystem GetUpdate(PlayerLoopSystem loopSystem)
        {
            for (var i = 0; i < loopSystem.subSystemList.Length; i++)
            {
                if (loopSystem.subSystemList[i].type == typeof(Update))
                {
                    for (var j = 0; j < loopSystem.subSystemList[i].subSystemList.Length; j++)
                    {
                        if (loopSystem.subSystemList[i].subSystemList[j].type == typeof(Update.ScriptRunBehaviourUpdate))
                        {
                            return loopSystem.subSystemList[i].subSystemList[j];
                        }
                    }
                }
            }

            return default;
        }
        /// <summary>
        /// Replaces the Update subsystem.
        /// </summary>
        private PlayerLoopSystem ReplaceUpdateRoot(ref PlayerLoopSystem root, ref PlayerLoopSystem custom, bool addCustomUpdateElseClear)
        {
            var lis = root.subSystemList.ToList();
            int? index = null;

            for (var i = 0; i < root.subSystemList.Length; i++)
            {
                if (lis[i].type == typeof(Update))
                {
                    index = i;
                    break;
                }
            }

            if (index.HasValue)
            {
                var tmp = root.subSystemList[index.Value].subSystemList.ToList();

                for (var i = tmp.Count; i-- > 0;)
                {
                    if (tmp[i].type == typeof(AwaitUpdate))
                    {
                        tmp.Remove(tmp[i]);
                    }
                }

                if (addCustomUpdateElseClear)
                {
                    tmp.Insert(0, custom);
                }

                root.subSystemList[index.Value].subSystemList = tmp.ToArray();
            }

            return root;
        }
        /// <summary>
        /// Replaces the PostLateUpdate subsystem.
        /// </summary>
        private PlayerLoopSystem ReplaceEndOfFrameRoot(ref PlayerLoopSystem root, ref PlayerLoopSystem custom, bool addCustomUpdateElseClear)
        {
            var lis = root.subSystemList.ToList();
            int? index = null;

            for (var i = 0; i < root.subSystemList.Length; i++)
            {
                if (lis[i].type == typeof(PreLateUpdate))
                {
                    index = i;
                    break;
                }
            }

            if (index.HasValue)
            {
                var tmp = root.subSystemList[index.Value].subSystemList.ToList();

                for (var i = tmp.Count; i-- > 0;)
                {
                    if (tmp[i].type == typeof(AwaitEndOfFrame))
                    {
                        tmp.Remove(tmp[i]);
                    }
                }

                if (addCustomUpdateElseClear)
                {
                    //1 is after script Update.
                    var idx = tmp.FindIndex(x=> x.type == typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate));
                    tmp.Insert(idx + 1, custom);
                }

                root.subSystemList[index.Value].subSystemList = tmp.ToArray();
            }

            return root;
        }
        /// <summary>
        /// Replaces FixedUpdate subsystem.
        /// </summary>
        private PlayerLoopSystem ReplaceFixedUpdateRoot(ref PlayerLoopSystem root, ref PlayerLoopSystem custom, bool addCustomUpdateElseClear)
        {
            var lis = root.subSystemList.ToList();
            int? index = null;

            for (var i = 0; i < root.subSystemList.Length; i++)
            {
                if (lis[i].type == typeof(FixedUpdate))
                {
                    index = i;
                    break;
                }
            }

            if (index.HasValue)
            {
                var tmp = root.subSystemList[index.Value].subSystemList.ToList();

                for (var i = tmp.Count; i-- > 0;)
                {
                    if (tmp[i].type == typeof(AwaitFixedUpdate))
                    {
                        tmp.Remove(tmp[i]);
                    }
                }

                if (addCustomUpdateElseClear)
                {
                    var idx = tmp.FindIndex(x=> x.type == typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate));
                    tmp.Insert(idx - 1, custom);
                }

                root.subSystemList[index.Value].subSystemList = tmp.ToArray();
            }

            return root;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (playerLoopUtil == null)
            {
                playerLoopUtil = new PlayerLoopUpdate();
            }
        }

        private void EndFrameUpdateRun()
        {
            while (_signalEndOfFrameQueue.TryDequeue(out var signal))
            {
                signal.TrySetResult(true);
                Wait.ReturnAwaiterToPool(signal);
            }

        }

        private void FixedUpdateRun()
        {
            var len = 0;
            var index = 0;

            len = _signalQueueFixedUpdateRealtime.Count;

            if (len > 0)
            {
                while (_signalQueueFixedUpdateRealtime.TryDequeue(out var signal))
                {
                    if (signal.frameIn == getCurrentFrame())
                    {
                        index++;
                        _signalQueueFixedUpdateRealtime.Enqueue(signal);

                        if (index == len)
                        {
                            break;
                        }

                        continue;
                    }

                    signal.TrySetResult(true);
                    index++;
                    Wait.ReturnAwaiterToPool(signal);

                    if (index == len)
                    {
                        break;
                    }
                }

                index = 0;
            }

            len = _signalQueueFixedUpdate.Count;

            if (len > 0)
            {
                while (_signalQueueFixedUpdate.TryDequeue(out var signal))
                {
                    if (signal.frameIn == getCurrentFrame())
                    {
                        index++;
                        _signalQueueFixedUpdate.Enqueue(signal);

                        if (index == len)
                        {
                            break;
                        }

                        continue;
                    }

                    signal.TrySetResult(true);
                    index++;

                    if (index == len)
                    {
                        break;
                    }
                }

                index = 0;
            }
        }

        private void UpdateRun()
        {
            var len = 0;
            var index = 0;

            len = _signalQueue.Count;

            if (len > 0)
            {
                while (_signalQueue.TryDequeue(out var signal))
                {
                    if (signal.frameIn == getCurrentFrame())
                    {
                        index++;
                        _signalQueue.Enqueue(signal);

                        if (index == len)
                        {
                            break;
                        }

                        continue;
                    }

                    signal.TrySetResult(true);
                    index++;
                    Wait.ReturnAwaiterToPool(signal);

                    if (index == len)
                    {
                        break;
                    }
                }

                index = 0;
            }

            len = _signalQueueReusableFrame.Count;

            if (len > 0)
            {
                while (_signalQueueReusableFrame.TryDequeue(out var signal))
                {
                    if (signal.frameIn == getCurrentFrame())
                    {
                        index++;
                        _signalQueueReusableFrame.Enqueue(signal);

                        if (index == len)
                        {
                            break;
                        }

                        continue;
                    }

                    signal.TrySetResult(true);
                    index++;

                    if (index == len)
                    {
                        break;
                    }
                }

                index = 0;
            }
        }

        /// <summary>
        /// Queues the next frame.
        /// </summary>
        /// <param name="signal"></param>
        public void QueueNextFrame(SignalAwaiter signal)
        {
            signal.frameIn = getCurrentFrame();
            _signalQueue.Enqueue(signal);
        }
        /// <summary>
        /// Queue the next fixedUpdate frame.
        /// </summary>
        /// <param name="signal"></param>
        public void QueueFixedNextFrame(SignalAwaiter signal)
        {
            signal.frameIn = getCurrentFrame();
            _signalQueueFixedUpdateRealtime.Enqueue(signal);
        }
        /// <summary>
        /// Queues the next fixed update.
        /// </summary>
        /// <param name="signal"></param>
        public void QueueFixedUpdate(SignalAwaiterReusable signal)
        {
            signal.frameIn = getCurrentFrame();
            _signalQueueFixedUpdate.Enqueue(signal);

        }
        /// <summary>
        /// Queues end of frame.
        /// </summary>
        /// <param name="signal"></param>
        public void QueueEndOfFrame(SignalAwaiter signal)
        {
            _signalEndOfFrameQueue.Enqueue(signal);
        }
        /// <summary>
        /// Queues reusable awaiter the next frame.
        /// </summary>
        /// <param name="signal"></param>
        public void QueueReusableNextFrame(SignalAwaiterReusable signal)
        {
            //We skip the frame init here, it's not needed.
            signal.frameIn = getCurrentFrame();
            _signalQueueReusableFrame.Enqueue(signal);
        }
        /// <summary>
        /// Finds subsystem.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="def"></param>
        /// <returns></returns>
        private static PlayerLoopSystem FindSubSystem<T>(PlayerLoopSystem def)
        {
            if (def.type == typeof(T))
            {
                return def;
            }
            if (def.subSystemList != null)
            {
                foreach (var s in def.subSystemList)
                {
                    var system = FindSubSystem<Update.ScriptRunBehaviourUpdate>(s);
                    if (system.type == typeof(T))
                    {
                        return system;
                    }
                }
            }
            return default(PlayerLoopSystem);
        }
        //TODO: I don't think respecting engine's timeScale is an option here.
        // People using Time.timeScale to pause or to slowdown their games are just wrong to begin with.
        public async ValueTask DeltaTimer(bool realtime)
        {
            if(!realtime)
            {
                if(!Mathf.Approximately(Time.timeScale, 1f))
                {
                    var dif = Time.timeScale;

                    while(dif < 1)
                    {
                        dif += Time.unscaledDeltaTime;
                        await Wait.EndOfFrame();
                    }
                }
            }
        }
    }
}
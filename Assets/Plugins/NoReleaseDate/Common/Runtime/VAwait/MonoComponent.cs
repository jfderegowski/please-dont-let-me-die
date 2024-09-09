using System.Collections;
using UnityEngine;

namespace NoReleaseDate.Common.Runtime.VAwait
{
    [AddComponentMenu("")]
    public class VWaitComponent : MonoBehaviour
    {
        private static VWaitComponent mono { get; set; }
        private WaitForFixedUpdate waitFixed;
        private WaitForEndOfFrame endOfFrame;

        private void Awake()
        {
            waitFixed = new WaitForFixedUpdate();
            endOfFrame = new WaitForEndOfFrame();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (mono == null)
            {
                mono = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private IEnumerator InstanceSeconds(SignalAwaiter signal, WaitForSeconds wait)
        {
            yield return wait;
            signal.TrySetResult(true);
            Wait.ReturnAwaiterToPool(signal);
        }

        private IEnumerator InstanceEndFrame(SignalAwaiter signal)
        {
            yield return endOfFrame;
            signal.TrySetResult(true);
            Wait.ReturnAwaiterToPool(signal);
        }

        private IEnumerator InstanceCoroutineFrame(SignalAwaiter signal)
        {
            yield return null;
            signal.TrySetResult(true);
            Wait.ReturnAwaiterToPool(signal);
        }

        private IEnumerator InstanceCoroutineFrameReusable(SignalAwaiterReusable signal)
        {
            yield return null;
            signal.TrySetResult(true);
        }

        private IEnumerator InstanceCoroutineFixedUpdate(SignalAwaiter signal)
        {
            yield return waitFixed;
            signal.TrySetResult(true);
            Wait.ReturnAwaiterToPool(signal);
        }

        private IEnumerator InstanceCoroutine(IEnumerator coroutine, SignalAwaiter signal)
        {
            yield return coroutine;
            signal.TrySetResult(true);    
            Wait.ReturnAwaiterToPool(signal);
        }
        public void TriggerFixedUpdateCoroutine(SignalAwaiter signal)
        {
            StartCoroutine(InstanceCoroutineFixedUpdate(signal));
        }
        public void TriggerFrameCoroutine(SignalAwaiter signal)
        {
            StartCoroutine(InstanceCoroutineFrame(signal));
        }
        public void TriggerSecondsCoroutine(SignalAwaiter signal, float duration)
        {
            StartCoroutine(InstanceSeconds(signal, new WaitForSeconds(duration)));
        }
        public void TriggerEndFrame(SignalAwaiter signal)
        {
            StartCoroutine(InstanceEndFrame(signal));
        }
        public void TriggerFrameCoroutineReusable(SignalAwaiterReusable signal)
        {
            StartCoroutine(InstanceCoroutineFrameReusable(signal));
        }
        public void TriggerCoroutine(IEnumerator coroutine, SignalAwaiter signal)
        {
            StartCoroutine(InstanceCoroutine(coroutine, signal));
        }

        public void CancelCoroutines()
        {
            StopAllCoroutines();
        }
        public void CancelCoroutine(IEnumerator ienumerator)
        {
            StopCoroutine(ienumerator);
        }

        private void OnApplicationQuit()
        {
            Wait.DestroyAwaits();
            CancelCoroutines();
        }
    }
}
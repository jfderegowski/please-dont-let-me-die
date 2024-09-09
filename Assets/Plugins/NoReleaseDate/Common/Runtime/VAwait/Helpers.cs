using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NoReleaseDate.Common.Runtime.VAwait
{
    public enum VWaitType
    {
        Frame,
        WaitSeconds,
        WaitSecondsRealtime
    }

    /// <summary>
    /// Awaiter class.
    /// </summary>
    public class SignalAwaiter : ICriticalNotifyCompletion
    {
        private Action _continuation;
        private bool _result;
        private readonly CancellationToken _token;
        public bool cancelled { get; private set; }
        public IEnumerator enumerator { get; private set; }
        public int id { get; set; } = -1;
        public int frameIn { get; set; }
        public bool IsCompleted { get; private set; }

        public SignalAwaiter(CancellationTokenSource cts) => _token = cts.Token;

        public void AssignEnumerator(IEnumerator newEnumerator) => this.enumerator = newEnumerator;

        public bool GetResult() => _result;

        public void OnCompleted(Action continuation)
        {
            if (_token.IsCancellationRequested) return;

            if (_continuation != null)
                throw new InvalidOperationException("VAwait Error : Is already being awaited");
            
            _continuation = continuation;
        }

        /// <summary>
        /// Attempts to transition the completion state.
        /// </summary>
        public bool TrySetResult(bool result)
        {
            if (cancelled || _token.IsCancellationRequested) return false;

            if (IsCompleted) return false;
            
            IsCompleted = true;
            _result = result;

            if (_continuation == null) return true;
            
            _continuation?.Invoke();
            _continuation = null;

            return true;
        }

        /// <summary>
        /// Reset the awaiter to initial status
        /// </summary>
        public SignalAwaiter Reset()
        {
            if (_token.IsCancellationRequested) return this;

            Cancel();
            
            _result = false;
            _continuation = null;
            IsCompleted = false;
            cancelled = false;
            
            return this;
        }

        public SignalAwaiter GetAwaiter() => this;

        public void Cancel()
        {
            cancelled = true;

            if (enumerator != null)
            {
                Wait.GetRuntimeInstance().component.CancelCoroutine(enumerator);
                enumerator = null;
            }

            if (id <= -1) return;
            Wait.RemoveID(id);
            id = -1;
        }

        public void UnsafeOnCompleted(Action continuation) => _continuation = continuation;
    }
}
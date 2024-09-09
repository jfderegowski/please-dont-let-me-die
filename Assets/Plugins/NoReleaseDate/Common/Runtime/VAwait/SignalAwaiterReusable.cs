using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NoReleaseDate.Common.Runtime.VAwait
{
    public class SignalAwaiterReusable : ICriticalNotifyCompletion
    {
        private Action _continuation;
        private bool _result;
        private readonly CancellationToken token;
        public int frameIn { get; set; }
        public bool IsCompleted { get; private set; }

        public SignalAwaiterReusable(CancellationTokenSource cts)
        {
            token = cts.Token;
        }

        public bool GetResult()
        {
            return _result;
        }

        public void OnCompleted(Action continuation)
        {
            if (_continuation != null)
                throw new InvalidOperationException("VAwait Error : Is already being awaited");

            _continuation = continuation;
        }

        /// <summary>
        /// Attempts to transition the completion state.
        /// </summary>
        public bool TrySetResult(bool result)
        {
            if (token.IsCancellationRequested)
            {
                return false;
            }

            if (!IsCompleted)
            {
                IsCompleted = true;
                _result = result;
                _continuation?.Invoke();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset the awaiter to initial status
        /// </summary>
        public void Reset()
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            _result = false;
            _continuation = null;
            IsCompleted = false;
        }

        public SignalAwaiterReusable GetAwaiter()
        {
            var pass = false;

            if (frameIn != PlayerLoopUpdate.playerLoopUtil.getCurrentFrame())
            {
                Reset();
                pass = true;
            }

            try
            {
                return this;
            }
            finally
            {
                if (pass)
                {
                    PlayerLoopUpdate.playerLoopUtil.QueueReusableNextFrame(this);
                }
            }
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        public void Cancel()
        {
            Reset();
        }
    }
}
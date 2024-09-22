using System;
using System.Collections.Generic;

namespace UISystem.UIAnimationSystem.New
{
    public class StateAnimations<T> : IDisposable where T : Enum
    {
        private readonly Dictionary<T, StateAnimation> _animations = new();
        private StateAnimation _lastStateAnimation;
        private Func<T> _currentState;

        public void UpdateState() => Play(_currentState());
        
        public void Play(T state)
        {
            if (!_animations.TryGetValue(state, out var animation)) return;
            
            _lastStateAnimation?.Kill();
            animation?.Play();
            _lastStateAnimation = animation;
        }

        public void Dispose()
        {
            foreach (var animation in _animations.Values) 
                animation?.Dispose();
            
            _animations.Clear();
            _lastStateAnimation?.Dispose();
            _lastStateAnimation = null;
        }
        
        public class Builder
        {
            private readonly StateAnimations<T> _stateAnimationsObj = new();
            
            public Builder SetAnimation(T state, StateAnimation stateAnimation)
            {
                _stateAnimationsObj._animations[state] = stateAnimation;
                
                return this;
            }

            public StateAnimations<T> Build(Func<T> currentState)
            {
                _stateAnimationsObj._currentState = currentState;
                
                return _stateAnimationsObj;
            }
        }
    }
}
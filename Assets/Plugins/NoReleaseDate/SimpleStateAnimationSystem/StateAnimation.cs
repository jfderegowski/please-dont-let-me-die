using System;

namespace UISystem.UIAnimationSystem.New
{
    public class StateAnimation : IDisposable
    {
        public event Action animation;
        public event Action kill;

        public StateAnimation(Action animation, Action kill)
        {
            this.animation = animation;
            this.kill = kill;
        }
        
        public void Play() => animation?.Invoke();

        public void Kill() => kill?.Invoke();
        
        public void Dispose()
        {
            Kill();
            
            animation = null;
            kill = null;
        }
    }
}
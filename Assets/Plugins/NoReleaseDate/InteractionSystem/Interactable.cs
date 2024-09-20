using System;
using UnityEngine;

namespace Plugins.NoReleaseDate.InteractionSystem
{
    public class Interactable : MonoBehaviour
    {
        public event Action<InteractingAgent> onInteractBefore;
        public event Action<InteractingAgent> onInteract;
        public event Action<InteractingAgent> onInteractAfter;
        public event Action<InteractingAgent> onInteractFailedBefore;
        public event Action<InteractingAgent> onInteractFailed;
        public event Action<InteractingAgent> onInteractFailedAfter;
        
        public virtual bool isInteractable => true;
        
        public virtual bool CanInteract(InteractingAgent interactingAgent) => isInteractable;
        
        public virtual void Interact(InteractingAgent interactingAgent)
        {
            if (!CanInteract(interactingAgent))
            {
                OnInteractFailedBefore(interactingAgent);
                OnInteractFailed(interactingAgent);
                OnInteractFailedAfter(interactingAgent);
                return;
            }
            
            OnInteractBefore(interactingAgent);
            OnInteract(interactingAgent);
            OnInteractAfter(interactingAgent);
        }
        
        protected virtual void OnInteractBefore(InteractingAgent interactingAgent) => onInteractBefore?.Invoke(interactingAgent);
        protected virtual void OnInteract(InteractingAgent interactingAgent) => onInteract?.Invoke(interactingAgent);
        protected virtual void OnInteractAfter(InteractingAgent interactingAgent) => onInteractAfter?.Invoke(interactingAgent);
        protected virtual void OnInteractFailedBefore(InteractingAgent interactingAgent) => onInteractFailedBefore?.Invoke(interactingAgent);
        protected virtual void OnInteractFailed(InteractingAgent interactingAgent) => onInteractFailed?.Invoke(interactingAgent);
        protected virtual void OnInteractFailedAfter(InteractingAgent interactingAgent) => onInteractFailedAfter?.Invoke(interactingAgent);
    }
}

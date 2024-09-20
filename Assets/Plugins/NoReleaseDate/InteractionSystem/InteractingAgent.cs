using System;
using FishNet.Object;

namespace Plugins.NoReleaseDate.InteractionSystem
{
    public class InteractingAgent : NetworkBehaviour
    {
        public event Action<Interactable> onInteractBefore;
        public event Action<Interactable> onInteract;
        public event Action<Interactable> onInteractAfter;
        public event Action<Interactable> onInteractFailedBefore;
        public event Action<Interactable> onInteractFailed;
        public event Action<Interactable> onInteractFailedAfter;
        
        public virtual bool canAgentInteracting => true;

        public virtual bool CanInteractWitch(Interactable interactable) => canAgentInteracting;

        public virtual void InteractWith(Interactable interactable)
        {
            if (!CanInteractWitch(interactable))
            {
                OnInteractFailedBefore(interactable);
                OnInteractFailed(interactable);
                OnInteractFailedAfter(interactable);
                return;
            }
            
            OnInteractBefore(interactable);
            
            interactable.Interact(this);
            
            OnInteract(interactable);
            OnInteractAfter(interactable);
        }
        
        protected virtual void OnInteractBefore(Interactable interactable) => onInteractBefore?.Invoke(interactable);
        protected virtual void OnInteract(Interactable interactable) => onInteract?.Invoke(interactable);
        protected virtual void OnInteractAfter(Interactable interactable) => onInteractAfter?.Invoke(interactable);
        protected virtual void OnInteractFailedBefore(Interactable interactable) => onInteractFailedBefore?.Invoke(interactable);
        protected virtual void OnInteractFailed(Interactable interactable) => onInteractFailed?.Invoke(interactable);
        protected virtual void OnInteractFailedAfter(Interactable interactable) => onInteractFailedAfter?.Invoke(interactable);
    }
}

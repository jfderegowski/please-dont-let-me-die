using System.Collections.Generic;
using NoReleaseDate.SingletonSystem.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace IntuitiveBackSystem.Runtime
{
    /// <summary>
    /// Manager that handles the back action and calls the current IIntuitiveBackHandler.
    /// It will call the OnBack method of the current IIntuitiveBackHandler when the back action is performed.
    /// </summary>
    public class IntuitiveBackManager : Singleton<IntuitiveBackManager>
    {
        /// <summary>
        /// Event that is called when a new IIntuitiveBackHandler is registered.
        /// </summary>
        [field: SerializeField] public UnityEvent<IIntuitiveBackHandler> onRegister { get; private set; } = new();

        /// <summary>
        /// Event that is called when an IIntuitiveBackHandler is unregistered.
        /// </summary>
        [field: SerializeField] public UnityEvent<IIntuitiveBackHandler> onUnregister { get; set; } = new();

        /// <summary>
        /// The current IIntuitiveBackHandler that is registered.
        /// </summary>
        public IIntuitiveBackHandler currentIntuitiveBackHandler => backHandlers.Count > 0 ? backHandlers[0] : null;

        /// <summary>
        /// A list of all registered IIntuitiveBackHandler (First in, first out)
        /// </summary>
        public List<IIntuitiveBackHandler> backHandlers { get; } = new();

        [SerializeField] private InputAction _backAction;

        protected override void Awake()
        {
            base.Awake();
            
            _backAction.performed += OnBackPerformed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _backAction.performed -= OnBackPerformed;
            UnregisterAll();
        }

        private void OnBackPerformed(InputAction.CallbackContext context) =>
            currentIntuitiveBackHandler?.OnBack();

        /// <summary>
        /// Register a new IIntuitiveBackHandler.
        /// </summary>
        /// <param name="intuitiveBackHandler">The IIntuitiveBackHandler to register.</param>
        public void Register(IIntuitiveBackHandler intuitiveBackHandler)
        {
            backHandlers.Insert(0, intuitiveBackHandler);

            if (backHandlers.Count > 0)
                _backAction.Enable();

            onRegister?.Invoke(intuitiveBackHandler);
        }

        /// <summary>
        /// Unregister an IIntuitiveBackHandler.
        /// </summary>
        /// <param name="intuitiveBackHandler">The IIntuitiveBackHandler to unregister.</param>
        public void Unregister(IIntuitiveBackHandler intuitiveBackHandler)
        {
            if (intuitiveBackHandler == null) return;
            if (!backHandlers.Contains(intuitiveBackHandler)) return;

            backHandlers.Remove(intuitiveBackHandler);

            if (backHandlers.Count == 0)
                _backAction.Disable();

            onUnregister?.Invoke(intuitiveBackHandler);
        }

        private void UnregisterAll()
        {
            for (var i = 0; i < backHandlers.Count; i++)
                Unregister(backHandlers[i]);
        }
    }
}

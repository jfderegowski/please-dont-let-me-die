using UnityEngine;

namespace NoReleaseDate.CursorSystem.Runtime
{
    /// <summary>
    /// Generic cursor lock mode class.
    /// It handles the cursor lock mode.
    /// </summary>
    public class GenericCursorLockMode : MonoBehaviour, ICursorHandler
    {
        private enum UnityLifecycleMethod
        {
            None,
            Awake,
            OnEnable,
            Start,
            OnDisable,
            OnDestroy
        }
        
        /// <summary>
        /// The cursor settings.
        /// </summary>
        [field: SerializeField] public CursorSettings cursorSettings { get; set; } = CursorSettings.hideAndLock;

        [Tooltip("The place where the cursor handler will be registered.")]
        [SerializeField] private UnityLifecycleMethod _registerAt = UnityLifecycleMethod.OnEnable;
        [Tooltip("The place where the cursor handler will be unregistered.")]
        [SerializeField] private UnityLifecycleMethod _unregisterAt = UnityLifecycleMethod.OnDisable;

        private static CursorManager cursorManager => CursorManager.instance;

        private void OnValidate()
        {
            if (_registerAt != _unregisterAt) return;
            _registerAt = UnityLifecycleMethod.None;
            Debug.LogWarning("Register and Unregister places are the same. Change RegisterAt to None.", gameObject);
        }

        private void Awake() => HandleRegisterUnregisterPlace(UnityLifecycleMethod.Awake);
        
        private void OnEnable() => HandleRegisterUnregisterPlace(UnityLifecycleMethod.OnEnable);
        
        private void Start() => HandleRegisterUnregisterPlace(UnityLifecycleMethod.Start);
        
        private void OnDisable() => HandleRegisterUnregisterPlace(UnityLifecycleMethod.OnDisable);
        
        private void OnDestroy() => HandleRegisterUnregisterPlace(UnityLifecycleMethod.OnDestroy);

        /// <summary>
        /// Register the cursor handler.
        /// </summary>
        public void Register() => cursorManager.Register(this);

        /// <summary>
        /// Unregister the cursor handler.
        /// </summary>
        public void Unregister() => cursorManager.Unregister(this);

        private void HandleRegisterUnregisterPlace(UnityLifecycleMethod place)
        {
            if (_registerAt == _unregisterAt)
            {
                Debug.LogError("[CursorSystem] Register and Unregister places are the same.");
                return;
            }
            
            if (place == _registerAt) 
                Register();
            
            if (place == _unregisterAt) 
                Unregister();
        }
    }
}
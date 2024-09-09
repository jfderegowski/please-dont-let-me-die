using UnityEngine;

namespace NoReleaseDate.SingletonSystem.Runtime
{
    /// <summary>
    /// Base class for all singletons.
    /// </summary>
    [DefaultExecutionOrder(-9990)]
    public abstract class Singleton : MonoBehaviour
    {
        /// <summary>
        /// If true, the singleton will be added to the DontDestroyOnLoad list.
        /// </summary>
        public abstract bool dontDestroyOnLoad { get; protected set; }
        
        /// <summary>
        /// Initializes the singleton.
        /// </summary>
        internal abstract void Initialize();
    }
    
    /// <summary>
    /// Singleton class that can be inherited to create a singleton.
    /// </summary>
    /// <typeparam name="T">The type of the singleton.</typeparam>
    public class Singleton<T> : Singleton where T : Component
    {
        /// <summary>
        /// The instance of the singleton.
        /// </summary>
        public static T instance
        {
            get
            {
                if (_instance) return _instance;

                _instance = Collection.Get<T>();

                return _instance;
            }
        }

        [field: SerializeField, Tooltip("If true, the singleton will be added to the DontDestroyOnLoad list.")]
        public override bool dontDestroyOnLoad { get; protected set; } = true;
        
        private static T _instance;

        /// <summary>
        /// Remember to call base.Awake() when overriding this method.
        /// Otherwise, the singleton will not be initialized.
        /// </summary>
        protected virtual void Awake() => Initialize();

        /// <summary>
        /// Remember to call base.OnDestroy() when overriding this method.
        /// Otherwise, the singleton will not be unregistered.
        /// </summary>
        protected virtual void OnDestroy() => Collection.UnRegister(this);

        /// <summary>
        /// Initializes the singleton
        /// </summary>
        internal override void Initialize()
        {
            if (_instance && _instance != this)
            {
                Debug.LogWarning($"[SingletonSystem] Singleton of type {typeof(T).Name} already exists. Destroying this instance.");
                Destroy(gameObject);
            }
            else if (!_instance)
            {
                _instance = GetComponent<T>();
                Collection.Register(this);
            }
        }
    }
}
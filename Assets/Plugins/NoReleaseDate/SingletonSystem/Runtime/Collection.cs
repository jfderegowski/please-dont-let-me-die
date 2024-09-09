using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoReleaseDate.SingletonSystem.Runtime
{
    /// <summary>
    /// Collection of all singletons.
    /// </summary>
    public class Collection : SingletonScriptableObject<Collection>
    {
        /// <summary>
        /// The list of all singletons.
        /// </summary>
        [field: SerializeField, Tooltip("Add a singleton to the list and it will be initialized on startup.")]
        public List<Singleton> singletons { get; private set; } = new();
        
        /// <summary>
        /// The list of all scriptable object singletons.
        /// </summary>
        [field: SerializeField, Tooltip("Add a scriptable object singleton to the list and it will be get from there.")]
        public List<SingletonScriptableObject> scriptableObjectSingletons { get; private set; } = new();
        
        private void OnValidate() => CheckForDuplicates();

        /// <summary>
        /// Validates the singletons before initializing them.
        /// </summary>
        private void ValidateBeforeInitialize()
        {
            CheckForNulls();
            CheckForDuplicates();
        }

        /// <summary>
        /// Checks for null singletons.
        /// </summary>
        private void CheckForNulls()
        {
            var nulls = (from singleton in singletons
                where !singleton
                select singleton).ToList();

            foreach (var nullSingleton in nulls)
            {
                Debug.Log($"[SingletonSystem] Removing null singleton", this);
                singletons.Remove(nullSingleton);
            }
        }

        /// <summary>
        /// Checks for duplicate singletons.
        /// </summary>
        private void CheckForDuplicates()
        {
            var duplicates = (from singleton in singletons
                where singleton
                let count = singletons.Count(otherSingleton => singleton == otherSingleton)
                where count > 1
                select singleton).ToList();

            foreach (var duplicate in duplicates) 
                Debug.Log($"[SingletonSystem] Duplicate singleton: {duplicate}", this);
            
            var soDuplicates = (from soSingleton in scriptableObjectSingletons
                where soSingleton
                let count = scriptableObjectSingletons.Count(otherSoSingleton => soSingleton == otherSoSingleton)
                where count > 1
                select soSingleton).ToList();
            
            foreach (var soDuplicate in soDuplicates)
                Debug.Log($"[SingletonSystem] Duplicate scriptable object singleton: {soDuplicate}", this);
        }

        #region Initialization

        /// <summary>
        /// The list of all spawned singletons.
        /// This list is used to get the singleton of the specified type.
        /// This list is also used to check if the singleton is already spawned.
        /// </summary>
        private static readonly List<Singleton> _spawnedSingletons = new();
        
        /// <summary>
        /// The list of all singletons to enable.
        /// </summary>
        private static readonly Dictionary<Singleton, bool> _singletonsToEnable = new();

        /// <summary>
        /// Initializes the singleton collection.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            instance.ValidateBeforeInitialize();

            _spawnedSingletons.Clear();
            _singletonsToEnable.Clear();
            
            foreach (var singletonPrefab in instance.singletons)
            {
                var prevEnabled = singletonPrefab.gameObject.activeSelf;

                singletonPrefab.gameObject.SetActive(false);

                var singletonClone = Instantiate(singletonPrefab);

                singletonPrefab.gameObject.SetActive(prevEnabled);

                singletonClone.Initialize();

                if (singletonClone.dontDestroyOnLoad)
                    DontDestroyOnLoad(singletonClone.gameObject);
                
                _singletonsToEnable.Add(singletonClone, prevEnabled);
            }
            
            foreach (var (singleton, prevEnabled) in _singletonsToEnable) 
                singleton.gameObject.SetActive(prevEnabled);
        }

        /// <summary>
        /// Gets the singleton of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton.</typeparam>
        /// <returns></returns>
        internal static T Get<T>() where T : Component
        {
            if (Application.isPlaying)
            {
                foreach (var singleton in _spawnedSingletons)
                    if (singleton is T prefab) return prefab;
            }
            else
            {
                foreach (var singleton in instance.singletons)
                    if (singleton is T prefab) return prefab;
            }

            Debug.Log($"[SingletonSystem] No found singleton of type {typeof(T).Name}. Calling Object.FindObjectOfType<T>()");
            var findObject = FindFirstObjectByType<T>();

            return findObject ? findObject : new GameObject(typeof(T).Name).AddComponent<T>();
        }

        /// <summary>
        /// Registers a singleton.
        /// </summary>
        /// <param name="singleton">The singleton to register.</param>
        internal static void Register(Singleton singleton) => _spawnedSingletons.Add(singleton);

        /// <summary>
        /// Unregisters a singleton.
        /// </summary>
        /// <param name="singleton">The singleton to unregister.</param>
        internal static void UnRegister(Singleton singleton) => _spawnedSingletons.Remove(singleton);

        #endregion
    }
}
using System.Linq;
using UnityEngine;

namespace NoReleaseDate.SingletonSystem.Runtime
{
    /// <summary>
    /// Base class for scriptable object singletons.
    /// </summary>
    public abstract class SingletonScriptableObject : ScriptableObject { }

    /// <summary>
    /// Adds singleton functionality to a scriptable object.
    /// </summary>
    /// <typeparam name="T">The type of the scriptable object singleton.</typeparam>
    public abstract class SingletonScriptableObject<T> : SingletonScriptableObject where T : SingletonScriptableObject<T>
    {
        /// <summary>
        /// The instance of the scriptable object singleton.
        /// </summary>
        public static T instance
        {
            get
            {
                if (_instance) return _instance;

                if (Collection._instance)
                    foreach (var soSingleton in Collection._instance.scriptableObjectSingletons.Where(
                                 soSingleton => soSingleton.GetType() == typeof(T)))
                        return _instance = soSingleton as T;

                _instance = Resources.Load<T>(typeof(T).Name);

                if (_instance) return _instance;

#if UNITY_EDITOR
                _instance = CreateScriptableObject();
                
                static T CreateScriptableObject()
                {
                    var soInstance = CreateInstance<T>();
                    soInstance.name = typeof(T).Name;

                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

                    // if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources/SingletonSystem"))
                    //     UnityEditor.AssetDatabase.CreateFolder("Assets/Resources", "SingletonSystem");

                    UnityEditor.AssetDatabase.CreateAsset(soInstance, $"Assets/Resources/{typeof(T).Name}.asset");
                    
                    UnityEditor.AssetDatabase.SaveAssets();

                    return soInstance;
                }
#endif

                return _instance;
            }
        }

        private static T _instance;
    }
}
using NoReleaseDate.SingletonSystem.Runtime;
using UnityEditor;

namespace NoReleaseDate.SingletonSystem.Editor
{
    public static class SingletonsCollectionEditor
    {
        /// <summary>
        /// Open the SingletonsCollection in the inspector
        /// </summary>
        [MenuItem("Tools/No Release Date/Singleton System/Open Singletons Collection")]
        private static void OpenBindingsCollection() => EditorUtility.OpenPropertyEditor(Collection.instance);
    }
}
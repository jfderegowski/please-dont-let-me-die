using UnityEditor;

namespace NoReleaseDate.ThemeSystem.Runtime.Editor
{
    public static class ThemeManagerEditor
    {
        /// <summary>
        /// Open the SingletonsCollection in the inspector
        /// </summary>
        [MenuItem("Tools/No Release Date/Theme System/Open Theme Manager")]
        private static void OpenThemeManager() => EditorUtility.OpenPropertyEditor(ThemeManager.instance);
    }
}
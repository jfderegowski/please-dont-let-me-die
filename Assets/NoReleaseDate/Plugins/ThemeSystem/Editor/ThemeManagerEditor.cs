using NoReleaseDate.ThemeSystem.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

namespace NoReleaseDate.Plugins.ThemeSystem.Editor
{
    public static class ThemeManagerEditor
    {
        /// <summary>
        /// Open the SingletonsCollection in the inspector
        /// </summary>
        [MenuItem("Tools/No Release Date/Theme System/Open Theme Manager")]
        private static void OpenThemeManager() => EditorUtility.OpenPropertyEditor(ThemeManager.instance);
        
        /// <summary>
        /// Add a ThemeControllerImage to the Image component
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Image/Add Theme Controller")]
        private static void AddUIThemeController(MenuCommand command)
        {
            var image = (Image)command.context;
            
            if (!image.gameObject.GetComponent<ThemeControllerImage>()) 
                image.gameObject.AddComponent<ThemeControllerImage>();
        }
        
        /// <summary>
        /// Add a ThemeControllerText to the TextMeshProUGUI component
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/TMP_Text/Add Theme Controller")]
        private static void AddTMPThemeController(MenuCommand command)
        {
            var text = (TextMeshProUGUI)command.context;
            
            if (!text.gameObject.GetComponent<ThemeControllerText>()) 
                text.gameObject.AddComponent<ThemeControllerText>();
        }
    }
}
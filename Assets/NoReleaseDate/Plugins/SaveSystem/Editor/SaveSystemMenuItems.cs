using Plugins.SaveSystem;
using UnityEditor;
using UnityEngine;

namespace NoReleaseDate.SaveSystem.Editor
{
    public static class SaveSystemMenuItems
    {
        [MenuItem("Tools/No Release Date/Save System/Open Persistent Data Folder", priority = 0)]
        private static void OpenPersistentDataFolder() =>
            SaveSystemEditorHelpers.OpenFolder(Application.persistentDataPath);
    }
}
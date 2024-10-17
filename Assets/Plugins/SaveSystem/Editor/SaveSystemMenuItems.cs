using Plugins.SaveSystem;
using SaveSystem;
using UnityEditor;
using UnityEngine;

namespace NoReleaseDate.SaveSystem.Editor
{
    public static class SaveSystemMenuItems
    {
        [MenuItem("Tools/Save System/Save Manager/Open Persistent Data Folder", priority = 0)]
        private static void OpenPersistentDataFolder() =>
            SavePath.OpenFolder(Application.persistentDataPath);
    }
}
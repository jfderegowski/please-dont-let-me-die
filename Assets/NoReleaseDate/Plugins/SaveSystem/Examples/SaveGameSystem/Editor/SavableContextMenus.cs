using System;
using NoReleaseDate.Plugins.SaveSystem.Examples.SaveGameSystem;
using NoReleaseDate.Plugins.SaveSystem.Runtime.DataStructure;
using UnityEditor;

namespace Plugins.SaveSystem.Examples.SaveGameSystem.Editor
{
    public static class SavableContextMenus
    {
        [MenuItem("CONTEXT/SavableMonoBehaviour/Generate New Save Key")]
        private static void GenerateNewSaveKey(MenuCommand menuCommand)
        {
            var saveBehaviour = (SavableMonoBehaviour)menuCommand.context;
            saveBehaviour.SaveKey = SaveKey.RandomKey;
        }
    }
}
using System.IO;
using SaveSystem;
using UnityEditor;

namespace Plugins.SaveSystem.Examples.SaveGameSystem.Editor
{
    public static class SaveGameManagerMenuItems
    {
        [MenuItem("Tools/Save System/Save Game Manager/Save/Auto Save", priority = 0)]
        public static async void AutoSave() =>
            await SaveGameManager.AutoSave();
        
        [MenuItem("Tools/Save System/Save Game Manager/Save/Quick Save", priority = 1)]
        public static async void QuickSave() =>
            await SaveGameManager.QuickSave();
        
        [MenuItem("Tools/Save System/Save Game Manager/Save/Manual Save", priority = 2)]
        public static async void ManualSave() =>
            await SaveGameManager.ManualSave();
        
        [MenuItem("Tools/Save System/Save Game Manager/Load From Latest Save" , priority = 1)]
        public static async void LoadFromLatestSave() => 
            await SaveGameManager.LoadFromLatestSave();
        
        [MenuItem("Tools/Save System/Save Game Manager/Ask For Save", priority = 20)]
        public static async void AskForSave() =>
            await SaveGameManager.AskForSave();

        [MenuItem("Tools/Save System/Save Game Manager/Ask For Load" , priority = 21)]
        public static async void AskForLoad() => 
            await SaveGameManager.AskForLoad();
        
        [MenuItem("Tools/Save System/Save Game Manager/Open Saves Folder", priority = 40)]
        public static void OpenSavesFolder()
        {
            if (!Directory.Exists(SaveGameManager.SaveFolderPath))
                Directory.CreateDirectory(SaveGameManager.SaveFolderPath);
            
            System.Diagnostics.Process.Start(SaveGameManager.SaveFolderPath);
        }
        
        [MenuItem("Tools/Save System/Save Game Manager/Open Last Save File", priority = 41)]
        public static void OpenLastSaveFile()
        {
            var latestSaveFile = SaveGameManager.GetLatestSaveFile();
            
            if (latestSaveFile == null)
                return;
            
            SavePath.OpenFile(latestSaveFile.FullName);
        }

        [MenuItem("Tools/Save System/Save Game Manager/Clear Save Data", priority = 60)]
        public static void ClearSaveData() =>
            SaveGameManager.CurrentSaveData.ClearData();
    }
}
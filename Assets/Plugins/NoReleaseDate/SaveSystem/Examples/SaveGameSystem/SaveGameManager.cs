using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NoReleaseDate.Common.Runtime.Extensions;
using Plugins.SaveSystem.DataStructure;
using SaveSystem;
using SaveSystem.Runtime.DataStructure;
using SaveSystem.Runtime.Settings;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.SaveSystem.Examples.SaveGameSystem
{
    public enum SaveType
    {
        QuickSave,
        AutoSave,
        ManualSave
    }
    
    public enum SavingStatus
    {
        None,
        SavingToFile,
        LoadingFromFile
    }
    
    public static class SaveGameManager
    {
        public static event Action onQuickSave;
        public static event Action onAutoSave;
        public static event Action onManualSave;
        public static event Action onLoad;
        public static event Action<SavingStatus> onSavingStatusChange;
        public static event Action<SavableMonoBehaviour, ClassData, ClassData> onSavableSaveBefore;
        public static event Action<SavableMonoBehaviour, ClassData, ClassData> onSavableSaveAfter;
        public static event Action<SavableMonoBehaviour, ClassData, ClassData> onSavableLoadBefore;
        public static event Action<SavableMonoBehaviour, ClassData, ClassData> onSavableLoadAfter;
        public static event Action onAskForLoadBefore;
        public static event Action onAskForLoad;
        public static event Action onAskForLoadAfter;
        public static event Action onAskForSaveBefore;
        public static event Action onAskForSave;
        public static event Action onAskForSaveAfter;
        
        public static SaveData CurrentSaveData { get; private set; } = new SaveData();
        public static int CurrentSaveProfileIndex { get; private set; }

        public static SavingStatus SavingStatus
        {
            get => _savingStatus;
            private set
            {
                if (_savingStatus == value) return;
                
                _savingStatus = value;
                
                onSavingStatusChange?.Invoke(_savingStatus);
            }
        }

        private static SavingStatus _savingStatus = SavingStatus.None;

        public static string CurrentRootSaveFolderPath =>
            GetFolderPath($"{Application.persistentDataPath}/{SAVES_FOLDER_NAME}");

        public static string CurrentProfileFolderPath =>
            GetFolderPath($"{CurrentRootSaveFolderPath}/{PROFILE_FOLDER_NAME_PREFIX}{CurrentSaveProfileIndex}");

        public static string CurrentQuickSaveFolderPath =>
            GetFolderPath($"{CurrentProfileFolderPath}/{QUICK_SAVE_FOLDER_NAME}");

        public static string CurrentAutoSaveFolderPath =>
            GetFolderPath($"{CurrentProfileFolderPath}/{AUTO_SAVE_FOLDER_NAME}");

        public static string CurrentManualSaveFolderPath => 
            GetFolderPath($"{CurrentProfileFolderPath}/{MANUAL_SAVE_FOLDER_NAME}");

        private const string SAVES_FOLDER_NAME = "Saves";
        private const string PROFILE_FOLDER_NAME_PREFIX = "Profile_";
        private const string QUICK_SAVE_FOLDER_NAME = "QuickSaves";
        private const string AUTO_SAVE_FOLDER_NAME = "AutoSaves";
        private const string MANUAL_SAVE_FOLDER_NAME = "ManualSaves";
        private const string SAVE_FILE_EXTENSION = ".sav";

        public static void SaveToSaveData(SavableMonoBehaviour savableMonoBehaviour, bool debug = false)
        {
            var saveKey = savableMonoBehaviour.SaveKey;
            var comment = savableMonoBehaviour.SaveKey.Comment.value;
            var defDataToSave = savableMonoBehaviour.DefSaveData;
            var dataToSave = savableMonoBehaviour.DataToSave;
            var previousSavedData = CurrentSaveData.GetKey(saveKey, defDataToSave);

            onSavableSaveBefore?.Invoke(savableMonoBehaviour, previousSavedData, dataToSave);
            savableMonoBehaviour.OnBeforeSave?.Invoke(previousSavedData, dataToSave);
            
            CurrentSaveData.SetKey(saveKey, dataToSave);
            
            savableMonoBehaviour.OnAfterSave?.Invoke(previousSavedData, dataToSave);
            onSavableSaveAfter?.Invoke(savableMonoBehaviour, previousSavedData, dataToSave);

            if (debug) 
                Debug.Log($"[SAVE-MANAGER] Saved: {saveKey}\n{string.Join(Environment.NewLine, dataToSave)}", savableMonoBehaviour);
        }
        
        public static void LoadFromSaveData(SavableMonoBehaviour savableMonoBehaviour, bool debug = false)
        {
            var saveKey = savableMonoBehaviour.SaveKey;
            var defDataToSave = savableMonoBehaviour.DefSaveData;
            var currentData = savableMonoBehaviour.DataToSave;
            var dataToLoad = CurrentSaveData.GetKey(saveKey, defDataToSave);
            
            onSavableLoadBefore?.Invoke(savableMonoBehaviour, currentData, dataToLoad);
            savableMonoBehaviour.OnBeforeLoad?.Invoke(currentData, dataToLoad);
            
            savableMonoBehaviour.OnLoad(dataToLoad);
            
            savableMonoBehaviour.OnAfterLoad?.Invoke(currentData, dataToLoad);
            onSavableLoadAfter?.Invoke(savableMonoBehaviour, currentData, dataToLoad);

            if (debug) 
                Debug.Log($"[SAVE-MANAGER] Loaded: {saveKey}\n{string.Join(Environment.NewLine, dataToLoad)}", savableMonoBehaviour);
        }

        public static async Task AutoSave()
        {
            SavingStatus = SavingStatus.SavingToFile;
            
            var saveSettings = DefaultSaveSettings.instance.SaveSettings;
            var path = CurrentAutoSaveFolderPath 
                   + "/" + await SaveSystemHelpers.GetUniqueFileName(CurrentAutoSaveFolderPath)
                   + SAVE_FILE_EXTENSION;
            
            await CurrentSaveData.Save(path, saveSettings);
            
            onAutoSave?.Invoke();
            
            SavingStatus = SavingStatus.None;
        }

        public static async Task QuickSave()
        {
            SavingStatus = SavingStatus.SavingToFile;
            
            var saveSettings = DefaultSaveSettings.instance.SaveSettings;
            var path = CurrentQuickSaveFolderPath 
                       + "/" + await SaveSystemHelpers.GetUniqueFileName(CurrentQuickSaveFolderPath)
                       + SAVE_FILE_EXTENSION;
            
            await CurrentSaveData.Save(path, saveSettings);
            
            onQuickSave?.Invoke();
            
            SavingStatus = SavingStatus.None;
        }

        public static async Task ManualSave()
        {
            SavingStatus = SavingStatus.SavingToFile;
            
            var saveSettings = DefaultSaveSettings.instance.SaveSettings;
            var path = CurrentManualSaveFolderPath 
                       + "/" + await SaveSystemHelpers.GetUniqueFileName(CurrentManualSaveFolderPath)
                       + SAVE_FILE_EXTENSION;
            
            await CurrentSaveData.Save(path, saveSettings);
            
            onManualSave?.Invoke();
            
            SavingStatus = SavingStatus.None;
        }

        public static async Task LoadFromLatestSave()
        {
            var latestSaveFile = GetLatestSaveFile();

            if (latestSaveFile == null) return;
            
            await LoadFromFile(latestSaveFile.FullName);
        }

        public static async Task LoadFromFile(string filePath)
        {
            SavingStatus = SavingStatus.LoadingFromFile;

            await CurrentSaveData.Load(filePath);
            
            SavingStatus = SavingStatus.None;
            
            onLoad?.Invoke();
        }

        public static async Task AskForLoad()
        {
            await onAskForLoadBefore.InvokeAsync();
            await onAskForLoad.InvokeAsync();
            await onAskForLoadAfter.InvokeAsync();
        }

        public static async Task AskForSave()
        {
            await onAskForSaveBefore.InvokeAsync();
            await onAskForSave.InvokeAsync();
            await onAskForSaveAfter.InvokeAsync();
        }

        public static FileInfo GetLatestSaveFile()
        {
            var profilesPath = Directory.GetDirectories(CurrentRootSaveFolderPath);
            
            if (profilesPath.Length == 0) return null;
            
            var saveFiles = new List<FileInfo>();
            foreach (var profilePath in profilesPath) 
                saveFiles.AddRange(GetSaveFilesAtProfile(profilePath));

            return saveFiles.GetLatest();
        }
        
        public static FileInfo[] GetSaveFiles(string folderPath) =>
            SaveSystemHelpers.GetSaveFiles(folderPath);

        public static FileInfo[] GetSaveFilesAtProfile(int profileIndex)
        {
            var profilePath = $"{CurrentRootSaveFolderPath}/{PROFILE_FOLDER_NAME_PREFIX}{profileIndex}";
            return GetSaveFilesAtProfile(profilePath);
        }
        
        public static FileInfo[] GetSaveFilesAtProfile(string profilePath)
        {
            var autoSavesPath = $"{profilePath}/{AUTO_SAVE_FOLDER_NAME}";
            var quickSavesPath = $"{profilePath}/{QUICK_SAVE_FOLDER_NAME}";
            var manualSavesPath = $"{profilePath}/{MANUAL_SAVE_FOLDER_NAME}";
            
            var autoSaves = SaveSystemHelpers.GetSaveFiles(autoSavesPath);
            var quickSaves = SaveSystemHelpers.GetSaveFiles(quickSavesPath);
            var manualSaves = SaveSystemHelpers.GetSaveFiles(manualSavesPath);
            
            var allSaves = new List<FileInfo>();
            allSaves.AddRange(autoSaves);
            allSaves.AddRange(quickSaves);
            
            return allSaves.ToArray();
        }
        
        public static string GetFolderPath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            return path;
        }
    }
}
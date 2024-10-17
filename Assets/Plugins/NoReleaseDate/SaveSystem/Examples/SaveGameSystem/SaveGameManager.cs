using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Plugins.SaveSystem.DataStructure;
using SaveSystem;
using SaveSystem.Runtime.Extensions;
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
        public static UnityEvent OnQuickSave { get; } = new();
        public static UnityEvent OnAutoSave { get; } = new();
        public static UnityEvent OnManualSave { get; } = new();
        public static UnityEvent OnLoad { get; } = new();
        public static UnityEvent<SavingStatus> OnSavingStatusChange { get; } = new();
        public static UnityEvent<SavableMonoBehaviour, ClassData, ClassData> OnSavableBehaviourSaveBefore { get; } = new();
        public static UnityEvent<SavableMonoBehaviour, ClassData, ClassData> OnSavableBehaviourSaveAfter { get; } = new();
        public static UnityEvent<SavableMonoBehaviour, ClassData, ClassData> OnSavableBehaviourLoadBefore { get; } = new();
        public static UnityEvent<SavableMonoBehaviour, ClassData, ClassData> OnSavableBehaviourLoadAfter { get; } = new();
        public static event Action OnAskForLoadBefore;
        public static event Action OnAskForLoad;
        public static event Action OnAskForLoadAfter;
        public static event Action OnAskForSaveBefore;
        public static event Action OnAskForSave;
        public static event Action OnAskForSaveAfter;
        
        public static SaveData CurrentSaveData { get; private set; } = new SaveData();
        public static int CurrentSaveProfileIndex { get; private set; }

        public static SavingStatus SavingStatus
        {
            get => _savingStatus;
            private set
            {
                if (_savingStatus == value) return;
                
                _savingStatus = value;
                
                OnSavingStatusChange?.Invoke(_savingStatus);
            }
        }

        private static SavingStatus _savingStatus = SavingStatus.None;

        public static string SaveFolderPath =>
            SavePath.GetFolderPath($"{Application.persistentDataPath}/{SAVES_FOLDER_NAME}");

        public static string CurrentProfileFolderPath =>
            SavePath.GetFolderPath($"{SaveFolderPath}/{PROFILE_FOLDER_NAME_PREFIX}{CurrentSaveProfileIndex}");

        public static string CurrentQuickSaveFolderPath =>
            SavePath.GetFolderPath($"{CurrentProfileFolderPath}/{QUICK_SAVE_FOLDER_NAME}");

        public static string CurrentAutoSaveFolderPath =>
            SavePath.GetFolderPath($"{CurrentProfileFolderPath}/{AUTO_SAVE_FOLDER_NAME}");

        public static string CurrentManualSaveFolderPath => 
            SavePath.GetFolderPath($"{CurrentProfileFolderPath}/{MANUAL_SAVE_FOLDER_NAME}");
        
        private const int PROFILE_LIMIT = 5;
        private const int QUICK_SAVES_LIMIT = 50;
        private const int AUTO_SAVES_LIMIT = 100;
        private const int MANUAL_SAVES_LIMIT = 10;

        private const string SAVES_FOLDER_NAME = "Saves";
        private const string PROFILE_FOLDER_NAME_PREFIX = "Profile_";
        private const string QUICK_SAVE_FOLDER_NAME = "QuickSaves";
        private const string AUTO_SAVE_FOLDER_NAME = "AutoSaves";
        private const string MANUAL_SAVE_FOLDER_NAME = "ManualSaves";

        public static void SaveToSaveData(SavableMonoBehaviour savableMonoBehaviour, bool debug = false)
        {
            var saveKey = savableMonoBehaviour.SaveKey;
            var comment = savableMonoBehaviour.SaveKey.Comment.value;
            var defDataToSave = savableMonoBehaviour.DefSaveData;
            var dataToSave = savableMonoBehaviour.DataToSave;
            var previousSavedData = CurrentSaveData.GetKey(saveKey, defDataToSave);

            OnSavableBehaviourSaveBefore?.Invoke(savableMonoBehaviour, previousSavedData, dataToSave);
            savableMonoBehaviour.OnBeforeSave?.Invoke(previousSavedData, dataToSave);
            
            CurrentSaveData.SetKey(saveKey, dataToSave, comment);
            
            savableMonoBehaviour.OnAfterSave?.Invoke(previousSavedData, dataToSave);
            OnSavableBehaviourSaveAfter?.Invoke(savableMonoBehaviour, previousSavedData, dataToSave);

            if (debug) 
                Debug.Log($"[SAVE-MANAGER] Saved: {saveKey}\n{string.Join(Environment.NewLine, dataToSave)}", savableMonoBehaviour);
        }
        
        public static void LoadFromSaveData(SavableMonoBehaviour savableMonoBehaviour, bool debug = false)
        {
            var saveKey = savableMonoBehaviour.SaveKey;
            var defDataToSave = savableMonoBehaviour.DefSaveData;
            var currentData = savableMonoBehaviour.DataToSave;
            var dataToLoad = CurrentSaveData.GetKey(saveKey, defDataToSave);
            
            OnSavableBehaviourLoadBefore?.Invoke(savableMonoBehaviour, currentData, dataToLoad);
            savableMonoBehaviour.OnBeforeLoad?.Invoke(currentData, dataToLoad);
            
            savableMonoBehaviour.OnLoad(dataToLoad);
            
            savableMonoBehaviour.OnAfterLoad?.Invoke(currentData, dataToLoad);
            OnSavableBehaviourLoadAfter?.Invoke(savableMonoBehaviour, currentData, dataToLoad);

            if (debug) 
                Debug.Log($"[SAVE-MANAGER] Loaded: {saveKey}\n{string.Join(Environment.NewLine, dataToLoad)}", savableMonoBehaviour);
        }

        public static async Task<string> AutoSave() => await SaveToFile(SaveType.AutoSave);
        
        public static async Task<string> QuickSave() => await SaveToFile(SaveType.QuickSave);
        
        public static async Task<string> ManualSave() => await SaveToFile(SaveType.ManualSave);

        private static async Task<string> SaveToFile(SaveType saveType)
        {
            SavingStatus = SavingStatus.SavingToFile;
            string json;
            
            switch (saveType)
            {
                case SaveType.QuickSave:
                    json = await CurrentSaveData.Save(CurrentQuickSaveFolderPath, QUICK_SAVES_LIMIT);
                    OnQuickSave?.Invoke();
                    break;
                case SaveType.AutoSave:
                    json = await CurrentSaveData.Save(CurrentAutoSaveFolderPath, AUTO_SAVES_LIMIT);
                    OnAutoSave?.Invoke();
                    break;
                case SaveType.ManualSave:
                    json = await CurrentSaveData.Save(CurrentManualSaveFolderPath, MANUAL_SAVES_LIMIT);
                    OnManualSave?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null);
            }
            
            SavingStatus = SavingStatus.None;
            return json;
        }

        public static async Task<SaveData> LoadFromLatestSave()
        {
            var latestSaveFile = GetLatestSaveFile();
            
            if (latestSaveFile == null) return CurrentSaveData;
            
            return await LoadFromFile(latestSaveFile.FullName);
        }

        public static async Task<SaveData> LoadFromFile(string filePath)
        {
            SavingStatus = SavingStatus.LoadingFromFile;

            await CurrentSaveData.Load(filePath);
            
            SavingStatus = SavingStatus.None;
            
            OnLoad?.Invoke();
            
            return CurrentSaveData;
        }

        public static async Task AskForLoad()
        {
            await OnAskForLoadBefore.InvokeAsync();
            await OnAskForLoad.InvokeAsync();
            await OnAskForLoadAfter.InvokeAsync();
        }

        public static async Task AskForSave()
        {
            await OnAskForSaveBefore.InvokeAsync();
            await OnAskForSave.InvokeAsync();
            await OnAskForSaveAfter.InvokeAsync();
        }

        public static FileInfo GetLatestSaveFile()
        {
            var profilesPath = Directory.GetDirectories(SaveFolderPath);
            
            if (profilesPath.Length == 0) return null;
            
            var saveFiles = new List<FileInfo>();
            foreach (var profilePath in profilesPath) 
                saveFiles.AddRange(GetSaveFilesAtProfile(profilePath));

            return saveFiles.GetLatest();
        }
        
        public static FileInfo[] GetSaveFiles(string folderPath) =>
            CurrentSaveData.GetSaveFiles(folderPath);

        public static FileInfo[] GetSaveFilesAtProfile(int profileIndex)
        {
            var profilePath = $"{SaveFolderPath}/{PROFILE_FOLDER_NAME_PREFIX}{profileIndex}";
            return GetSaveFilesAtProfile(profilePath);
        }
        
        public static FileInfo[] GetSaveFilesAtProfile(string profilePath)
        {
            var autoSavesPath = $"{profilePath}/{AUTO_SAVE_FOLDER_NAME}";
            var quickSavesPath = $"{profilePath}/{QUICK_SAVE_FOLDER_NAME}";
            var manualSavesPath = $"{profilePath}/{MANUAL_SAVE_FOLDER_NAME}";
            
            var autoSaves = CurrentSaveData.GetSaveFiles(autoSavesPath);
            var quickSaves = CurrentSaveData.GetSaveFiles(quickSavesPath);
            var manualSaves = CurrentSaveData.GetSaveFiles(manualSavesPath);
            
            var allSaves = new List<FileInfo>();
            allSaves.AddRange(autoSaves);
            allSaves.AddRange(quickSaves);
            
            return allSaves.ToArray();
        }
    }
}
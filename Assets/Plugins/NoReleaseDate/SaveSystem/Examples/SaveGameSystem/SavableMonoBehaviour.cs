using System;
using Plugins.SaveSystem.DataStructure;
using SaveSystem.Runtime.Serializable;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.SaveSystem.Examples.SaveGameSystem
{
    public abstract class SavableMonoBehaviour : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent<ClassData, ClassData> OnBeforeSave { get; private set; } = new();
        [field: SerializeField] public UnityEvent<ClassData, ClassData> OnAfterSave { get; private set; } = new();
        [field: SerializeField] public UnityEvent<ClassData, ClassData> OnBeforeLoad { get; private set; } = new();
        [field: SerializeField] public UnityEvent<ClassData, ClassData> OnAfterLoad { get; private set; } = new();
        
        [field: SerializeField] public virtual SaveKey SaveKey { get; set; } = SaveKey.RandomKey;
        public abstract ClassData DefSaveData { get; }
        public abstract ClassData DataToSave { get; }
        
        [SerializeField] private SaveLoadPlace _loadAt = SaveLoadPlace.Start;
        [SerializeField] private SaveLoadPlace _saveAt = SaveLoadPlace.OnDisable;
        [SerializeField] private bool _debug;

        protected virtual void OnValidate()
        {
            if (SaveKey.Key == SerializableGuid.Empty)
                SaveKey = SaveKey.RandomKey;
        }
        
        protected virtual void Awake()
        {
            SaveGameManager.OnAskForSave += SaveToSaveData;
            SaveGameManager.OnAskForLoad += LoadFromSaveData;
            
            HandleSaveLoadPlace(SaveLoadPlace.Awake);
        }
        
        protected virtual void OnEnable()
        {
            HandleSaveLoadPlace(SaveLoadPlace.OnEnable);
        }
        
        protected virtual void Start()
        {
            HandleSaveLoadPlace(SaveLoadPlace.Start);
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) return;
            
            HandleSaveLoadPlace(SaveLoadPlace.OnApplicationPause);
        }
        
        protected virtual void OnApplicationQuit()
        {
            HandleSaveLoadPlace(SaveLoadPlace.OnApplicationQuit);
        }

        protected virtual void OnDisable()
        {
            HandleSaveLoadPlace(SaveLoadPlace.OnDisable);
        }

        protected virtual void OnDestroy()
        {
            HandleSaveLoadPlace(SaveLoadPlace.OnDestroy);
            
            SaveGameManager.OnAskForSave -= SaveToSaveData;
            SaveGameManager.OnAskForLoad -= LoadFromSaveData;
        }

        public void SaveToSaveData() => 
            SaveGameManager.SaveToSaveData(this, _debug);

        public void LoadFromSaveData() => 
            SaveGameManager.LoadFromSaveData(this, _debug);

        public abstract void OnLoad(ClassData classData);

        private void HandleSaveLoadPlace(SaveLoadPlace currentPlace)
        {
            if (_saveAt == currentPlace)
                SaveToSaveData();

            if (_loadAt == currentPlace)
                LoadFromSaveData();
        }
    }
}
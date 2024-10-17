using System;
using Newtonsoft.Json;
using Plugins.SaveSystem.DataStructure;
using UnityEditor;
using UnityEngine;

namespace Plugins.SaveSystem.Examples.SaveGameSystem.Editor
{
    [CustomEditor(typeof(SavableMonoBehaviour), true), CanEditMultipleObjects]
    public class SavableMonoBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var savableMonoBehaviour = (SavableMonoBehaviour)target;
            var inspectorWidth = EditorGUIUtility.currentViewWidth - 25;

            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Controls", EditorStyles.largeLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("SaveToSaveData"))
                savableMonoBehaviour.SaveToSaveData();
            
            if (GUILayout.Button("LoadFromSaveData")) 
                savableMonoBehaviour.LoadFromSaveData();
            
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Clear Save Data"))
                SaveGameManagerMenuItems.ClearSaveData();
            
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = Application.isPlaying;
            
            if (GUILayout.Button("Ask For Save", GUILayout.Width(inspectorWidth / 2)))
                SaveGameManagerMenuItems.AskForSave();
            
            if (GUILayout.Button("Ask For Load", GUILayout.Width(inspectorWidth / 2)))
                SaveGameManagerMenuItems.AskForLoad();
            
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Auto Save")) 
                SaveGameManagerMenuItems.AutoSave();
            
            if (GUILayout.Button("Quick Save"))
                SaveGameManagerMenuItems.QuickSave();
            
            if (GUILayout.Button("Manual Save"))
                SaveGameManagerMenuItems.ManualSave();
            
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Load From Latest Save"))
                SaveGameManagerMenuItems.LoadFromLatestSave();
            
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("Validate Serialization"))
            {
                try
                {
                    var dataToSave = new ClassData(savableMonoBehaviour.DataToSave);
                    var defSaveData = new ClassData(savableMonoBehaviour.DefSaveData);
                
                    var dataToSaveJson = JsonConvert.SerializeObject(dataToSave);
                    var defSaveDataJson = JsonConvert.SerializeObject(defSaveData);
                    
                    Debug.Log($"[SAVABLE-MONO-BEHAVIOUR] Data To Save:\n{dataToSaveJson}", savableMonoBehaviour);
                    Debug.Log($"[SAVABLE-MONO-BEHAVIOUR] Def Save Data:\n{defSaveDataJson}", savableMonoBehaviour);
                    Debug.Log($"[SAVABLE-MONO-BEHAVIOUR] Serialization is valid!");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SAVABLE-MONO-BEHAVIOUR] {e.Message}", this);
                }
            }
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Open Save Folder", GUILayout.Width(inspectorWidth / 2))) 
                SaveGameManagerMenuItems.OpenSavesFolder();
            
            var isLastSaveFileAvailable = SaveGameManager.GetLatestSaveFile() != null;
            GUI.enabled = isLastSaveFileAvailable;
            if (GUILayout.Button("Open Last Save File", GUILayout.Width(inspectorWidth / 2)))
                SaveGameManagerMenuItems.OpenLastSaveFile();
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
        }
    }
}

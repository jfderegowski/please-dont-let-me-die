using System;
using NoReleaseDate.Plugins.SaveSystem.Examples.SaveGameSystem;
using NoReleaseDate.Plugins.SaveSystem.Runtime.Settings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NoReleaseDate.Plugins.SaveSystem
{
    public class TestSaveSettings : MonoBehaviour
    {
        public SaveSettings SaveSettings;
        
        [Button]
        private async void PerformTest(int saveCount, SaveType saveType)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            
            stopWatch.Start();
            
            switch (saveType)
            {
                case SaveType.QuickSave:
                    for (var i = 0; i < saveCount; i++) 
                        await SaveGameManager.QuickSave();
                    break;
                case SaveType.AutoSave:
                    for (var i = 0; i < saveCount; i++) 
                        await SaveGameManager.AutoSave();
                    break;
                case SaveType.ManualSave:
                    for (var i = 0; i < saveCount; i++)
                        await SaveGameManager.ManualSave();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null);
            }
            
            stopWatch.Stop();
            
            Debug.Log($"Time taken to perform {saveCount} {saveType} saves: {stopWatch.ElapsedMilliseconds}ms");
        }
    }
}
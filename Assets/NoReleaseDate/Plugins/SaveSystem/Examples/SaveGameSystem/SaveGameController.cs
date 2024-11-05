using UnityEngine;

namespace NoReleaseDate.Plugins.SaveSystem.Examples.SaveGameSystem
{
    public class SaveGameController : MonoBehaviour
    {
#if UNITY_EDITOR

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async void OnApplicationPlay()
        {
            Debug.Log($"[SAVE-MANAGER] Load From Latest Save");
            await SaveGameManager.LoadFromLatestSave();
        }

#endif

        private async void OnDestroy() => 
            await SaveGameManager.AutoSave();
    }
}
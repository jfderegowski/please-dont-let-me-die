using InputIcons;
using UnityEngine;
namespace InputIcons
{
    public class InputIconsManagerInitializer
    {
        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeInitialized()
        {
            InputIconsManagerSO.Instance.Initialize();
        }
    }
}

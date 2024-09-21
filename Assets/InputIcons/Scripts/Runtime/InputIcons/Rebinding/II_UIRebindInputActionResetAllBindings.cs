using UnityEngine;
using UnityEngine.InputSystem;

namespace InputIcons
{
    public class II_UIRebindInputActionResetAllBindings : MonoBehaviour
    {

        public InputActionAsset assetToReset;

        public void ResetBindings()
        {
            //InputIconsManagerSO.ResetAllBindings();
            InputIconsManagerSO.ResetBindingsOfAsset(assetToReset);
        }
    }
}

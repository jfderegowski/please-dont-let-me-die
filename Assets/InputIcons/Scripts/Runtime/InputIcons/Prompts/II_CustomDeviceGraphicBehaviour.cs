using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InputIcons
{
    public class II_CustomDeviceGraphicBehaviour : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
        private Image image => GetComponent<Image>();

        public Sprite keyboardSprite;
        public Sprite gamepadOverrideSprite;
        public Sprite nintendoSprite;
        public Sprite ps3Sprite;
        public Sprite ps4Sprite;
        public Sprite ps5Sprite;
        public Sprite xBoxSprite;

        public Sprite fallbackSprite; //if chosen sprite from above is null, use this instead

        private void OnEnable()
        {
            InputIconsManagerSO.onControlsChanged += DisplayCurrentDeviceSprite;
            DisplayCurrentDeviceSprite(InputIconsManagerSO.GetCurrentInputDevice());
        }

        private void OnDisable()
        {
            InputIconsManagerSO.onControlsChanged -= DisplayCurrentDeviceSprite;
        }

        public void DisplayCurrentDeviceSprite()
        {
            Sprite spriteToDisplay = GetSpriteByIconSet(InputIconSetConfiguratorSO.GetCurrentIconSet());
            DisplaySprite(spriteToDisplay);
        }

        private void DisplayCurrentDeviceSprite(InputDevice inputDevice)
        {
            Sprite spriteToDisplay = GetSpriteByInputDevice(inputDevice);
            DisplaySprite(spriteToDisplay);
        }

        private void DisplaySprite(Sprite spriteToDisplay)
        {
            if (spriteToDisplay == null)
                spriteToDisplay = fallbackSprite;

            if (spriteRenderer)
                spriteRenderer.sprite = spriteToDisplay;

            if (image)
                image.sprite = spriteToDisplay;
        }

        private Sprite GetSpriteByIconSet(InputIconSetBasicSO iconSet)
        {
            if (iconSet is InputIconSetKeyboardSO)
                return keyboardSprite;


            if (gamepadOverrideSprite != null)
                return gamepadOverrideSprite;

            InputIconSetConfiguratorSO configurator = InputIconSetConfiguratorSO.Instance;
            if (iconSet == configurator.ps3IconSet)
                return ps3Sprite;

            if (iconSet == configurator.ps4IconSet) return ps4Sprite;

            if (iconSet == configurator.ps5IconSet) return ps5Sprite;

            if (iconSet == configurator.switchIconSet) return nintendoSprite;

            if (iconSet == configurator.xBoxIconSet) return xBoxSprite;

            return fallbackSprite;
        }


        public Sprite GetSpriteByInputDevice(InputDevice device)
        {
            if (!(device is Gamepad)) //device is not a gamepad, return keyboard sprite
                return keyboardSprite;

            //handle gamepads
            if (gamepadOverrideSprite != null)
                return gamepadOverrideSprite;

            if (device is UnityEngine.InputSystem.XInput.XInputController)
            {
                return xBoxSprite;
            }

            //THE FOLLOWING REFERENCES MIGHT NOT BE AVAILABLE ON SOME PLATFORMS LIKE: LINUX, SWITCH, PS5, WEBGL
            //see also here: https://forum.unity.com/threads/linux-build-error-namespace-name-dualshock4gamepadhid-does-not-exist.1278962/
            //if you are developing for those platforms, comment the following problematic code out
            //and only use the below fallback code to detect which gamepad is being used
#if !UNITY_STANDALONE_LINUX && !UNITY_WEBGL && !PLATFORM_SWITCH
            if (device is UnityEngine.InputSystem.DualShock.DualShock3GamepadHID)
            {
                return ps3Sprite;
            }

            if (device is UnityEngine.InputSystem.DualShock.DualShock4GamepadHID)
            {
                return ps4Sprite;
            }

            if (device is UnityEngine.InputSystem.DualShock.DualSenseGamepadHID) //Input System 1.2.0 or higher required (package manager dropdown menu -> see other versions)
            {
                return ps5Sprite;
            }

            if (device is UnityEngine.InputSystem.Switch.SwitchProControllerHID)
            {
                return nintendoSprite;
            }
#endif

            if (device is UnityEngine.InputSystem.DualShock.DualShockGamepad)
            {
                return ps4Sprite;
            }

            //FALLBACK CODE TO DETECT DEVICE TYPE
            if (device.name.Contains("DualShock3"))
                return ps3Sprite;

            if (device.name.Contains("DualShock4"))
                return ps4Sprite;

            if (device.name.Contains("DualSense"))
                return ps5Sprite;

            if (device.name.Contains("ProController"))
                return nintendoSprite;

            return fallbackSprite;
        }
    }
}

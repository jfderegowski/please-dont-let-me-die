using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEditor;

namespace InputIcons
{
    [CreateAssetMenu(fileName = "Input Icon Set Configurator", menuName = "Input Icon Set/InputIconSetConfigurator", order = 503)]
    public class InputIconSetConfiguratorSO : ScriptableObject
    {

        public static InputIconSetConfiguratorSO instance;
        public static InputIconSetConfiguratorSO Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                else
                {
                    InputIconSetConfiguratorSO iconManager = Resources.Load("InputIcons/InputIconSetConfigurator") as InputIconSetConfiguratorSO;
                    if (iconManager)
                        instance = iconManager;

                    return instance;
                }
            }
            set => instance = value;
        }

        private static InputIconSetBasicSO currentIconSet;
        public delegate void OnIconSetUpdated();
        public static OnIconSetUpdated onIconSetUpdated;

        public InputIconSetBasicSO keyboardIconSet;
        public InputIconSetBasicSO ps3IconSet;
        public InputIconSetBasicSO ps4IconSet;
        public InputIconSetBasicSO ps5IconSet;
        public InputIconSetBasicSO switchIconSet;
        public InputIconSetBasicSO xBoxIconSet;

        private InputIconSetBasicSO last_ps3IconSet;
        private InputIconSetBasicSO last_ps4IconSet;
        private InputIconSetBasicSO last_ps5IconSet;
        private InputIconSetBasicSO last_switchIconSet;
        private InputIconSetBasicSO last_xBoxIconSet;
        private InputIconSetBasicSO last_overwriteIconSet;
        private InputIconSetBasicSO last_fallbackIconSet;

        public enum InputIconsDevice {Keyboard, PS3, PS4, PS5, Switch, XBox, Fallback};

        public InputIconSetBasicSO overwriteIconSet;

        public InputIconSetBasicSO fallbackGamepadIconSet;

        private InputIconSetBasicSO lastUsedGamepadIconSet;

        public DisconnectedSettings disconnectedDeviceSettings;

        //Helper function to display icon sets which just got assigned through the setup window (e.g. if user replaces XBox icon set with XBox_flat icon set)
        public InputIconSetBasicSO GetGuessedNewlyAssignedGamepadIconSet()
        {
            if (last_ps3IconSet != ps3IconSet)
                return ps3IconSet;

            if (last_ps4IconSet != ps4IconSet)
                return ps4IconSet;

            if (last_ps5IconSet != ps5IconSet)
                return ps5IconSet;

            if (last_switchIconSet != switchIconSet)
                return switchIconSet;

            if (last_xBoxIconSet != xBoxIconSet)
                return xBoxIconSet;

            if (last_overwriteIconSet != overwriteIconSet)
                return overwriteIconSet;

            if (last_fallbackIconSet != fallbackGamepadIconSet)
                return fallbackGamepadIconSet;

            return null;
        }

        //Helper function to help detect which icon set got replaced in the setup window. Otherwise there is no way to actually detect changed icon sets
        public void RememberAssignedGamepadIconSets()
        {
            last_ps3IconSet = ps3IconSet;
            last_ps4IconSet = ps4IconSet;
            last_ps5IconSet = ps5IconSet;
            last_switchIconSet = switchIconSet;
            last_xBoxIconSet = xBoxIconSet;
            last_overwriteIconSet = overwriteIconSet;
            last_fallbackIconSet = fallbackGamepadIconSet;
        }

        private void Awake()
        {
            instance = this;
        }

        [System.Serializable]
        public struct DeviceSet
        {
            public string deviceRawPath;
            public InputIconSetBasicSO iconSetSO;
        }

        [System.Serializable]
        public struct DisconnectedSettings
        {
            public string disconnectedDisplayName;
            public Color disconnectedDisplayColor;
        }

        public static void UpdateCurrentIconSet(InputDevice device)
        {
            currentIconSet = GetIconSet(device);
            SetCurrentIconSet(currentIconSet);
            //Debug.Log("icon set updated: " + currentIconSet.iconSetName);
            onIconSetUpdated?.Invoke();
        }

        public static void SetCurrentIconSet(InputIconsDevice iconSet)
        {
            if (iconSet == InputIconsDevice.Keyboard)
                currentIconSet = Instance.keyboardIconSet;

            if (iconSet == InputIconsDevice.PS3)
                currentIconSet = Instance.ps3IconSet;

            if (iconSet == InputIconsDevice.PS4)
                currentIconSet = Instance.ps4IconSet;

            if (iconSet == InputIconsDevice.PS5)
                currentIconSet = Instance.ps5IconSet;

            if (iconSet == InputIconsDevice.Switch)
                currentIconSet = Instance.switchIconSet;

            if (iconSet == InputIconsDevice.XBox)
                currentIconSet = Instance.xBoxIconSet;

            if (iconSet == InputIconsDevice.Fallback)
                currentIconSet = Instance.fallbackGamepadIconSet;

            if(currentIconSet != Instance.keyboardIconSet)
                Instance.lastUsedGamepadIconSet = currentIconSet;
        }

        public static void SetCurrentIconSet(InputIconSetBasicSO iconSet)
        {
            if (iconSet == null)
                return;

            currentIconSet = iconSet;

            if(iconSet.GetType() == typeof(InputIconSetGamepadSO))
            {
                Instance.lastUsedGamepadIconSet = iconSet;
            }
        }

        public static InputIconSetBasicSO GetCurrentIconSet()
        {
            if (currentIconSet == null) UpdateCurrentIconSet(InputIconsManagerSO.GetCurrentInputDevice());


            return currentIconSet;
        }

        public static InputIconSetBasicSO GetIconSetOfDeviceID(int deviceID = 0)
        {
            InputDevice device = InputSystem.devices[deviceID];
            if (device == null) return null;

            return GetIconSet(device);
        }

        public static InputIconSetBasicSO GetLastUsedGamepadIconSet()
        {
            if (Instance.lastUsedGamepadIconSet == null)
                return Instance.fallbackGamepadIconSet;

            return Instance.lastUsedGamepadIconSet;
        }

        public static List<InputIconSetBasicSO> GetAllIconSetsOnConfigurator()
        {
            List<InputIconSetBasicSO> sets = new List<InputIconSetBasicSO>();

            InputIconSetConfiguratorSO configurator = Instance;
            if(configurator)
            {
                sets.Add(configurator.keyboardIconSet);
                sets.Add(configurator.ps3IconSet);
                sets.Add(configurator.ps4IconSet);
                sets.Add(configurator.ps5IconSet);
                sets.Add(configurator.switchIconSet);
                sets.Add(configurator.xBoxIconSet);

                sets.Add(configurator.overwriteIconSet);
                sets.Add(configurator.fallbackGamepadIconSet);
            }

            return sets;
        }

        public static InputIconSetBasicSO GetIconSet(InputDevice device)
        {

            if (device == null)
                return Instance.keyboardIconSet;

            //InputIconsLogger.Log(device.displayName+": "+device.GetType());

            if(device is Gamepad)
            {
                if (Instance.overwriteIconSet != null) //if overwriteIconSet is not null, this set will be used for all gamepads
                    return Instance.overwriteIconSet;

                if (device is UnityEngine.InputSystem.XInput.XInputController)
                {
                    return Instance.xBoxIconSet;
                }


                //THE FOLLOWING REFERENCES MIGHT NOT BE AVAILABLE ON SOME PLATFORMS LIKE: LINUX, SWITCH, PS5, WEBGL
                //see also here: https://forum.unity.com/threads/linux-build-error-namespace-name-dualshock4gamepadhid-does-not-exist.1278962/
                //if you are developing for those platforms, comment the following problematic code out
                //and only use the below fallback code to detect which gamepad is being used
#if !UNITY_STANDALONE_LINUX && !UNITY_WEBGL && !PLATFORM_SWITCH
                if (device is UnityEngine.InputSystem.DualShock.DualShock3GamepadHID)
                {
                    return Instance.ps3IconSet;
                }

                if (device is UnityEngine.InputSystem.DualShock.DualShock4GamepadHID)
                {
                    return Instance.ps4IconSet;
                }

                if (device is UnityEngine.InputSystem.DualShock.DualSenseGamepadHID) //Input System 1.2.0 or higher required (package manager dropdown menu -> see other versions)
                {
                    return Instance.ps5IconSet;
                }

                if (device is UnityEngine.InputSystem.Switch.SwitchProControllerHID)
                {
                    return Instance.switchIconSet;
                }
#endif

                if (device is UnityEngine.InputSystem.DualShock.DualShockGamepad)
                {
                    return Instance.ps4IconSet;
                }

                //FALLBACK CODE TO DETECT DEVICE TYPE
                if (device.name.Contains("DualShock3"))
                    return Instance.ps3IconSet;

                if (device.name.Contains("DualShock4"))
                    return Instance.ps4IconSet;

                if (device.name.Contains("DualSense"))
                    return Instance.ps5IconSet;

                if (device.name.Contains("ProController"))
                    return Instance.switchIconSet;
            }
           

            //in case it is none of the above gamepads, return fallback icons
            if(device is Gamepad)
            {
                return Instance.fallbackGamepadIconSet;
            }

            return Instance.keyboardIconSet;
        }

        public static InputIconSetBasicSO GetIconSet(string iconSetName)
        {
            List<InputIconSetBasicSO> sets = GetAllIconSetsOnConfigurator();
            for(int i=0; i<sets.Count; i++)
            {
                if (sets[i] == null)
                    continue;

                if(sets[i].iconSetName == iconSetName)
                    return sets[i];
            }

            InputIconsLogger.LogWarning("Icon Set not found: " + iconSetName);
            return null;
        }

        public static string GetCurrentDeviceName()
        {
            return GetCurrentIconSet().iconSetName;
        }

        public static Color GetCurrentDeviceColor()
        {
            return GetCurrentIconSet().deviceDisplayColor;
        }

        public static string GetDisconnectedName()
        {
            return Instance.disconnectedDeviceSettings.disconnectedDisplayName;
        }

        public static Color GetDisconnectedColor()
        {
            return Instance.disconnectedDeviceSettings.disconnectedDisplayColor;
        }


    }
}
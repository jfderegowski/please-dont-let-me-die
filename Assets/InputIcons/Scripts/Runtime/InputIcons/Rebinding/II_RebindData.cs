using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InputIcons.InputIconsUtility;

namespace InputIcons
{

    [System.Serializable]
    public class II_RebindData
    {

        /// <summary>
        /// Reference to the action that is to be rebound.
        /// </summary>
        [Tooltip("Reference to action that is to be rebound from the UI.")]
        public InputActionReference actionReference;
        public enum BindingSearchStrategy { BindingType, BindingIndex };

        public InputIconsUtility.DeviceType deviceType = InputIconsUtility.DeviceType.Auto;
        public BindingSearchStrategy bindingSearchStrategy = BindingSearchStrategy.BindingType;

        public int bindingIndexKeyboard = 0;
        public InputIconsUtility.CompositeType compositeTypeKeyboard = InputIconsUtility.CompositeType.Composite;
        public BindingType bindingTypeKeyboard = BindingType.Up;
        public int controlSchemeIndexKeyboard = 0;
        public int bindingIDInAvailableListKeyboard = 0;
        public InputIconSetKeyboardSO optionalKeyboardIconSet;


        public int bindingIndexGamepad = 0;
        public InputIconsUtility.CompositeType compositeTypeGamepad = InputIconsUtility.CompositeType.Composite;
        public BindingType bindingTypeGamepad = BindingType.Up;
        public int controlSchemeIndexGamepad = 0;
        public int bindingIDInAvailableListGamepad = 0;
        public InputIconSetGamepadSO optionalGamepadIconSet;


        [SerializeField]
        public InputBindingComposite part;


        private string previousBinding;



        private static InputActionRebindingExtensions.RebindingOperation rebindOperation;

        public delegate void OnRebindOperationCompleted(II_UIRebindInputActionImageBehaviour rebindBehaviour);
        public static OnRebindOperationCompleted onRebindOperationCompleted;

        //subscribe to react and display a message like "Key already bound to Jump action" for example
        public static UnityEvent<InputBinding> duplicateBindingFoundOnRebindOperation;

        public bool canBeRebound = true;
        public bool ignoreOtherButtons = false;

        [Header("UI Display - Action Text")]
        public TextMeshProUGUI actionNameDisplayText;

        [Header("UI Display - Binding Image")]
        public Image bindingDisplayImage;
        //public TextMeshProUGUI bindingNameDisplayText;

        [Header("UI Display - Buttons")]
        public GameObject rebindButtonObject;
        public GameObject resetButtonObject;

        [Header("UI Display - Listening Text")]
        public GameObject listeningForInputObject;

        [Header("UI Display - Key Already Used")]
        public GameObject keyAlreadyUsedObject;

        public string keyboardCancelKey = "<Keyboard>/escape";
        public string gamepadCancelKey = "<Gamepad>/start";

        private static InputDevice deviceToRebind;

        public II_RebindData()
        {

        }

        public static II_RebindData Clone(II_RebindData original)
        {
            Type type = original.GetType();


            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            II_RebindData createdRebindBehaviour = new II_RebindData();

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(original);

                field.SetValue(createdRebindBehaviour, value);
            }

            return createdRebindBehaviour;
        }

        public string GetBindingOverride()
        {
            return actionReference.action.bindings[GetMyBindingIndex()].overridePath;
        }

        public int GetMyBindingIndex()
        {
            InputIconSetBasicSO currentIconSet = InputIconSetConfiguratorSO.GetCurrentIconSet();
            InputDevice device = InputIconsManagerSO.GetCurrentInputDevice();

            if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
            {
                string controlSchemeName = GetActiveDeviceString();
                BindingType bindingType = bindingTypeKeyboard;
                int bindingIndex = bindingIDInAvailableListKeyboard;

                if (deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse || (deviceType == InputIconsUtility.DeviceType.Auto && device is Keyboard))
                    controlSchemeName = InputIconsManagerSO.GetKeyboardControlSchemeName(controlSchemeIndexKeyboard);

                if (deviceType == InputIconsUtility.DeviceType.Gamepad || (deviceType == InputIconsUtility.DeviceType.Auto && device is Gamepad))
                {
                    controlSchemeName = InputIconsManagerSO.GetGamepadControlSchemeName(controlSchemeIndexGamepad);
                    bindingType = bindingTypeGamepad;
                    bindingIndex = bindingIDInAvailableListGamepad;
                }


                int index = GetIndexOfBindingType(actionReference.action, bindingType, controlSchemeName, bindingIndex);
                return index;
            }
            else
            {
                if (deviceType == InputIconsUtility.DeviceType.Gamepad
                   || (deviceType == InputIconsUtility.DeviceType.Auto && currentIconSet is InputIconSetGamepadSO))
                {
                    return bindingIndexGamepad;
                }
                else
                    return bindingIndexKeyboard;
            }

        }

        public int GetNumberOfAvailableBindings()
        {
            string constrolScheme = InputIconsManagerSO.GetKeyboardControlSchemeName(controlSchemeIndexKeyboard);
            if (deviceType == InputIconsUtility.DeviceType.Gamepad)
            {
                constrolScheme = InputIconsManagerSO.GetGamepadControlSchemeName(controlSchemeIndexGamepad);
            }

            if (deviceType == InputIconsUtility.DeviceType.Auto)
            {
                if (!InputIconsManagerSO.DeviceIsKeyboardOrMouse(InputIconsManagerSO.GetCurrentInputDevice()))
                {
                    constrolScheme = InputIconsManagerSO.GetGamepadControlSchemeName(controlSchemeIndexGamepad);
                }
            }

            CompositeType compositeType = CompositeType.NonComposite;
            if (InputIconsUtility.ActionIsComposite(actionReference))
                compositeType = CompositeType.Composite;

            return InputIconsUtility.GetIndexesOfBindingTypeDeviceSpecific(actionReference, compositeType, bindingTypeKeyboard, constrolScheme).Count - 1;
        }



        public HashSet<InputIconsUtility.CompositeType> GetAvailableCompositeTypesOfAction(InputAction inputAction, bool gamepad)
        {
            string controlSchemeName = II_SpritePrompt.SpritePromptData.GetKeyboardControlSchemeNameAdvanced(controlSchemeIndexKeyboard);
            if (gamepad)
                controlSchemeName = II_SpritePrompt.SpritePromptData.GetGamepadControlSchemeNameAdvanced(controlSchemeIndexGamepad);

            return InputIconsUtility.GetAvailableCompositeTypesOfAction(inputAction, controlSchemeName);
        }

        public string GetControlSchemeNameKeyboard()
        {
            return InputIconsManagerSO.GetKeyboardControlSchemeName(controlSchemeIndexKeyboard).ToString();
        }

        public string GetControlSchemeNameGamepad()
        {
            return InputIconsManagerSO.GetGamepadControlSchemeName(controlSchemeIndexGamepad).ToString();
        }

        public int GetNumberOfAvailableBindingGroupsKeyboard(InputAction inputAction)
        {
            return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeTypeKeyboard, GetControlSchemeNameKeyboard()).Count;
        }

        public int GetNumberOfAvailableBindingGroupsGamepad(InputAction inputAction)
        {
            return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeTypeGamepad, GetControlSchemeNameGamepad()).Count;
        }



        private int GetRestrictedBindingIDInAvailableListKeyboard()
        {
            if (actionReference == null)
                return 0;

            int maxID = Mathf.Max(0, GetNumberOfAvailableBindingGroupsKeyboard(actionReference) - 1);
            return Mathf.Clamp(bindingIDInAvailableListKeyboard, 0, maxID);
        }

        private int GetRestrictedBindingIDInAvailableListGamepad()
        {
            if (actionReference == null)
                return 0;

            int maxID = Mathf.Max(0, GetNumberOfAvailableBindingGroupsGamepad(actionReference) - 1);
            return Mathf.Clamp(bindingIDInAvailableListGamepad, 0, maxID);
        }

        public string GetKeyNameKeyboard()
        {
            if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
            {
                int listIDToUse = GetRestrictedBindingIDInAvailableListKeyboard();
                string controlScheme =  InputIconsManagerSO.GetKeyboardControlSchemeName(controlSchemeIndexKeyboard);
                string keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeKeyboard, bindingTypeKeyboard, controlScheme, listIDToUse);
                return keyName;
            }
            else
            {
                string keyName = InputIconsUtility.GetSpriteName(actionReference, bindingIndexKeyboard, false);
                return keyName;
            }

        }

        public string GetKeyNameGamepad()
        {
            if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
            {
                int listIDToUse = GetRestrictedBindingIDInAvailableListGamepad();
                string controlScheme = InputIconsManagerSO.GetGamepadControlSchemeName(controlSchemeIndexGamepad);
                string keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeGamepad, bindingTypeGamepad, controlScheme, listIDToUse);
                return keyName;
            }
            else
            {
                string keyName = InputIconsUtility.GetSpriteName(actionReference, bindingIndexGamepad, true);
                return keyName;
            }
        }

        public Sprite GetKeySpriteByBindingIndexKeyboard()
        {
            return InputIconsUtility.GetSpriteByBindingIndex(actionReference.action, bindingIndexKeyboard);
        }

        public Sprite GetKeySpriteByBindingIndexGamepad()
        {
            return InputIconsUtility.GetSpriteByBindingIndex(actionReference.action, bindingIndexGamepad);
        }

        public Sprite GetSpriteKeyboard()
        {
            string keyName = GetKeyNameKeyboard();
            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
            if (optionalKeyboardIconSet != null)
                iconSet = optionalKeyboardIconSet;

            return iconSet.GetSprite(keyName);
        }

        public Sprite GetSpriteGamepad()
        {
            string keyName = GetKeyNameGamepad();
            //Debug.Log(controlScheme +" __ "+ listIDToUse+ " __ "+ keyName);
            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
            if (optionalGamepadIconSet != null)
                iconSet = optionalGamepadIconSet;

            return iconSet.GetSprite(keyName);
        }
    }
}

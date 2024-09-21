using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputIcons
{
    public class II_SpritePrompt : MonoBehaviour
    {

        public List<SpritePromptData> spritePromptDatas = new List<SpritePromptData>();


        private void Start()
        {
            UpdateDisplayedSprites();
            InputIconsManagerSO.onBindingsChanged += UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged += UpdateDisplayedSprites;
        }

        private void OnDestroy()
        {
            InputIconsManagerSO.onBindingsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged -= UpdateDisplayedSprites;
        }

        public void UpdateDisplayedSprites()
        {
            foreach(SpritePromptData s in spritePromptDatas)
            {
                s.UpdateDisplayedSprite();
            }

        }
        private void UpdateDisplayedSprites(InputDevice inputDevice)
        {
            UpdateDisplayedSprites();
        }


#if UNITY_EDITOR
        public void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

        private void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            if (this == null) return;
            foreach(SpritePromptData s in spritePromptDatas)
            {
                s.ValidatePromptData();
            }
            UpdateDisplayedSprites();
        }
#endif

        public class II_PromptData
        {
            public InputActionReference actionReference;
            public enum BindingSearchStrategy { BindingType, BindingIndex };
            public BindingSearchStrategy bindingSearchStrategy = BindingSearchStrategy.BindingType;

            public InputIconsUtility.CompositeType compositeType = InputIconsUtility.CompositeType.Composite;
            public InputIconsUtility.BindingType bindingType = InputIconsUtility.BindingType.Up;
            public InputIconsUtility.DeviceType deviceType = InputIconsUtility.DeviceType.Auto;

            public int bindingIndexKeyboard = 0;
            public int bindingIndexGamepad = 0;

            public int keyboardControlSchemeIndex = 0;
            public int gamepadControlSchemeIndex = 0;
            public int bindingIDInAvailableList = 0;

            public bool advancedMode = false;
            public InputIconsUtility.CompositeType compositeTypeKeyboardAdvanced = InputIconsUtility.CompositeType.Composite;
            public InputIconsUtility.CompositeType compositeTypeGamepadAdvanced = InputIconsUtility.CompositeType.Composite;
            public InputIconsUtility.BindingType bindingTypeKeyboardAdvanced = InputIconsUtility.BindingType.Up;
            public InputIconsUtility.BindingType bindingTypeGamepadAdvanced = InputIconsUtility.BindingType.Up;

            public int keyboardControlSchemeIndexAdvanced = 0;
            public int gamepadControlSchemeIndexAdvanced = 0;

            public int bindingIDInAvailableListKeyboardAdvanced = 0;
            public int bindingIDInAvailableListGamepadAdvanced = 0;


            public InputIconSetKeyboardSO optionalKeyboardIconSet;
            public InputIconSetGamepadSO optionalGamepadIconSet;

            public void ValidatePromptData()
            {
                if (actionReference == null)
                    return;

                //adjust keyboard composite type if necessary
                HashSet<InputIconsUtility.CompositeType> foundCompositeTypes = GetAvailableCompositeTypesOfActionAdvanced(actionReference, false);
                if (foundCompositeTypes.Count > 0)
                {
                    if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite)
                        && foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        //keep everything as is, composite type is user decision
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite))
                    {
                        compositeTypeKeyboardAdvanced = InputIconsUtility.CompositeType.NonComposite;
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        compositeTypeKeyboardAdvanced = InputIconsUtility.CompositeType.Composite;
                    }
                }

                //adjust gamepad composite type if necessary
                foundCompositeTypes = GetAvailableCompositeTypesOfActionAdvanced(actionReference, true);
                if (foundCompositeTypes.Count > 0)
                {
                    if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite)
                        && foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        //keep everything as is, composite type is user decision
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite))
                    {
                        compositeTypeGamepadAdvanced = InputIconsUtility.CompositeType.NonComposite;
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        compositeTypeGamepadAdvanced = InputIconsUtility.CompositeType.Composite;
                    }
                }
            }


            public II_PromptData()
            {

            }

            public II_PromptData(II_PromptData data)
            {
                if (data == null)
                    return;

                actionReference = data.actionReference;
                bindingSearchStrategy = data.bindingSearchStrategy;
                bindingType = data.bindingType;
                deviceType = data.deviceType;

                keyboardControlSchemeIndex = data.keyboardControlSchemeIndex;
                gamepadControlSchemeIndex = data.gamepadControlSchemeIndex;
                bindingIDInAvailableList = data.bindingIDInAvailableList;

                bindingIndexKeyboard = data.bindingIndexKeyboard;
                bindingIndexGamepad = data.bindingIndexGamepad;

                advancedMode = data.advancedMode;

                optionalKeyboardIconSet = data.optionalKeyboardIconSet;
                optionalGamepadIconSet = data.optionalGamepadIconSet;


                compositeTypeKeyboardAdvanced = data.compositeTypeKeyboardAdvanced;
                compositeTypeGamepadAdvanced = data.compositeTypeGamepadAdvanced;
                bindingTypeKeyboardAdvanced = data.bindingTypeKeyboardAdvanced;
                bindingTypeGamepadAdvanced = data.bindingTypeGamepadAdvanced;


                keyboardControlSchemeIndexAdvanced = data.keyboardControlSchemeIndexAdvanced;
                gamepadControlSchemeIndexAdvanced = data.gamepadControlSchemeIndexAdvanced;

                bindingIDInAvailableListKeyboardAdvanced = data.bindingIDInAvailableListKeyboardAdvanced;
                bindingIDInAvailableListGamepadAdvanced = data.bindingIDInAvailableListGamepadAdvanced;

        }

            public int GetRestrictedBindingIDInAvailableList()
            {
                if (actionReference == null)
                    return 0;

                int maxID = Mathf.Max(0, GetNumberOfAvailableBindingGroups(actionReference) - 1);
                return Mathf.Clamp(bindingIDInAvailableList, 0, maxID);
            }

            public int GetRestrictedBindingIDInAvailableListKeyboardAdvanced()
            {
                if (actionReference == null)
                    return 0;

                int maxID = Mathf.Max(0, GetNumberOfAvailableBindingGroupsKeyboardAdvanced(actionReference) - 1);
                return Mathf.Clamp(bindingIDInAvailableListKeyboardAdvanced, 0, maxID);
            }

            public int GetRestrictedBindingIDInAvailableListGamepadAdvanced()
            {
                if (actionReference == null)
                    return 0;

                int maxID = Mathf.Max(0, GetNumberOfAvailableBindingGroupsGamepadAdvanced(actionReference) - 1);
                return Mathf.Clamp(bindingIDInAvailableListGamepadAdvanced, 0, maxID);
            }


            public int GetNumberOfAvailableBindings(InputAction inputAction, bool gamepad)
            {
                if(!gamepad)
                    return InputIconsUtility.GetIndexesOfBindingTypeDeviceSpecific(inputAction, compositeType, bindingType, GetControlSchemeNameKeyboard()).Count;
                else
                    return InputIconsUtility.GetIndexesOfBindingTypeDeviceSpecific(inputAction, compositeType, bindingType, GetControlSchemeNameGamepad()).Count;
            }

            public int GetNumberOfAvailableBindingGroups(InputAction inputAction)
            {
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeType, GetControlSchemeName()).Count;
            }

            public int GetNumberOfAvailableBindingGroupsKeyboard(InputAction inputAction)
            {
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeType, GetControlSchemeNameKeyboard()).Count;
            }

            public int GetNumberOfAvailableBindingGroupsGamepad(InputAction inputAction)
            {
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeType, GetControlSchemeNameGamepad()).Count;
            }



            public int GetNumberOfAvailableBindingsAdvanced(InputAction inputAction)
            {
                return InputIconsUtility.GetIndexesOfBindingTypeDeviceSpecific(inputAction, compositeTypeKeyboardAdvanced, bindingTypeKeyboardAdvanced, GetControlSchemeNameAdvanced()).Count;
            }

            public int GetNumberOfAvailableBindingGroupsKeyboardAdvanced(InputAction inputAction)
            {
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeTypeKeyboardAdvanced, GetKeyboardControlSchemeNameAdvanced(keyboardControlSchemeIndexAdvanced)).Count;
            }

            public int GetNumberOfAvailableBindingGroupsGamepadAdvanced(InputAction inputAction)
            {
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeTypeGamepadAdvanced, GetGamepadControlSchemeNameAdvanced(gamepadControlSchemeIndexAdvanced)).Count;
            }

            public HashSet<InputIconsUtility.CompositeType> GetAvailableCompositeTypesOfActionAdvanced(InputAction inputAction, bool gamepad)
            {
                string controlSchemeName = GetKeyboardControlSchemeNameAdvanced(keyboardControlSchemeIndexAdvanced);
                if (gamepad)
                    controlSchemeName = GetGamepadControlSchemeNameAdvanced(gamepadControlSchemeIndexAdvanced);

                return InputIconsUtility.GetAvailableCompositeTypesOfAction(inputAction, controlSchemeName);
            }

            public HashSet<InputIconsUtility.CompositeType> GetAvailableCompositeTypesOfAction(InputAction inputAction, string controlSchemeName)
            {
                return InputIconsUtility.GetAvailableCompositeTypesOfAction(inputAction, controlSchemeName);
            }

            public void AutoassignControlSchemeIndexKeyboard()
            {
                foreach(InputBinding binding in actionReference.action.bindings)
                {
                    if(InputIconsManagerSO.InputBindingContainsKeyboardControlSchemes(binding))
                    {
                        keyboardControlSchemeIndex = InputIconsManagerSO.GetKeyboardControlSchemeID(binding.groups);
                        return;
                    }
                }
            }

            public void AutoassignControlSchemeIndexGamepad()
            {
                foreach (InputBinding binding in actionReference.action.bindings)
                {
                    if (InputIconsManagerSO.InputBindingContainsGamepadControlSchemes(binding))
                    {
                        gamepadControlSchemeIndex = InputIconsManagerSO.GetGamepadControlSchemeID(binding.groups);
                        return;
                    }
                }
            }

            public string GetControlSchemeName()
            {
                string controlScheme = "";
                string keyboardControlScheme = GetControlSchemeNameKeyboard();
                string gamepadControlScheme = GetControlSchemeNameGamepad();

                if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
                {
                    if (deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                    {
                        controlScheme = keyboardControlScheme;
                    }
                    else if (deviceType == InputIconsUtility.DeviceType.Gamepad)
                    {
                        controlScheme = gamepadControlScheme;
                    }
                    else
                    {
                        InputIconSetBasicSO currentIconSet = InputIconSetConfiguratorSO.GetCurrentIconSet();
                        controlScheme = gamepadControlScheme;
                        if (currentIconSet is InputIconSetKeyboardSO)
                            controlScheme = keyboardControlScheme;
                    }
                }
                return controlScheme;
            }

            public string GetControlSchemeNameKeyboard()
            {
                return InputIconsManagerSO.GetKeyboardControlSchemeName(keyboardControlSchemeIndex);
            }

            public string GetControlSchemeNameGamepad()
            {
                return InputIconsManagerSO.GetGamepadControlSchemeName(gamepadControlSchemeIndex);
            }


            private string GetControlSchemeNameAdvanced()
            {
                string controlScheme = "";
                string keyboardControlScheme = InputIconsManagerSO.GetKeyboardControlSchemeName(keyboardControlSchemeIndexAdvanced);
                string gamepadControlScheme = InputIconsManagerSO.GetGamepadControlSchemeName(gamepadControlSchemeIndexAdvanced);

                if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
                {
                    if (deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                    {
                        controlScheme = keyboardControlScheme;
                    }
                    else if (deviceType == InputIconsUtility.DeviceType.Gamepad)
                    {
                        controlScheme = gamepadControlScheme;
                    }
                    else
                    {
                        InputIconSetBasicSO currentIconSet = InputIconSetConfiguratorSO.GetCurrentIconSet();
                        controlScheme = gamepadControlScheme;
                        if (currentIconSet is InputIconSetKeyboardSO)
                            controlScheme = keyboardControlScheme;
                    }
                }
                return controlScheme;
            }

            public static string GetKeyboardControlSchemeNameAdvanced(int id)
            {
                return InputIconsManagerSO.GetKeyboardControlSchemeName(id);
            }

            public static string GetGamepadControlSchemeNameAdvanced(int id)
            {
                return InputIconsManagerSO.GetGamepadControlSchemeName(id);
            }

            

            public InputIconSetBasicSO GetIconSet()
            {
                InputIconSetBasicSO currentIconSet = InputIconSetConfiguratorSO.GetCurrentIconSet();

                if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
                {
                    if (deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                    {
                        currentIconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                    }
                    else if (deviceType == InputIconsUtility.DeviceType.Gamepad)
                    {
                        currentIconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();

                    
                    }
                }

                if (optionalKeyboardIconSet != null && currentIconSet is InputIconSetKeyboardSO)
                    currentIconSet = optionalKeyboardIconSet;

                if (optionalGamepadIconSet != null && currentIconSet is InputIconSetGamepadSO)
                    currentIconSet = optionalGamepadIconSet;

                return currentIconSet;
            }

            public Sprite GetKeySpriteByBindingIndexKeyboard()
            {
                return InputIconsUtility.GetSpriteByBindingIndex(actionReference.action, bindingIndexKeyboard, optionalKeyboardIconSet, optionalGamepadIconSet);
            }

            public Sprite GetSpriteByBindingIndexGamepad()
            {
                return InputIconsUtility.GetSpriteByBindingIndex(actionReference.action, bindingIndexGamepad, optionalKeyboardIconSet, optionalGamepadIconSet);
            }

            public Sprite GetKeySprite()
            {
                if (actionReference == null)
                    return null;


                string keyName = "";
                InputIconSetBasicSO currentIconSet = GetIconSet();

                if (currentIconSet is InputIconSetKeyboardSO && optionalKeyboardIconSet != null)
                    currentIconSet = optionalKeyboardIconSet;

                if (currentIconSet is InputIconSetGamepadSO && optionalGamepadIconSet != null)
                    currentIconSet = optionalGamepadIconSet;

                if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
                {
                    if(!advancedMode)
                    {
                        if (currentIconSet is InputIconSetKeyboardSO)
                            return GetKeySpriteKeyboard();
                        else
                            return GetKeySpriteGamepad();
                    }
                    else
                    {
                        if (currentIconSet is InputIconSetKeyboardSO)
                            return GetSpriteKeyboardAdvanced();
                        else
                            return GetSpriteGamepadAdvanced();
                    }
                }

                if (bindingSearchStrategy == BindingSearchStrategy.BindingIndex)
                {
                    if (deviceType == InputIconsUtility.DeviceType.Gamepad
                        || (deviceType == InputIconsUtility.DeviceType.Auto && currentIconSet is InputIconSetGamepadSO))
                    {
                        return GetSpriteByBindingIndexGamepad();
                    }
                    else
                    {
                        return GetKeySpriteByBindingIndexKeyboard();
                    }
        
                }

                
                if (currentIconSet is InputIconSetGamepadSO && optionalGamepadIconSet != null)
                    currentIconSet = optionalGamepadIconSet;

                if (currentIconSet is InputIconSetKeyboardSO && optionalKeyboardIconSet != null)
                    currentIconSet = optionalKeyboardIconSet;

                return currentIconSet.GetSprite(keyName);
            }


            public string GetKeyNameKeyboard()
            {
                if (InputIconsManagerSO.GetKeyboardControlSchemeCountOfAction(actionReference) == 1)
                    AutoassignControlSchemeIndexKeyboard();

                string controlScheme = GetControlSchemeNameKeyboard();

                int listIDToUse = GetRestrictedBindingIDInAvailableList();
                string keyName = InputIconsUtility.GetSpriteName(actionReference, compositeType, bindingType, controlScheme, listIDToUse);
                //Debug.Log("keyName: " + keyName);
                return keyName;
            }


            public string GetKeyNameKeyboardAdvanced()
            {
                if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
                {
                    int listIDToUse = GetRestrictedBindingIDInAvailableListKeyboardAdvanced();
                    string controlScheme = GetKeyboardControlSchemeNameAdvanced(keyboardControlSchemeIndexAdvanced);
                    string keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeKeyboardAdvanced, bindingTypeKeyboardAdvanced, controlScheme, listIDToUse);
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
                    if (InputIconsManagerSO.GetGamepadControlSchemeCountOfAction(actionReference) == 1)
                        AutoassignControlSchemeIndexGamepad();

                    string controlScheme = GetControlSchemeNameGamepad();
                    int listIDToUse = GetRestrictedBindingIDInAvailableList();
                    string keyName = InputIconsUtility.GetSpriteName(actionReference, compositeType, bindingType, controlScheme, listIDToUse);
                    return keyName;
                }
                else
                {
                    string keyName = InputIconsUtility.GetSpriteName(actionReference, bindingIndexKeyboard, true);
                    return keyName;
                }
            }


            public string GetKeyNameGamepadAdvanced()
            {
                if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
                {
                    int listIDToUse = GetRestrictedBindingIDInAvailableListGamepadAdvanced();
                    string controlScheme = GetGamepadControlSchemeNameAdvanced(gamepadControlSchemeIndexAdvanced);
                    string keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeGamepadAdvanced, bindingTypeGamepadAdvanced, controlScheme, listIDToUse);
                    return keyName;
                }
                else
                {
                    string keyName = InputIconsUtility.GetSpriteName(actionReference, bindingIndexGamepad, true);
                    return keyName;
                }
            }

            public Sprite GetKeySpriteKeyboard()
            {
                string keyName = GetKeyNameKeyboard();
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                if (optionalKeyboardIconSet != null)
                    iconSet = optionalKeyboardIconSet;

                return iconSet.GetSprite(keyName);
            }

            public Sprite GetKeySpriteGamepad()
            {
                string keyName = GetKeyNameGamepad();
                //Debug.Log(controlScheme +" __ "+ listIDToUse+ " __ "+ keyName);
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                if (optionalGamepadIconSet != null)
                    iconSet = optionalGamepadIconSet;

                return iconSet.GetSprite(keyName);
            }

            public Sprite GetSpriteKeyboardAdvanced()
            {
                string keyName = GetKeyNameKeyboardAdvanced();
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                if (optionalKeyboardIconSet != null)
                    iconSet = optionalKeyboardIconSet;

                return iconSet.GetSprite(keyName);
            }

            public Sprite GetSpriteGamepadAdvanced()
            {
                string keyName = GetKeyNameGamepadAdvanced();
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                if (optionalGamepadIconSet != null)
                    iconSet = optionalGamepadIconSet;

                return iconSet.GetSprite(keyName);
            }


        }

        [System.Serializable]
        public class SpritePromptData : II_PromptData
        {
           
            public SpriteRenderer spriteRenderer;

            public SpritePromptData() : base()
            {

            }

            public SpritePromptData(SpritePromptData data) : base(data)
            {
                if (data == null)
                    return;

                spriteRenderer = data.spriteRenderer;
            }

            public static List<SpritePromptData> CloneList(List<SpritePromptData> originalList)
            {
                if (originalList == null)
                    return null;

                List<SpritePromptData> clonedList = new List<SpritePromptData>();

                foreach (var item in originalList)
                {
                    // Create a new SpritePromptData object using the copy constructor
                    SpritePromptData clonedItem = new SpritePromptData(item);
                    clonedList.Add(clonedItem);
                }

                return clonedList;
            }

            public void UpdateDisplayedSprite()
            {
                if (actionReference == null)
                {
                    if (spriteRenderer != null)
                        spriteRenderer.sprite = null;
                    return;
                }


                if (spriteRenderer)
                    spriteRenderer.sprite = GetKeySprite();
            }

        }
    }
}


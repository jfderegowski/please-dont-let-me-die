using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputIcons
{
    public class II_LocalMultiplayerSpritePrompt : MonoBehaviour
    {

        public List<SpritePromptData> spritePromptDatas = new List<SpritePromptData>();

        private void Awake()
        {
            UpdateDisplayedSprites();
            InputIconsManagerSO.onBindingsChanged += UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged += UpdateDisplayedSprites;
            InputIconsManagerSO.onInputUsersChanged += UpdateDisplayedSprites;
        }

        private void OnDestroy()
        {
            InputIconsManagerSO.onBindingsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onInputUsersChanged -= UpdateDisplayedSprites;
        }

        public void UpdateDisplayedSprites()
        {
            //Debug.Log("update sprites");
            foreach (SpritePromptData s in spritePromptDatas)
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
            UpdateDisplayedSprites();
        }
#endif


        public class II_PromptDataLocalMultiplayer
        {
            public InputActionReference actionReference;
            public int playerID = 0;
            public enum BindingSearchStrategy { BindingType, BindingIndex };
            public InputIconsUtility.CompositeType compositeTypeKeyboard = InputIconsUtility.CompositeType.Composite;
            public InputIconsUtility.BindingType bindingTypeKeyboard = InputIconsUtility.BindingType.Up;

            public InputIconsUtility.CompositeType compositeTypeGamepad = InputIconsUtility.CompositeType.Composite;
            public InputIconsUtility.BindingType bindingTypeGamepad = InputIconsUtility.BindingType.Up;

            public int controlSchemeIndexKeyboard = 0;
            public int controlSchemeIndexGamepad = 0;

            public int bindingIndexKeyboard = 0;
            public int bindingIndexGamepad = 0;

            public InputIconSetKeyboardSO optionalKeyboardIconSet;
            public InputIconSetGamepadSO optionalGamepadIconSet;

            public II_PromptDataLocalMultiplayer()
            {

            }

            public II_PromptDataLocalMultiplayer(II_PromptDataLocalMultiplayer data)
            {
                if (data == null)
                    return;

                actionReference = data.actionReference;
                playerID = data.playerID;

                compositeTypeKeyboard = data.compositeTypeKeyboard;
                bindingTypeKeyboard = data.bindingTypeKeyboard;

                compositeTypeGamepad = data.compositeTypeGamepad;
                bindingTypeGamepad = data.bindingTypeGamepad;

                controlSchemeIndexKeyboard = data.controlSchemeIndexKeyboard;
                controlSchemeIndexGamepad = data.controlSchemeIndexGamepad;

                bindingIndexKeyboard = data.bindingIndexKeyboard;
                bindingIndexGamepad = data.bindingIndexGamepad;

                optionalKeyboardIconSet = data.optionalKeyboardIconSet;
                optionalGamepadIconSet = data.optionalGamepadIconSet;
            }

            public static List<II_LocalMultiplayerSpritePrompt.SpritePromptData> CloneList(List<II_LocalMultiplayerSpritePrompt.SpritePromptData> originalList)
            {
                if (originalList == null)
                    return null;

                List< II_LocalMultiplayerSpritePrompt.SpritePromptData> clonedList = new List< II_LocalMultiplayerSpritePrompt.SpritePromptData>();

                foreach (var item in originalList)
                {
                    // Create a new SpritePromptData object using the copy constructor
                    II_LocalMultiplayerSpritePrompt.SpritePromptData clonedItem = new  II_LocalMultiplayerSpritePrompt.SpritePromptData(item);
                    clonedList.Add(clonedItem);
                }

                return clonedList;
            }


            public HashSet<InputIconsUtility.CompositeType> GetAvailableCompositeTypesOfAction(InputAction inputAction, bool gamepad)
            {
                string controlSchemeName = GetControlSchemeNameKeyboard();
                if (gamepad)
                    controlSchemeName = GetControlSchemeNameGamepad();

                return InputIconsUtility.GetAvailableCompositeTypesOfAction(inputAction, controlSchemeName);
            }

            public void AutoassignControlSchemeIndexKeyboard()
            {
                foreach (InputBinding binding in actionReference.action.bindings)
                {
                    if (InputIconsManagerSO.InputBindingContainsKeyboardControlSchemes(binding))
                    {
                        controlSchemeIndexKeyboard = InputIconsManagerSO.GetKeyboardControlSchemeID(binding.groups);
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
                        controlSchemeIndexGamepad = InputIconsManagerSO.GetGamepadControlSchemeID(binding.groups);
                        return;
                    }
                }
            }

            public int GetNumberOfAvailableBindingGroupsKeyboard(InputAction inputAction)
            {
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeTypeKeyboard, GetControlSchemeNameKeyboard()).Count;
            }

            public int GetNumberOfAvailableBindingGroupsGamepad(InputAction inputAction)
            {
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeTypeGamepad, GetControlSchemeNameGamepad()).Count;
            }

            public string GetControlSchemeNameKeyboard()
            {
                return InputIconsManagerSO.GetKeyboardControlSchemeName(controlSchemeIndexKeyboard);
            }

            public string GetControlSchemeNameGamepad()
            {
                return InputIconsManagerSO.GetGamepadControlSchemeName(controlSchemeIndexGamepad);
            }

            public int GetNumberOfAvailableBindingGroups(InputAction inputAction, InputIconsUtility.CompositeType compositeType, string controlSchemeName)
            {
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(inputAction, compositeType, controlSchemeName).Count;
            }

            public string GetKeyNameKeyboard()
            {
                string controlScheme = GetControlSchemeNameKeyboard();
                int maxID = GetNumberOfAvailableBindingGroups(actionReference, compositeTypeKeyboard, GetControlSchemeNameKeyboard())-1;
                int listIDToUse = Mathf.Clamp(bindingIndexKeyboard, 0, maxID);
                string keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeKeyboard, bindingTypeKeyboard, controlScheme, listIDToUse);
                return keyName;
            }

            public Sprite GetSpriteKeyboard()
            {
                string keyName = GetKeyNameKeyboard();
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                if (optionalKeyboardIconSet != null)
                    iconSet = optionalKeyboardIconSet;
                return iconSet.GetSprite(keyName);
            }

            public string GetKeyNameGamepad()
            {
                string controlScheme = GetControlSchemeNameGamepad();
                int maxID = GetNumberOfAvailableBindingGroups(actionReference, compositeTypeGamepad, GetControlSchemeNameGamepad())-1;
                int listIDToUse = Mathf.Clamp(bindingIndexGamepad, 0, maxID);
                string keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeGamepad, bindingTypeGamepad, controlScheme, listIDToUse);
                return keyName;
            }

            public Sprite GetSpriteGamepad()
            {
                string keyName = GetKeyNameGamepad();

                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();

                InputDevice device = InputIconsManagerSO.localMultiplayerManagement.GetDeviceForPlayer(playerID);
                if (device != null)
                    if(device is Gamepad)
                        iconSet = InputIconSetConfiguratorSO.GetIconSet(device);
                

             
                if (optionalGamepadIconSet != null)
                    iconSet = optionalGamepadIconSet;
                return iconSet.GetSprite(keyName);
            }



            public Sprite GetKeySprite()
            {
                if (actionReference == null)
                    return null;

                InputDevice device = InputIconsManagerSO.localMultiplayerManagement.GetDeviceForPlayer(playerID);
                if (device == null)
                {
                    return InputIconSetConfiguratorSO.Instance.keyboardIconSet.unboundData.sprite;
                }

                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetIconSet(device);

                string chosenControlSchemeName = InputIconsManagerSO.localMultiplayerManagement.GetControlSchemeForPlayer(playerID);
                //Debug.Log("chosen control scheme: "+chosenControlSchemeName);
                if (chosenControlSchemeName == "")
                {
                    if (iconSet is InputIconSetKeyboardSO)
                        chosenControlSchemeName = GetControlSchemeNameKeyboard();
                    else
                        chosenControlSchemeName = GetControlSchemeNameGamepad();
                }

                string keyName = "";
                if(iconSet is  InputIconSetKeyboardSO)
                    keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeKeyboard, bindingTypeKeyboard, chosenControlSchemeName, bindingIndexKeyboard);

                if (iconSet is InputIconSetGamepadSO)
                    keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeGamepad, bindingTypeGamepad, chosenControlSchemeName, bindingIndexGamepad);


                return iconSet.GetSprite(keyName);
            }

           
        }

        [System.Serializable]
        public class SpritePromptData : II_PromptDataLocalMultiplayer
        {

            public SpritePromptData() : base()
            {

            }

            public SpritePromptData(SpritePromptData data) : base(data)
            {
                if (data == null)
                    return;

                spriteRenderer = data.spriteRenderer;
            }



            public SpriteRenderer spriteRenderer;

            public void UpdateDisplayedSprite()
            {
                if (actionReference == null)
                {
                    if (spriteRenderer != null)
                        spriteRenderer.sprite = null;
                    return;
                }


                if (spriteRenderer)
                {
                    Sprite aSprite = GetKeySprite();
                    if (aSprite != null)
                    {
                        spriteRenderer.sprite = aSprite;
                    }

                }

            }

        }
    }
}

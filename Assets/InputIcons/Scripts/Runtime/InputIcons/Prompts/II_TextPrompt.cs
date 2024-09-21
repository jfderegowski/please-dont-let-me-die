using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using static InputIcons.II_SpritePrompt;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;

namespace InputIcons
{
    public class II_TextPrompt : MonoBehaviour
    {

        public TMP_Text textComponent;

        [TextArea(6,12)]
        public string originalText = "";


        public List<TextPromptData> textPromptDatas = new List<TextPromptData>();

        private void Awake()
        {
            if (textComponent == null)
            {
                textComponent = GetComponent<TMP_Text>();
            }
        }

        private void Start()
        {
            //ApplyTextToPromptDatas();
            UpdateDisplayedSprites();
            InputIconsManagerSO.onBindingsChanged += UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged += UpdateDisplayedSprites;
        }

        private void OnDestroy()
        {
            InputIconsManagerSO.onBindingsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged -= UpdateDisplayedSprites;
        }

        public void SetText(string newText)
        {
            originalText = newText;
            UpdateDisplayedSprites();
        }

        public void SetTextPromptData(List<TextPromptData>  newTextPromptDatas)
        {
            textPromptDatas = newTextPromptDatas;
            UpdateDisplayedSprites();
        }

        public void SetTextPromptData(II_TextPromptDataSO textPromptDataSO)
        {
            SetTextPromptData(textPromptDataSO.textPromptDatas);
        }

        public void UpdateDisplayedSprites()
        {

            string outcome = originalText;
            if (outcome.Length < 1)
                return;

            bool usingFont = false;

            foreach (TextPromptData s in textPromptDatas)
            {
                try
                {
                    string spriteTag = s.GetSpriteTag();
                    outcome = ProcessCustomTags(outcome, spriteTag);

                    if (s.useSDFFont) usingFont = true;
                }
                catch (NoActionReferenceException)
                {
                    InputIconsLogger.LogWarning("No action reference on text prompt", gameObject);
                }
                catch (NoTextComponentException)
                {
                    InputIconsLogger.LogWarning("No text component on text prompt", gameObject);
                }
            }


            if (textComponent != null)
            {
                textComponent.text = outcome;

                foreach (TMP_SubMesh s in textComponent.GetComponentsInChildren<TMP_SubMesh>())
                {
                    if (s.gameObject != null)
                        DestroyImmediate(s.gameObject);
                }

                if (usingFont)
                {
                    //this would work, but unfotunately OnRebuildRequested is not available in build
                    //textComponent.OnRebuildRequested();

                }
            }

            
        }

        private string ProcessCustomTags(string currentText, string spriteTag)
        {
            string text = currentText;
            int startIndex = -1;

            // Find the second occurrence of the <inputaction> tag
            for (int i = 0; i < text.Length; i++)
            {
                if (text.IndexOf(InputIconsManagerSO.TEXT_TAG_VALUE, i) == i)
                {
                    startIndex = i;
                    break;
                }
            }

            // If the second occurrence is found, replace it
            if (startIndex != -1)
            {
                // Replace the second occurrence of <inputaction> tag with your desired content
                text = text.Substring(0, startIndex) + spriteTag + text.Substring(startIndex + InputIconsManagerSO.TEXT_TAG_VALUE.Length);

                // Update the text with modified string
                currentText = text;
            }

            return currentText;
        }

        public void ApplyTextToPromptDatas()
        {
            if (textComponent != null)
                originalText = textComponent.text;
        }

        private void UpdateDisplayedSprites(InputDevice inputDevice)
        {
            UpdateDisplayedSprites();
        }

        public bool CanMoveUp(int id)
        {
            if (id < 0)
                return false;

            if (id > textPromptDatas.Count - 1)
                return false;

            return true;
        }

        public bool CanMoveDown(int id)
        {
            if (id < 1)
                return false;

            if (id > textPromptDatas.Count - 2)
                return false;

            return true;
        }

        public void MoveDataUp(int id)
        {
            if (!CanMoveUp(id))
                return;

            if (id > 0)
            {
                TextPromptData data = textPromptDatas[id];
                // Swap the item with the previous one
                TextPromptData temp = textPromptDatas[id - 1];
                textPromptDatas[id - 1] = data;
                textPromptDatas[id] = temp;
            }
        }

        public void MoveDataDown(int id)
        {
            if (!CanMoveDown(id))
                return;

            if (id < textPromptDatas.Count - 1 && id != -1)
            {
                TextPromptData data = textPromptDatas[id];
                // Swap the item with the next one
                TextPromptData temp = textPromptDatas[id + 1];
                textPromptDatas[id + 1] = data;
                textPromptDatas[id] = temp;
            }
        }


#if UNITY_EDITOR
        public void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

        public void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            if (this == null) return;

            if (textComponent == null)
            {
                textComponent = GetComponent<TMP_Text>();
            }

            foreach (TextPromptData s in textPromptDatas)
            {
                s.ValidatePromptData();
            }

            UpdateDisplayedSprites();

        }
#endif



        [System.Serializable]
        public class TextPromptData : II_PromptData
        {

            public enum ActionDisplayType { SingleBinding, AllMatchingBindingsSingle, AllMatchingBindingsWithDelimiters };
            public ActionDisplayType actionDisplayType = ActionDisplayType.SingleBinding;

            public int bindingIDInAvailableListAllMatchingBindings = 0;

            public int bindingIDInAvailableListAllMatchingBindingsSingleKeyboard = 0;
            public int bindingIDInAvailableListAllMatchingBindingsSingleGamepad = 0;

            public int bindingIDInAvailableListAllMatchingBindingsWidthDelimiterKeyboard = 0;
            public int bindingIDInAvailableListAllMatchingBindingsWidthDelimiterGamepad = 0;

            public string delimiter = " or ";

            public bool allowTinting = false;
            public bool useSDFFont = false;

            public bool showAllInfo = true;

            public TextPromptData() : base()
            {

            }

            public TextPromptData(TextPromptData data) : base(data)
            {
                if (data == null)
                    return;

                actionDisplayType = data.actionDisplayType;
                bindingIDInAvailableListAllMatchingBindings = data.bindingIDInAvailableListAllMatchingBindings;

                bindingIDInAvailableListAllMatchingBindingsSingleKeyboard = data.bindingIDInAvailableListAllMatchingBindingsSingleKeyboard;
                bindingIDInAvailableListAllMatchingBindingsSingleGamepad = data.bindingIDInAvailableListAllMatchingBindingsSingleGamepad;

                bindingIDInAvailableListAllMatchingBindingsWidthDelimiterKeyboard = data.bindingIDInAvailableListAllMatchingBindingsWidthDelimiterKeyboard;
                bindingIDInAvailableListAllMatchingBindingsWidthDelimiterGamepad = data.bindingIDInAvailableListAllMatchingBindingsWidthDelimiterGamepad;

                delimiter = data.delimiter;

                allowTinting = data.allowTinting;
                useSDFFont = data.useSDFFont;

                showAllInfo = data.showAllInfo;
            }

            public static List<TextPromptData> CloneList(List<TextPromptData> originalList)
            {
                if (originalList == null)
                    return null;

                List<TextPromptData> clonedList = new List<TextPromptData>();

                foreach (var item in originalList)
                {
                    // Create a new SpritePromptData object using the copy constructor
                    TextPromptData clonedItem = new TextPromptData(item);
                    clonedList.Add(clonedItem);
                }

                return clonedList;
            }

            public int GetNumberOfAvailableBindingsAllMatchingBindings(InputAction inputAction)
            {
                return InputIconsUtility.GetNumberOfBindingsAllMatchingBindingsDeviceSpecific(inputAction, GetControlSchemeName());
            }

            public string GetSpriteTag()
            {
                if (actionReference == null)
                    return InputIconsManagerSO.TEXT_TAG_VALUE;

                string keyName = "";
                InputIconSetBasicSO currentIconSet = GetIconSet();
                string controlScheme = GetControlSchemeName();

                if (deviceType == InputIconsUtility.DeviceType.Gamepad)
                    currentIconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();

                if (deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                    currentIconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;


                if (currentIconSet is InputIconSetKeyboardSO && optionalKeyboardIconSet != null)
                    currentIconSet = optionalKeyboardIconSet;

                if (currentIconSet is InputIconSetGamepadSO && optionalGamepadIconSet != null)
                    currentIconSet = optionalGamepadIconSet;


                if (advancedMode)
                {
                    if (currentIconSet is InputIconSetKeyboardSO)
                        controlScheme = GetKeyboardControlSchemeNameAdvanced(keyboardControlSchemeIndexAdvanced);
                    else
                        controlScheme = GetGamepadControlSchemeNameAdvanced(gamepadControlSchemeIndexAdvanced);
                }
                else
                {
                    if (InputIconsManagerSO.GetKeyboardControlSchemeCountOfAction(actionReference) == 1)
                        AutoassignControlSchemeIndexKeyboard();

                    if (InputIconsManagerSO.GetGamepadControlSchemeCountOfAction(actionReference) == 1)
                        AutoassignControlSchemeIndexGamepad();

                    if (currentIconSet is InputIconSetKeyboardSO)
                        controlScheme = GetControlSchemeNameKeyboard();
                    else
                        controlScheme = GetControlSchemeNameGamepad();
                }


               
                if (actionDisplayType == ActionDisplayType.SingleBinding)
                {
                    if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
                    {
                        if (!advancedMode)
                        {
                            keyName = InputIconsUtility.GetSpriteName(actionReference, compositeType, bindingType, controlScheme, bindingIDInAvailableList);
                        }
                        else
                        {
                      
                            if (currentIconSet is InputIconSetKeyboardSO)
                                keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeKeyboardAdvanced, bindingTypeKeyboardAdvanced, controlScheme, bindingIDInAvailableListKeyboardAdvanced);
                            else
                                keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeGamepadAdvanced, bindingTypeGamepadAdvanced, controlScheme, bindingIDInAvailableListGamepadAdvanced);

                        }
                    }

                    if (bindingSearchStrategy == BindingSearchStrategy.BindingIndex)
                    {
                        if (deviceType == InputIconsUtility.DeviceType.Gamepad
                            || (deviceType == InputIconsUtility.DeviceType.Auto && currentIconSet is InputIconSetGamepadSO))
                        {
                            return GetSpriteTagByBindingIndexGamepad();
                        }
                        else
                        {
                            return GetSpriteTagByBindingIndexKeyboard();
                        }

                    }

                    if (!useSDFFont)
                        return currentIconSet.GetSpriteTag(keyName, allowTinting);
                    else
                        return currentIconSet.GetFontTag(keyName);

                }
                else if (actionDisplayType == ActionDisplayType.AllMatchingBindingsSingle)
                {
                    List<string> spriteNames = new List<string>();
                    if (!advancedMode)
                    {
                        spriteNames = InputIconsUtility.GetSpriteNamesAllSingle(actionReference.action, controlScheme, bindingIDInAvailableListAllMatchingBindings);
                    }
                    else
                    {
                        if (currentIconSet is InputIconSetKeyboardSO)
                            spriteNames = InputIconsUtility.GetSpriteNamesAllSingle(actionReference.action, controlScheme, bindingIDInAvailableListAllMatchingBindingsSingleKeyboard);
                        else
                            spriteNames = InputIconsUtility.GetSpriteNamesAllSingle(actionReference.action, controlScheme, bindingIDInAvailableListAllMatchingBindingsSingleGamepad);
                    }

                    if (!useSDFFont)
                        return currentIconSet.GetSpriteTag(spriteNames, allowTinting);
                    else
                        return currentIconSet.GetFontTag(spriteNames);
                }
                else
                {
                    //if (currentIconSet is InputIconSetKeyboardSO)
                    //    controlScheme = GetControlSchemeNameKeyboard();
                    //else
                    //    controlScheme = GetControlSchemeNameGamepad();
                    
                    List<List<string>> spriteNames = InputIconsUtility.GetSpriteNamesAll(actionReference.action, controlScheme);

                    if (!useSDFFont)
                        return currentIconSet.GetSpriteTag(spriteNames, allowTinting, delimiter);
                    else
                        return currentIconSet.GetFontTag(spriteNames, delimiter);

                }
            }

            public string GetSpriteTagByBindingIndexKeyboard()
            {
                return InputIconsUtility.GetSpriteTagByBindingIndex(actionReference.action, bindingIndexKeyboard, allowTinting, useSDFFont, optionalKeyboardIconSet, optionalGamepadIconSet);
            }

            public string GetSpriteTagByBindingIndexGamepad()
            {
                return InputIconsUtility.GetSpriteTagByBindingIndex(actionReference.action, bindingIndexGamepad, allowTinting, useSDFFont, optionalKeyboardIconSet, optionalGamepadIconSet);
            }

            public int GetGroupCountAllSpritesSingleKeyboard()
            {
                if (actionReference == null)
                    return 0;

                string controlScheme = GetKeyboardControlSchemeNameAdvanced(keyboardControlSchemeIndexAdvanced);
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(actionReference.action, controlScheme).Count;
            }

            public int GetGroupCountAllSpritesSingleGamepad()
            {
                if (actionReference == null)
                    return 0;

                string controlScheme = GetGamepadControlSchemeNameAdvanced(gamepadControlSchemeIndexAdvanced);
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(actionReference.action, controlScheme).Count;
            }

            public List<Sprite> GetSpritesKeyboardAllMatchingBindingsSingle(bool advanced)
            {
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                if (optionalKeyboardIconSet != null)
                    iconSet = optionalKeyboardIconSet;


                string controlScheme = GetKeyboardControlSchemeNameAdvanced(keyboardControlSchemeIndexAdvanced);
                int id = bindingIDInAvailableListAllMatchingBindingsSingleKeyboard;

                if (!advanced)
                {
                    controlScheme = GetControlSchemeNameKeyboard();
                    id = bindingIDInAvailableListAllMatchingBindings;
                }

                List<string> spriteNames = InputIconsUtility.GetSpriteNamesAllSingle(actionReference.action, controlScheme, id);
                return iconSet.GetSprites(spriteNames);
            }



            public List<Sprite> GetSpritesGamepadAllMatchingBindingsSingle(bool advanced)
            {
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                if (optionalGamepadIconSet != null)
                    iconSet = optionalGamepadIconSet;


                string controlScheme = GetGamepadControlSchemeNameAdvanced(gamepadControlSchemeIndexAdvanced);
                int id = bindingIDInAvailableListAllMatchingBindingsSingleGamepad;

                if (!advanced)
                {
                    controlScheme = GetControlSchemeNameGamepad();
                    id = bindingIDInAvailableListAllMatchingBindings;
                }

                List<string> spriteNames = InputIconsUtility.GetSpriteNamesAllSingle(actionReference.action, controlScheme, id);
                return iconSet.GetSprites(spriteNames);
            }



            public List<Sprite> GetSpritesKeyboardAllMatchingBindings(bool advanced)
            {
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                if (optionalKeyboardIconSet != null)
                    iconSet = optionalKeyboardIconSet;

                string controlScheme = GetKeyboardControlSchemeNameAdvanced(keyboardControlSchemeIndexAdvanced);

                if (!advanced)
                    controlScheme = GetControlSchemeNameKeyboard();


                List<List<string>> spriteNames = InputIconsUtility.GetSpriteNamesAll(actionReference.action, controlScheme);
                return iconSet.GetSprites(spriteNames);
            }

            public List<Sprite> GetSpritesGamepadAllMatchingBindings(bool advanced)
            {
                InputIconSetBasicSO iconSet =  InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                if (optionalGamepadIconSet != null)
                    iconSet = optionalGamepadIconSet;

                string controlScheme = GetGamepadControlSchemeNameAdvanced(gamepadControlSchemeIndexAdvanced);

                if (!advanced)
                    controlScheme = GetControlSchemeNameGamepad();


                List<List<string>> spriteNames = InputIconsUtility.GetSpriteNamesAll(actionReference.action, controlScheme);
                return iconSet.GetSprites(spriteNames);
            }
        }
    }

    [Serializable]
    internal class NoActionReferenceException : Exception
    {
        public NoActionReferenceException()
        {
        }

    }


    [Serializable]
    internal class NoTextComponentException : Exception
    {
        public NoTextComponentException()
        {
        }

    }
}

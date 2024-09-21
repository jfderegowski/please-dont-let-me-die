using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace InputIcons
{
    public class II_LocalMultiplayerTextPrompt : MonoBehaviour
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
            InputIconsManagerSO.onInputUsersChanged += UpdateDisplayedSprites;
        }

        private void OnDestroy()
        {
            InputIconsManagerSO.onBindingsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onInputUsersChanged -= UpdateDisplayedSprites;
        }

        public void SetText(string newText)
        {
            originalText = newText;
            UpdateDisplayedSprites();
        }

        public void SetTextPromptData(List<TextPromptData> newTextPromptDatas)
        {
            textPromptDatas = newTextPromptDatas;
            UpdateDisplayedSprites();
        }

        public void SetTextPromptData(II_LocalMultiplayerTextPromptDataSO textPromptDataSO)
        {
            SetTextPromptData(textPromptDataSO.textPromptDatas);
        }

        public void UpdateDisplayedSprites()
        {
            string outcome = originalText;
            if (outcome.Length < 3)
                return;

            foreach (TextPromptData s in textPromptDatas)
            {
                try
                {
                    string spriteTag = s.GetSpriteTag();
                    outcome = ProcessCustomTags(outcome, spriteTag);
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

                textComponent.ForceMeshUpdate();
                //textComponent.SetAllDirty();
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

        private void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            if (this == null) return;

            if (textComponent == null)
            {
                textComponent = GetComponent<TMP_Text>();
            }

            UpdateDisplayedSprites();

        }
#endif



        [System.Serializable]
        public class TextPromptData : II_LocalMultiplayerSpritePrompt.II_PromptDataLocalMultiplayer
        {

            public enum ActionDisplayType { SingleBinding, AllMatchingBindingsSingle, AllMatchingBindingsWithDelimiters };
            public ActionDisplayType actionDisplayType = ActionDisplayType.SingleBinding;


            public int bindingIDInAvailableListAllMatchingBindingsSingleKeyboard = 0;
            public int bindingIDInAvailableListAllMatchingBindingsSingleGamepad = 0;

            public int bindingIDInAvailableListAllMatchingBindingsWitdhDelimiterKeyboard = 0;
            public int bindingIDInAvailableListAllMatchingBindingsWidthDelimiterGamepad = 0;

            public string delimiter = " or ";

            public bool allowTinting = false;
            public bool useSDFFont = false;

            public bool showAllInfo  = true;

            public TextPromptData() : base()
            {

            }

            public TextPromptData(TextPromptData data) : base(data)
            {
                if (data == null)
                    return;

                actionDisplayType = data.actionDisplayType;

                bindingIDInAvailableListAllMatchingBindingsSingleKeyboard = data.bindingIDInAvailableListAllMatchingBindingsSingleKeyboard;
                bindingIDInAvailableListAllMatchingBindingsSingleGamepad = data.bindingIDInAvailableListAllMatchingBindingsSingleGamepad;

                bindingIDInAvailableListAllMatchingBindingsWitdhDelimiterKeyboard = data.bindingIDInAvailableListAllMatchingBindingsWitdhDelimiterKeyboard;
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
                    TextPromptData clonedItem = new TextPromptData(item);
                    clonedList.Add(clonedItem);
                }

                return clonedList;
            }

            public InputIconSetBasicSO GetMyIconSet()
            {
                InputDevice device = InputIconsManagerSO.localMultiplayerManagement.GetDeviceForPlayer(playerID);
                if (device == null)
                {
                    return InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                }

                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetIconSet(device);
                if (iconSet is InputIconSetKeyboardSO && optionalKeyboardIconSet != null)
                    iconSet = optionalKeyboardIconSet;

                if(iconSet is InputIconSetGamepadSO && optionalGamepadIconSet != null)
                    iconSet = optionalGamepadIconSet;

                return iconSet;
            }

            public new string GetControlSchemeNameKeyboard()
            {
                string controlScheme = InputIconsManagerSO.localMultiplayerManagement.GetControlSchemeForPlayer(playerID);
                if (controlScheme == "" || !Application.isPlaying)
                {
                    controlScheme = InputIconsManagerSO.GetKeyboardControlSchemeName(controlSchemeIndexKeyboard);
                }
                return controlScheme;
            }

            public new string GetControlSchemeNameGamepad()
            {
                string controlScheme = InputIconsManagerSO.localMultiplayerManagement.GetControlSchemeForPlayer(playerID);
                if (controlScheme == "" || !Application.isPlaying)
                {
                    controlScheme = InputIconsManagerSO.GetGamepadControlSchemeName(controlSchemeIndexGamepad);
                }
                return controlScheme;
            }

            public int GetGroupCountAllSpritesSingleKeyboard()
            {
                if (actionReference == null)
                    return 0;

                string controlScheme = base.GetControlSchemeNameKeyboard();
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(actionReference.action, controlScheme).Count;
            }

            public int GetGroupCountAllSpritesSingleGamepad()
            {
                if (actionReference == null)
                    return 0;

                string controlScheme = GetControlSchemeNameGamepad();
                return InputIconsUtility.GetGroupIndexesOfActionDeviceSpecific(actionReference.action, controlScheme).Count;
            }

            public int GetNumberOfAvailableBindingsAllMatchingBindings(InputAction inputAction)
            {
                return InputIconsUtility.GetNumberOfBindingsAllMatchingBindingsDeviceSpecific(inputAction, GetControlSchemeNameKeyboard());
            }

            public int GetRestrictedBindingIDInAvailableListKeyboard()
            {
                if (actionReference == null)
                    return 0;

                int output = bindingIndexKeyboard;
                Debug.Log("output: " + output);
                int maxID = GetNumberOfAvailableBindingsKeyboard(actionReference)-1;
                if (output > maxID)
                {
                    output = maxID;
                }
                if (output < 0)
                {
                    output = 0;
                }
                return output;
            }

            public int GetRestrictedBindingIDInAvailableListGamepad()
            {
                if (actionReference == null)
                    return 0;

                int output = bindingIndexGamepad;
                int maxID = GetNumberOfAvailableBindingsGamepad(actionReference)-1;
                if (output > maxID)
                {
                    output = maxID;
                }
                if (output < 0)
                {
                    output = 0;
                }
                return output;
            }

            public int GetNumberOfAvailableBindingsKeyboard(InputAction inputAction)
            {
                return InputIconsUtility.GetIndexesOfBindingTypeDeviceSpecific(inputAction, compositeTypeKeyboard, bindingTypeKeyboard, GetControlSchemeNameKeyboard()).Count;
            }

            public int GetNumberOfAvailableBindingsGamepad(InputAction inputAction)
            {
                return InputIconsUtility.GetIndexesOfBindingTypeDeviceSpecific(inputAction, compositeTypeGamepad, bindingTypeGamepad, GetControlSchemeNameGamepad()).Count;
            }

            public List<Sprite> GetSpritesKeyboardAllMatchingBindingsSingle()
            {
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                if (optionalKeyboardIconSet != null)
                    iconSet = optionalKeyboardIconSet;


                string controlScheme = base.GetControlSchemeNameKeyboard();
                int id = bindingIDInAvailableListAllMatchingBindingsSingleKeyboard;

                List<string> spriteNames = InputIconsUtility.GetSpriteNamesAllSingle(actionReference.action, controlScheme, id);
                return iconSet.GetSprites(spriteNames);
            }

            public List<Sprite> GetSpritesGamepadAllMatchingBindingsSingle()
            {
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                if (optionalGamepadIconSet != null)
                    iconSet = optionalGamepadIconSet;

                string controlScheme = GetControlSchemeNameGamepad();
                int id = bindingIDInAvailableListAllMatchingBindingsSingleGamepad;

                List<string> spriteNames = InputIconsUtility.GetSpriteNamesAllSingle(actionReference.action, controlScheme, id);
                return iconSet.GetSprites(spriteNames);
            }

            public List<Sprite> GetSpritesKeyboardAllMatchingBindings()
            {
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                if (optionalKeyboardIconSet != null)
                    iconSet = optionalKeyboardIconSet;

                string controlScheme = GetControlSchemeNameKeyboard();


                List<List<string>> spriteNames = InputIconsUtility.GetSpriteNamesAll(actionReference.action, controlScheme);
                return iconSet.GetSprites(spriteNames);
            }

            public List<Sprite> GetSpritesGamepadAllMatchingBindings()
            {
                InputIconSetBasicSO iconSet =  InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                if (optionalGamepadIconSet != null)
                    iconSet = optionalGamepadIconSet;

                string controlScheme = GetControlSchemeNameGamepad();


                List<List<string>> spriteNames = InputIconsUtility.GetSpriteNamesAll(actionReference.action, controlScheme);
                return iconSet.GetSprites(spriteNames);
            }

            public string GetSpriteTag()
            {
                if (actionReference == null)
                    return InputIconsManagerSO.TEXT_TAG_VALUE;

                string keyName = "";
                InputIconSetBasicSO myIconSet = GetMyIconSet();
                string controlScheme = GetControlSchemeNameKeyboard();

                if (myIconSet is InputIconSetKeyboardSO && optionalKeyboardIconSet != null)
                    myIconSet = optionalKeyboardIconSet;

                if (myIconSet is InputIconSetGamepadSO && optionalGamepadIconSet != null)
                    myIconSet = optionalGamepadIconSet;

                if (myIconSet is InputIconSetKeyboardSO)
                    controlScheme = GetControlSchemeNameKeyboard();
                else
                    controlScheme = GetControlSchemeNameGamepad();


                if (actionDisplayType == ActionDisplayType.SingleBinding)
                {

                    if (myIconSet is InputIconSetKeyboardSO)
                        keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeKeyboard, bindingTypeKeyboard, controlScheme, bindingIndexKeyboard);
                    else
                        keyName = InputIconsUtility.GetSpriteName(actionReference, compositeTypeGamepad, bindingTypeGamepad, controlScheme, bindingIndexGamepad);

                    if (!useSDFFont)
                        return myIconSet.GetSpriteTag(keyName, allowTinting);
                    else
                        return myIconSet.GetFontTag(keyName);

                }
                else if (actionDisplayType == ActionDisplayType.AllMatchingBindingsSingle)
                {
                    List<string> spriteNames = new List<string>();

                    if (myIconSet is InputIconSetKeyboardSO)
                        spriteNames = InputIconsUtility.GetSpriteNamesAllSingle(actionReference.action, controlScheme, bindingIDInAvailableListAllMatchingBindingsSingleKeyboard);
                    else
                        spriteNames = InputIconsUtility.GetSpriteNamesAllSingle(actionReference.action, controlScheme, bindingIDInAvailableListAllMatchingBindingsSingleGamepad);

                    if (!useSDFFont)
                        return myIconSet.GetSpriteTag(spriteNames, allowTinting);
                    else
                        return myIconSet.GetFontTag(spriteNames);
                }
                else
                {
                    List<List<string>> spriteNames = InputIconsUtility.GetSpriteNamesAll(actionReference.action, controlScheme);

                    if (!useSDFFont)
                        return myIconSet.GetSpriteTag(spriteNames, allowTinting, delimiter);
                    else
                        return myIconSet.GetFontTag(spriteNames, delimiter);
                }
            }
        }
    }
}
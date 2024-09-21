using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

namespace InputIcons
{
    public class InputIconsUtility : MonoBehaviour
    {
        //english, german and french layouts
        private static readonly HashSet<string> SupportedLayouts = new HashSet<string> { "00000409", "00000407", "0000040C" };
        private static Dictionary<InputBinding, string> cachedKeyboardStrings = new Dictionary<InputBinding, string>();


        [System.Serializable]
        public class InputStyleData
        {
            public bool isComposite = false;
            public bool isPartOfComposite = false;
            public string tmproReferenceText;  //<style=Controls/Action/compositePart>
            public string bindingName; //Controls/Action/compositePart
            public string inputStyleString;  //<sprite=....><sprite=...><sprite=...>...
            public string inputStyleString_singleInput;  //<sprite=....>
            public string humanReadableString; //WASD or Arrows
            public string humanReadableString_singleInput; //WASD
            public string fontReferenceText; ////<style=Font/Controls/Action/compositePart>
            public string fontCode; //E905 or E95B, ...
            public string fontCode_singleInput; //E905

            public InputStyleData()
            {
            }

            public InputStyleData(string bName, bool isComp, bool isPartOfComp, string tmproText, string style, string humanReadableStr, string fontC, string fontReference)
            {
                tmproReferenceText = tmproText;
                bindingName = bName;
                inputStyleString = style;
                humanReadableString = humanReadableStr;
                isComposite = isComp;
                isPartOfComposite = isPartOfComp;
                fontReferenceText = fontReference;
                fontCode = fontC;
            }
        }


        public enum CompositeType {Composite, NonComposite };

        public enum BindingType { Up, Down, Left, Right, Forward, Backward , Modifier, Modifier1, Modifier2, Binding, Positive, Negative};

        public enum DeviceType { Auto, KeyboardAndMouse, Gamepad };

        public static string GetStyleName(InputAction action)
        {
            return action.actionMap.name + "/" + action.name;
        }

        public static bool IsDifferentBindingGroup(InputAction action, InputBinding binding1, InputBinding binding2, string controlSchemeName)
        {
            if (action == null) return false;
            if (binding1 == null) return false;
            if (binding2 == null) return false;

            //If either one is not part of a composite, return true
            if (!binding1.isPartOfComposite || !binding2.isPartOfComposite)
                return true;


            bool bindingOneFound = false;
            InputBinding bindingOne = new InputBinding();
            InputBinding bindingTwo = new InputBinding();
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i] == binding1)
                {
                    if (!bindingOneFound)
                    {
                        bindingOne = action.bindings[i];
                        bindingOneFound = true;
                    }
                    else
                        bindingTwo = action.bindings[i];
                }

                if (action.bindings[i] == binding2)
                {
                    if (!bindingOneFound)
                    {
                        bindingOne = action.bindings[i];
                        bindingOneFound = true;
                    }
                    else
                        bindingTwo = action.bindings[i];
                }
            }

            bool bindingOnePassed = false;
            for (int i = 0; i < action.bindings.Count; i++)
            {
                //LogBindingInfo(action.bindings[i]);
                if (action.bindings[i] == bindingOne)
                {
                    bindingOnePassed = true;
                    continue;
                }

                if (bindingOnePassed)
                {
                    if (action.bindings[i] == binding2)
                    {
                        if (bindingOne.isPartOfComposite && bindingTwo.isPartOfComposite)
                            return false;
                        else
                            return true;
                    }

                    if (action.bindings[i] != bindingTwo && BindingGroupContainsControlScheme(action.bindings[i].groups, controlSchemeName))
                    {
                        if (action.bindings[i].isComposite || !action.bindings[i].isPartOfComposite)
                        {
                            return true;
                        }
                    }

                }
            }

            return false;
        }

        public static void LogBindingInfo(InputBinding binding)
        {
            Debug.Log("binding: " + binding.name + " ___ composite: " + binding.isComposite + " ___ partOfComposite:" + binding.isPartOfComposite);
        }

        public static int GetNumberOfBindingsAllMatchingBindingsDeviceSpecific(InputAction action, string controlSchemeName)
        {
            if (action == null) return 0;

            List<int> bindingIDs = new List<int>();
            InputBinding lastBinding = new InputBinding();
            int numberOfMatches = 1;
            bool needsDelimiter = false;
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (bindingIDs.Count > 0)
                {
                    if (IsDifferentBindingGroup(action, lastBinding, action.bindings[i], controlSchemeName))
                    {
                        needsDelimiter = true;
                    }
                }

                if (BindingGroupContainsControlScheme(action.bindings[i].groups, controlSchemeName))
                {

                    if (needsDelimiter)
                    {
                        bindingIDs.Add(-1);
                        needsDelimiter = false;
                        numberOfMatches++;
                    }
                    bindingIDs.Add(i);
                }

                lastBinding = action.bindings[i];
            }

            return numberOfMatches;
        }


      
        public static Sprite GetSpriteByBindingIndex(InputAction action, int bindingIndex, 
            InputIconSetKeyboardSO optionalKeyboardIconSet = null, InputIconSetGamepadSO optionalGamepadIconSet = null)
        {
            InputIconSetBasicSO currentIconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
            string keyName = GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(action, bindingIndex, false);

            if (currentIconSet is InputIconSetKeyboardSO && optionalKeyboardIconSet != null)
                currentIconSet = optionalKeyboardIconSet;

            if (currentIconSet.HasSprite(keyName))
                return currentIconSet.GetSprite(keyName);
            else
            {
                currentIconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                keyName = GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(action, bindingIndex, true);
                if (optionalGamepadIconSet != null)
                    currentIconSet = optionalGamepadIconSet;

                return currentIconSet.GetSprite(keyName);
            }
        }

      

        public static List<string> GetSpriteNamesAllSingle(InputAction action, string controlSchemeName, int idInList)
        {
            List<string> outputList = new List<string>();

            List<List<int>> indexesGroupList = GetGroupIndexesOfActionDeviceSpecific(action, controlSchemeName);
            bool gamepad = InputIconsManagerSO.IsGamepadControlScheme(controlSchemeName);

            for(int i=0; i<indexesGroupList.Count; i++)
            {
                if (i != idInList)
                    continue;

                //Debug.Log(controlSchemeName+ "___ group count: " + indexesGroupList.Count);
                for(int j=0; j < indexesGroupList[i].Count; j++)
                {
                    outputList.Add(GetSpriteName(action.bindings[indexesGroupList[i][j]], true, gamepad));
                    //Debug.Log("output list: " + outputList.Count);
                }
            }

            //Debug.Log("output list: "+outputList.Count);
            return outputList;
        }

        public static List<List<string>> GetSpriteNamesAll(InputAction action, string controlSchemeName)
        {
            List<List<string>> outputList = new List<List<string>>();

            List<List<int>> indexesGroupList = GetGroupIndexesOfActionDeviceSpecific(action, controlSchemeName);
            bool gamepad = InputIconsManagerSO.IsGamepadControlScheme(controlSchemeName);

            for (int i = 0; i < indexesGroupList.Count; i++)
            {
                outputList.Add(new List<string>());
                //Debug.Log(controlSchemeName+ "___ group count: " + indexesGroupList.Count);
                for (int j = 0; j < indexesGroupList[i].Count; j++)
                {
                    outputList[i].Add(GetSpriteName(action.bindings[indexesGroupList[i][j]], true, gamepad));
                    //Debug.Log("output list: " + outputList.Count);
                }
            }

            //Debug.Log("output list: "+outputList.Count);
            return outputList;
        }


        public static string GetSpriteName(InputBinding binding, string controlSchemeName, bool forOpeningTag)
        {
            if (binding == null) return "";

            string keyName = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            if (InputIconsManagerSO.IsKeyboardControlScheme(controlSchemeName))
            {
                keyName = GetKeyboardString(binding, forOpeningTag);
            }

            if (keyName == InputIconsManagerSO.Instance.textDisplayForUnboundActions)
                keyName = "";

            return keyName;
        }

        public static string GetSpriteName(InputBinding binding, bool forOpeningTag, bool gamepad = false)
        {
            if (binding == null) return "";

            string keyName = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            if (!gamepad)
            {
                keyName = GetKeyboardString(binding, forOpeningTag);

            }

            if (keyName == InputIconsManagerSO.Instance.textDisplayForUnboundActions)
                keyName = "";

            return keyName;
        }

        public static string GetSpriteName(InputAction action, int bindingIndex, bool gamepad)
        {
            if (action == null || bindingIndex < 0) return "";

            if (bindingIndex > action.bindings.Count - 1) return "";

            string keyName = InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

            if (!gamepad)
            {
                keyName = GetKeyboardString(action.bindings[bindingIndex], true);
            }

            if (keyName == InputIconsManagerSO.Instance.textDisplayForUnboundActions)
                keyName = "";

            return keyName;
        }


        public static string GetSpriteName(InputAction action, BindingType bindingType, string controlSchemeName, int bindingIDInList = 0)
        {
            int index = GetIndexOfBindingType(action, bindingType, controlSchemeName, bindingIDInList);
            if (index >= 0)
            {
                string keyName = GetSpriteName(action.bindings[index], controlSchemeName, true);

                if (keyName == InputIconsManagerSO.Instance.textDisplayForUnboundActions)
                    keyName = "";

                return keyName;
            }
            return "";
        }

        public static string GetSpriteName(InputAction action, CompositeType compositeType, BindingType bindingType, string controlSchemeName, int bindingIDInList = 0)
        {
            if (action == null) return "";

            //Debug.Log("action: " + action.name + " bindingType: " + bindingType.ToString() + " scheme: " + controlSchemeName + " bindingID " + bindingIDInList);
            int index = GetIndexOfBindingType(action, compositeType, bindingType, controlSchemeName, bindingIDInList);
            //Debug.Log("index: " + index);
            if (index >= 0)
            {
                string keyName = GetSpriteName(action.bindings[index], controlSchemeName, true);

                if (keyName == InputIconsManagerSO.Instance.textDisplayForUnboundActions)
                    keyName = "";

                return keyName;
            }
            return "";
        }


        public static string GetSpriteTagByBindingIndex(InputAction action, int bindingIndex, bool allowTinting, bool useFontTag,
            InputIconSetKeyboardSO optionalKeyboardIconSet = null, InputIconSetGamepadSO optionalGamepadIconSet = null)
        {
            if (action == null) return "";

            InputIconSetBasicSO currentIconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
            string keyName = GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(action, bindingIndex, false);

            if (currentIconSet is InputIconSetKeyboardSO && optionalKeyboardIconSet != null)
                currentIconSet = optionalKeyboardIconSet;

            if (!currentIconSet.HasSprite(keyName))
            {
                currentIconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                keyName = GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(action, bindingIndex, true);
                if (optionalGamepadIconSet != null)
                    currentIconSet = optionalGamepadIconSet;
            }
            

            if (!useFontTag)
                return currentIconSet.GetSpriteTag(keyName, allowTinting);
            else
                return currentIconSet.GetFontTag(keyName);
        }
        

        public static string GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(InputAction action, int bindingIndex, bool gamepad)
        {
            if (action == null) return "";

            string keyName = GetSpriteName(action.bindings[bindingIndex], true, gamepad);

            if (keyName == InputIconsManagerSO.Instance.textDisplayForUnboundActions)
                keyName = "";

            return keyName;
        }


        public static InputStyleData GetStyleOpeningTagOfComposite(InputAction action, InputBinding binding, string deviceDisplayName, string controlSchemeName)
        {
            if (!binding.isComposite)
            {
                InputIconsLogger.LogError("composite binding expected, but non composite found");
                return null;
            }

            InputStyleData styleData = new InputStyleData();
            string compositeDelimiter = InputIconsManagerSO.Instance.compositeInputDelimiter;

            string bindingName = action.actionMap.name + "/" + action.name;
            styleData.fontReferenceText = "<style=font/" + bindingName + ">";

            bool isCorrectComposite = false;
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].isComposite)
                {
                    //Debug.Log(action.bindings[i].name + " " + binding.name);
                    isCorrectComposite = action.bindings[i].name == binding.name;
                }

                if (!action.bindings[i].isComposite && isCorrectComposite && action.bindings[i].isPartOfComposite)
                {
                    //s += GetStyleOpeningTag(action, action.bindings[i], deviceDisplayName, controlSchemeName);

                    InputStyleData sPart = GetStyleOpeningTag(action, action.bindings[i], deviceDisplayName, controlSchemeName);
                    //                    InputStyleData sPart = new InputStyleData();

                    if (sPart == null)
                        continue;

                    if (styleData.bindingName == null)
                    {
                        string[] stringParts = sPart.bindingName.Split('/');
                        for (int n = 0; n < stringParts.Length - 1; n++)
                        {
                            styleData.bindingName += stringParts[n];
                            if (n < stringParts.Length - 2)
                                styleData.bindingName += "/";
                        }

                    }

                    styleData.isComposite = true;
                    styleData.humanReadableString += sPart.humanReadableString + compositeDelimiter;
                    styleData.inputStyleString += sPart.inputStyleString;
                    styleData.tmproReferenceText += sPart.tmproReferenceText + " ";
                    styleData.fontCode += sPart.fontCode;
                }


            }

            if (styleData.humanReadableString != null)
            {
                //Debug.Log(styleData.humanReadableString.ElementAt(styleData.humanReadableString.Length - 2));
                if (styleData.humanReadableString.ElementAt(styleData.humanReadableString.Length - 2) == ',')
                {
                    styleData.humanReadableString = styleData.humanReadableString.Remove(styleData.humanReadableString.Length - 2);
                }

            }


            return styleData;
        }

        /// <summary>
        /// Creates and returns a Style Opening Tag for the default style sheet for a specific binding.
        /// </summary>
        public static InputStyleData GetStyleOpeningTag(InputAction action, InputBinding binding, string deviceDisplayName, string controlSchemeName)
        {

            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetIconSet(deviceDisplayName);
            if (iconSet == null)
            {
                InputIconsLogger.LogWarning("Could not find icon set with device display name: " + deviceDisplayName + ". Check your Input Icon Sets in " +
                    "Assets/Input Icons");

                if (InputIconsManagerSO.IsKeyboardControlScheme(controlSchemeName))
                    iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                else
                    iconSet = InputIconSetConfiguratorSO.Instance.xBoxIconSet;
            }

            if (binding.isComposite)
            {
                return GetStyleOpeningTagOfComposite(action, binding, deviceDisplayName, controlSchemeName);
            }


            if (binding.groups.Length == 0)
            {
                InputIconsLogger.LogWarning("There is no Control Scheme set for the Action " + action.name + " in the Input Action Asset: " + action.actionMap + "\n" +
                    "Please make sure to set up control schemes and to assign the proper devices to these control schemes.");
            }

            if (!BindingGroupContainsControlScheme(binding.groups, controlSchemeName))
            {
                //InputIconsLogger.LogWarning("Action does not contain searched control scheme name: " + controlSchemeName);
                return null;
            }

            string bindingTag = GetSpriteName(binding, controlSchemeName, true);

            //Handle the three types of Shift, Alt and Ctrl. There is "shift", "leftShift" and "rightShift"
            //just shift gets converted to left shift, the same for alt and ctrl
            if (bindingTag == "shift") bindingTag = "leftShift";
            if (bindingTag == "alt") bindingTag = "leftAlt";
            if (bindingTag == "ctrl") bindingTag = "leftCtrl";


            if (bindingTag == InputIconsManagerSO.Instance.textDisplayForUnboundActions)//if there is no binding, return empty string to display the unbound sprite
            {
                bindingTag = "";
            }
            else if (InputIconsManagerSO.Instance.displayType == InputIconsManagerSO.DisplayType.Sprites &&
                !iconSet.HasSprite(bindingTag))//if sprite of binding tag is not available, use custom fallback sprite instead of default TMP fallback
            {
                bindingTag = "FallbackSprite";
            }


            string styleOpeningTag = "<sprite=\"" + deviceDisplayName + "\" name=\"" + bindingTag.ToUpper() + "\"";
            if (InputIconsManagerSO.Instance.tintingEnabled)
                styleOpeningTag += ", tint=1";

            styleOpeningTag += ">";

            string bindingName = action.actionMap.name + "/" + action.name;
            if (binding.name != "")
                bindingName += "/" + binding.name;


            string tmproReference = "<style="+bindingName+">";
            string fontReference = "<style=font/"+bindingName+">";

            string humanReadableString = GetHumanReadableActionName(action, binding, controlSchemeName);
            //Debug.Log(humanReadableString);
            humanReadableString = InputIconsManagerSO.GetActionStringRenaming(humanReadableString);

            //humanReadableString = humanReadableString.Replace("{", "");
            //humanReadableString = humanReadableString.Replace("}", "");

            if (humanReadableString == "")
            {
                humanReadableString = InputIconsManagerSO.Instance.textDisplayForUnboundActions;
            }

            //Debug.Log(tmproReference + ": " + bindingTag);
            string fontCode = "\\u" + GetFontCodeOfDevice(bindingTag, deviceDisplayName);
            //Debug.Log(humanReadableString);

            //if (InputIconsManagerSO.Instance.displayType == InputIconsManagerSO.DisplayType.TextInBrackets)
            //styleOpeningTag = "[" + styleOpeningTag + "]";


            InputStyleData isd = new InputStyleData(bindingName, binding.isComposite, binding.isPartOfComposite, tmproReference, styleOpeningTag, humanReadableString, fontCode, fontReference);

            return isd;


        }

  
        public static string GetHumanReadableActionNameOfComposite(InputAction action, InputBinding binding, string controlSchemeName)
        {
            if (action == null) return "";
            if (binding == null) return "";

            string s = "";
            bool found = false;
            for (int i = 0; i < action.bindings.Count; i++)
            {

                if (found)
                    break;

                if (action.bindings[i] == binding && binding.isComposite)
                {
                    found = true;
                    bool lastWasComposite = true;
                    for (int j = i; j < action.bindings.Count; j++)
                    {
                        if (action.bindings[j].isComposite)//&& action.bindings[j+1].groups.Contains(controlSchemeName))
                        {
                            lastWasComposite = true;
                            continue;
                        }

                        if (BindingGroupContainsControlScheme(action.bindings[j].groups, controlSchemeName))
                        {
                            if (s.Length > 0)
                            {
                                if (lastWasComposite)
                                    s += InputIconsManagerSO.Instance.multipleInputsDelimiter;
                                else
                                    s += ", ";
                            }


                            s += GetHumanReadableActionName(action, action.bindings[j], controlSchemeName);
                        }

                        lastWasComposite = false;

                    }
                }
            }

            return s;
        }

        public static string GetHumanReadableActionName(InputAction action, InputBinding binding, string controlSchemeName)
        {
            if (action == null) return "";
            if (binding == null) return "";

            if (binding.isComposite)
            {
                return GetHumanReadableActionNameOfComposite(action, binding, controlSchemeName);
            }

            string s = "";

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i] == binding)
                {
                    if (binding.groups.Length == 0)
                    {
                        InputIconsLogger.LogWarning("There is no Control Scheme set for the Action " + action.name + " in the Input Action Asset: " + action.actionMap + "\n" +
                            "Please make sure to set up control schemes and to assign the proper devices to these control schemes.");
                    }

                    if (!BindingGroupContainsControlScheme(binding.groups, controlSchemeName))
                    {
                        continue;
                    }

                    string bindingTag = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

                    if (InputIconsManagerSO.IsKeyboardControlScheme(controlSchemeName))
                    {
                        bindingTag = GetKeyboardString(binding, false);
                    }


                    if (s.Length > 0)
                    {
                        if (InputIconsManagerSO.Instance.showAllInputOptionsInStyles)
                        {
                            s += InputIconsManagerSO.Instance.multipleInputsDelimiter;
                        }
                    }

                    s += bindingTag.ToUpper();

                    if (!InputIconsManagerSO.Instance.showAllInputOptionsInStyles)
                        return s;

                }
            }

            return s.Trim();
        }


        /// <summary>
        /// Cleans up the styles and removes any unnecessary characters
        /// </summary>
        /// <param name="styles"></param>
        /// <returns></returns>
        private static List<string> GetCleanedUpStyles(List<List<string>> styles)
        {
            List<string> outputParts = new List<string>();
            int unboundCount = 0;
            int delimiterCount = 0;
            string delimiter = InputIconsManagerSO.Instance.multipleInputsDelimiter;

            for (int i = 0; i < styles.Count; i++) //combine composites together
            {
                string s = "";
                for (int j = 0; j < styles[i].Count; j++)
                {
                    s += styles[i][j];
                }
                //Debug.Log("add: " + s);
                outputParts.Add(s);

                if (outputParts[i].Contains("\"\""))
                {
                    unboundCount++;
                }

                if (outputParts[i] == delimiter)
                    delimiterCount++;
            }


            //for non-composites: remove fallback sprites if another binding is available (e.g. do not display "Jump: Space or FallbackSprite or U" ... or "Jump: FallbackSprite or Space or U")
            if (unboundCount > 0)
            {
                string[] stringSeparators = new string[] { "sprite" };
                int spriteCount = outputParts[0].Split(stringSeparators, System.StringSplitOptions.None).Length;

                //Debug.Log(spriteCount + " " + delimiterCount);

                if (spriteCount <= delimiterCount && outputParts.Count > 0)
                {
                    for (int i = outputParts.Count - 1; i >= 0; i--)
                    {
                        if (outputParts[i].Contains("\"\""))
                        {
                            outputParts.RemoveAt(i);
                        }
                    }
                }
            }



            for (int i = 0; i < outputParts.Count; i++) //remove double texts, like "E or E" ... or "WASD or WASD"
            {
                for (int j = outputParts.Count - 1; j >= 0; j--)
                {
                    if (i != j && outputParts[i] == outputParts[j] && outputParts[i] != delimiter)
                    {
                        outputParts.RemoveAt(j);
                    }
                }
            }

            bool delimitersRemoved;
            do
            {
                delimitersRemoved = false;
                if (outputParts.Count > 0) //remove unnecessary delimiters at the front and back if there are any
                {
                    if (outputParts[outputParts.Count - 1] == delimiter)
                    {
                        outputParts.RemoveAt(outputParts.Count - 1);
                        delimitersRemoved = true;
                    }

                    if (outputParts[0] == delimiter)
                    {
                        outputParts.RemoveAt(0);
                        delimitersRemoved = true;
                    }
                }
            }
            while (delimitersRemoved);

            //remove duplicate delimiters in the center
            bool lastWasDelimiter = false;
            for (int i = outputParts.Count - 1; i >= 0; i--)
            {
                if (outputParts[i] == delimiter)
                {
                    if (lastWasDelimiter)
                    {
                        outputParts.RemoveAt(i);
                    }
                    lastWasDelimiter = true;
                }
                else
                    lastWasDelimiter = false;
            }


            return outputParts;
        }

        public static string GetCorrectKeyForSpecialCases(string keyName)
        {
            //Handle the three types of Shift, Alt and Ctrl. There is "shift", "leftShift" and "rightShift"
            //just shift gets converted to left shift, the same for alt and ctrl
            if (keyName == "shift") return "leftShift";
            if (keyName == "alt") return "leftAlt";
            if (keyName == "ctrl") return "leftCtrl";
            return keyName;
        }

        /// <summary>
        /// Returns a human readable string for a binding. Uses either system language or english only, depending on the setting in the InputIconsManager
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="forStyleOpeningTag"></param>
        /// <returns></returns>
        public static string GetKeyboardString(InputBinding binding, bool forStyleOpeningTag)
        {
            if (cachedKeyboardStrings.TryGetValue(binding, out string cachedResult))
            {
                return cachedResult;
            }


            string fullButtonPath = binding.effectivePath;
            if (string.IsNullOrEmpty(fullButtonPath))
            {
                return InputIconsManagerSO.Instance.textDisplayForUnboundActions;
            }

            string buttonFullName = GetTextAfterSlash(fullButtonPath);
            string currentLayout = "";
            if(Keyboard.current!=null)
                currentLayout = Keyboard.current.keyboardLayout;

            InputControl control = InputSystem.FindControl(fullButtonPath);
            KeyControl keyControl = control as KeyControl;

            if (!forStyleOpeningTag && keyControl != null)
            {
                if (InputIconsManagerSO.Instance.textDisplayLanguage == InputIconsManagerSO.TextDisplayLanguage.SystemLanguage
                    && SupportedLayouts.Contains(currentLayout))
                {
                    return keyControl.displayName;
                }
                return binding.ToDisplayString();
            }

            if (keyControl != null)
            {
                switch (buttonFullName.Length)
                {
                    case 1:
                        if (char.IsDigit(buttonFullName[0]))
                        {
                            buttonFullName = "Digit" + buttonFullName;
                        }
                        else if (char.IsLetter(buttonFullName[0]) && SupportedLayouts.Contains(currentLayout))
                        {
                            buttonFullName = keyControl.displayName;
                        }
                        break;
                    default:
                        if (keyControl.displayName.Length == 1 && char.IsLetter(keyControl.displayName[0])
                            && SupportedLayouts.Contains(currentLayout))
                        {
                            buttonFullName = keyControl.displayName;
                        }
                        break;
                }
            }

            string result = buttonFullName.Replace(" ", "");
            cachedKeyboardStrings[binding] = result;
            return result;
        }

        private static string GetTextAfterSlash(string input)
        {
            int slashIndex = input.IndexOf(">/");
            if (slashIndex != -1)
            {
                // Get the substring starting from the index after '>/'
                string result = input.Substring(slashIndex + 2);
                return result;
            }

            // Return the original string if '>/' was not found
            return input;
        }

        private static bool IsNumber(char c)
        {
            int number = System.Convert.ToInt32(c);
            if (number >= 48 && number <= 57)
            {
                return true;
            }
            return false;
        }

        private static bool IsLetter(char c)
        {
            int number = System.Convert.ToInt32(c);
            if ((number >= 65 && number <= 90)
                || (number >= 97 && number <= 122))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Returns the binding index of an action of the active device.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingType">Binding type of the action. Can be anything if the action is not a part of a composite</param>
        /// <returns></returns>
        public static int GetIndexOfBindingType(InputAction action, BindingType bindingType, string controlScheme, int bindingNumberInList = 0)
        {
            if (action == null) return -1;

            //Debug.Log("action: " + action.name + " bindingType: " + bindingType.ToString() + " scheme: " + controlScheme + " bindingID " + bindingNumberInList);
            int c = 0;
            for (int i = 0; i < action.bindings.Count; i++)
            {

                if (BindingGroupContainsControlScheme(action.bindings[i].groups, controlScheme))
                {
                    //Debug.Log(action.bindings[i]);
                    bool isComposite = ActionIsComposite(action, controlScheme);
                    if (!isComposite)
                    {
                        //is it the actual binding we are looking for? In case we have several fitting bindings
                        //(e.g. Attack with space and lmb, but we need the lmb not space)
                        if (bindingNumberInList != 0)
                        {
                            if (bindingNumberInList == c)
                                return i;
                            else
                                c++;
                        }
                        else
                            return i;

                    }
                    else if (action.bindings[i].name.ToUpper() == bindingType.ToString().ToUpper())
                    {
                        if (bindingNumberInList == c)
                            return i;
                        else
                            c++;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the binding index of an action of the active device.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingType">Binding type of the action. Can be anything if the action is not a part of a composite</param>
        /// <returns></returns>
        public static int GetIndexOfBindingType(InputAction action, CompositeType compositeType, BindingType bindingType, string controlScheme, int bindingNumberInList = 0)
        {
            if (action == null) return -1;

            //Debug.Log("action: " + action.name + " bindingType: " + bindingType.ToString() + " scheme: " + controlScheme + " bindingID " + bindingNumberInList);
            int c = 0;
            for (int i = 0; i < action.bindings.Count; i++)
            {

                if (BindingGroupContainsControlScheme(action.bindings[i].groups, controlScheme))
                {
                    //Debug.Log(action.bindings[i]);
                    
                    bool isComposite = BindingIsComposite(action.bindings[i], controlScheme);
                    if (!isComposite && compositeType == CompositeType.NonComposite)
                    {

                        //Debug.Log("is non composite "+action.bindings[i]);
                        //is it the actual binding we are looking for? In case we have several fitting bindings
                        //(e.g. Attack with space and lmb, but we need the lmb not space)
                        if (bindingNumberInList != 0)
                        {
                            if (bindingNumberInList == c)
                                return i;
                            else
                                c++;
                        }
                        else
                            return i;

                    }
                    else if (action.bindings[i].name.ToUpper() == bindingType.ToString().ToUpper() && compositeType == CompositeType.Composite)
                    {
                        //Debug.Log("is composite " + action.bindings[i]);

                        if (bindingNumberInList == c)
                            return i;
                        else
                            c++;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns all binding indexes of an action of the active device.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingType">Binding type of the action. Can be anything if the action is not a part of a composite</param>
        /// <returns></returns>
        public static List<int> GetIndexesOfBindingType(InputAction action, BindingType bindingType, string controlScheme)
        {
            List<int> outputList = new List<int>();

            if (action == null)  return outputList;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (BindingGroupContainsControlScheme(action.bindings[i].groups, controlScheme))
                {
                    if (!BindingIsComposite(action.bindings[i], controlScheme))
                    {
                        outputList.Add(i);
                    }
                    else if (action.bindings[i].name.ToUpper() == bindingType.ToString().ToUpper())
                    {
                        outputList.Add(i);
                    }
                }
            }
            return outputList;
        }

        public static List<List<int>> GetGroupIndexesOfActionDeviceSpecific(InputAction action, string controlScheme)
        {
            List<List<int>> outputList = new List<List<int>>();
            List<int> currentList = new List<int>();

            if (action == null)
                return outputList;

            bool lastWasPartOfComposite = false;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                InputBinding binding = action.bindings[i];
                bool isPartOfComposite = binding.isPartOfComposite;
                bool isComposite = binding.isComposite;

                //Debug.Log("binding: " + binding.name + " __ isPartOfComposite: " + isPartOfComposite + " ___ isComposite: " + isComposite);

                if (isComposite)
                {
                    if (currentList.Count > 0)
                        outputList.Add(new List<int>(currentList));

                    currentList.Clear();
                    continue;
                }

                // Check if the binding belongs to the specified control scheme
                if (BindingGroupContainsControlScheme(binding.groups, controlScheme))
                {
                    //This is a one button binding
                    if (!isComposite && !isPartOfComposite)
                    {
                        if (currentList.Count > 0)
                            outputList.Add(new List<int>(currentList));

                        currentList.Clear();
                        currentList.Add(i);
                        outputList.Add(new List<int>(currentList));
                        currentList.Clear();

                        lastWasPartOfComposite = false;
                    }
                    //Is part of a composite
                    else if(isPartOfComposite)
                    {
                        if (!lastWasPartOfComposite)
                        {
                            if (currentList.Count > 0)
                                outputList.Add(new List<int>(currentList));

                            currentList.Clear();
                        }

                        currentList.Add(i);
                        lastWasPartOfComposite = true;
                    }
                    else if(isComposite)
                    {
                        lastWasPartOfComposite = false;
                    }
                }
            }

            if (currentList.Count > 0)
                outputList.Add(currentList);

            /*
            string s = controlScheme+ "___ ";
            for(int i=0; i<outputList.Count; i++)
            {
                for(int j=0; j < outputList[i].Count; j++)
                {
                    s += outputList[i][j];
                }
                s += " ----- ";
            }
            Debug.Log(s);
            */

            return outputList;
        }

        public static List<List<int>> GetGroupIndexesOfActionDeviceSpecific(InputAction action, CompositeType compositeType, string controlScheme)
        {
            List<List<int>> outputList = new List<List<int>>();
            List<int> currentList = new List<int>();

            if (action == null)
                return outputList;

            bool lastWasPartOfComposite = false;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                InputBinding binding = action.bindings[i];
                bool isPartOfComposite = binding.isPartOfComposite;
                bool isComposite = binding.isComposite;

                //Debug.Log("binding: " + binding.name + " __ isPartOfComposite: " + isPartOfComposite + " ___ isComposite: " + isComposite);

                if(isComposite)
                {
                    if (currentList.Count > 0)
                        outputList.Add(new List<int>(currentList));

                    currentList.Clear();
                    continue;
                }

                // Check if the binding belongs to the specified control scheme
                if (BindingGroupContainsControlScheme(binding.groups, controlScheme))
                {
                    // If the binding is non-composite and compositeType is NonComposite, or if the binding is part of a composite and compositeType is Composite
                    if ((!isComposite && !isPartOfComposite && compositeType == CompositeType.NonComposite) || (isPartOfComposite && compositeType == CompositeType.Composite))
                    {

                        if (compositeType == CompositeType.NonComposite)
                        {
                            currentList.Clear();
                            currentList.Add(i);
                            outputList.Add(new List<int>(currentList));
                            currentList.Clear();
                        }

                        if (compositeType == CompositeType.Composite)
                        {
                            currentList.Add(i);
                            lastWasPartOfComposite = true;
                        }
                       
                    }
                    // If the binding is composite and compositeType is NonComposite, or if the binding is not part of a composite and compositeType is Composite
                    else
                    {
                        if (lastWasPartOfComposite)
                        {
                            if (currentList.Count > 0)
                                outputList.Add(new List<int>(currentList));

                            currentList.Clear();
                        }

                        lastWasPartOfComposite = false;
                    }
                }
            }

            if (currentList.Count > 0)
                outputList.Add(currentList);

            return outputList;
        }

        public static List<int> GetIndexesOfBindingTypeDeviceSpecific(InputAction action, CompositeType compositeType, BindingType bindingType, string controlScheme)
        {
            List<int> outputList = new List<int>();

            if (action == null) return outputList;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                //Debug.Log(action.bindings[i].GetNameOfComposite());
                if (BindingGroupContainsControlScheme(action.bindings[i].groups, controlScheme))
                {
                  
                    if (!BindingIsComposite(action.bindings[i], controlScheme) && compositeType == CompositeType.NonComposite)
                    {
                        outputList.Add(i);
                    }
                    else if (action.bindings[i].name.ToUpper() == bindingType.ToString().ToUpper() && compositeType == CompositeType.Composite)
                    {
                        outputList.Add(i);
                    }
                }
            }
            return outputList;
        }


        public static HashSet<CompositeType> GetAvailableCompositeTypesOfAction(InputAction inputAction, string controlSchemeName)
        {
            HashSet<CompositeType> outputList = new HashSet<CompositeType>();
            foreach(InputBinding binding in inputAction.bindings)
            {
                if (BindingGroupContainsControlScheme(binding.groups, controlSchemeName))
                {
                    if (binding.isPartOfComposite)
                        outputList.Add(CompositeType.Composite);

                    if (!binding.isComposite && !binding.isPartOfComposite)
                        outputList.Add(CompositeType.NonComposite);
                }
            }

            //foreach (CompositeType c in outputList)
            //{
            //    Debug.Log(c);
            //}
            return outputList;
        }


        /// <returns>Returns the first binding index of the current device</returns>
        public static int GetIndexOfInputBinding(InputAction action, InputBinding binding)
        {
            if(action == null) return -1;

            string deviceString = GetActiveDeviceString();
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (BindingGroupContainsControlScheme(action.bindings[i].groups, deviceString))
                {
                    if (action.bindings[i].id == binding.id)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns all bindings of an action. Can be used after assigning an override to an action to check if
        /// there are any duplicates of bindings and to react accordingly.
        /// </summary>
        /// <returns>All bindings of an action</returns>
        public static List<InputBinding> GetBindings(InputAction action, BindingType bindingType, string controlScheme)
        {
            List<InputBinding> foundBindings = new List<InputBinding>();

            if (action == null) return foundBindings;

            List<int> indexes = GetIndexesOfBindingType(action, bindingType, controlScheme);

            for (int i = 0; i < indexes.Count; i++)
            {
                foundBindings.Add(action.bindings[indexes[i]]);
            }

            return foundBindings;
        }

        public static int GetNumberOfBindings(InputAction action)
        {
            if (action == null) return 0;

            return action.bindings.Count;
        }

        //Comparing like this takes more effort, but improves usability by also accepting small typos in form of lower/uppercase errors
        //Also splits group into an array. A string is more difficult to check as "Gamepad" gets detected in "Gamepad 2" as well for example 
        public static bool BindingGroupContainsControlScheme(string group, string controlScheme)
        {
            List<string> groupArray = group.Split(';').ToList();

            foreach (string item in groupArray)
            {
                if (item.Trim().ToUpper() == controlScheme.Trim().ToUpper())
                    return true;
            }
            return false;
        }

        public static bool BindingIsComposite(InputBinding binding)
        {
            if(binding == null) return false;

            string deviceString = GetActiveDeviceString();
            if (BindingGroupContainsControlScheme(binding.groups, deviceString))
            {
                if (binding.isPartOfComposite)
                    return true;
            }

            return false;
        }

        public static bool BindingIsComposite(InputBinding binding, string controlSchemeName)
        {
            if (binding == null) return false;

            if (BindingGroupContainsControlScheme(binding.groups, controlSchemeName))
            {
                if (binding.isPartOfComposite)
                    return true;
            }

            return false;
        }

        public static bool ActionIsComposite(InputAction action)
        {
            if (action == null) return false;

            string deviceString = GetActiveDeviceString();
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (BindingGroupContainsControlScheme(action.bindings[i].groups, deviceString))
                {
                   

                    if (action.bindings[i].isPartOfComposite)
                        return true;
                }
            }

            return false;
        }

        public static bool ActionIsComposite(InputAction action, string controlSchemeName)
        {
            if (action == null) return false;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (BindingGroupContainsControlScheme(action.bindings[i].groups, controlSchemeName))
                {
                    if (action.bindings[i].isPartOfComposite)
                        return true;
                }
            }

            return false;
        }

        public static string GetFontCodeOfDevice(string textMeshStyleTag, string deviceDisplayName)
        {
            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetIconSet(deviceDisplayName);
            if (iconSet == null)
                return "";

            List<InputSpriteData> data = iconSet.GetAllInputSpriteData();
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].textMeshStyleTag.ToUpper() == textMeshStyleTag.ToUpper())
                {
                    //Debug.Log("font code for: " + data[i].textMeshStyleTag + " is: " + data[i].fontCode);
                    return data[i].fontCode;
                }
            }

            if (textMeshStyleTag != "FallbackSprite")
                return GetFontCodeOfDevice("FallbackSprite", deviceDisplayName);

            return "";
        }

        /// <summary>
        /// Get the name of the currently used device ... Keyboard and Mouse, PS3, PS4, XBox, ...
        /// </summary>
        /// <returns></returns>
        public static string GetActiveDeviceString()
        {
            InputDevice device = InputIconsManagerSO.GetCurrentInputDevice();
            string deviceString;
            if (device is Gamepad)
            {
                deviceString = InputIconsManagerSO.GetGamepadControlSchemeName(0);
            }
            else
            {
                deviceString = InputIconsManagerSO.GetKeyboardControlSchemeName(0);
            }

            return deviceString;
        }

        /// <summary>
        /// Creates updated input style data for the assigned Input Action Assets
        /// </summary>
        /// <param name="usedActionAssets">The assets for which to create style updates</param>
        /// <param name="controlSchemeName">Keyboard and Mouse or Gamepad control scheme</param>
        /// <param name="deviceString">Keyboard, PS3, PS4, XBox, ... the name of the sprite atlas</param>
        /// <returns></returns>
        public static List<InputStyleData> CreateInputStyleData(List<InputActionAsset> usedActionAssets, string controlSchemeName, string deviceString)
        {

            List <InputStyleData> outputList = new List<InputStyleData>();

            for (int k = 0; k < usedActionAssets.Count; k++)
            {
                if (usedActionAssets[k] == null)
                    continue;


                for (int m = 0; m < usedActionAssets[k].actionMaps.Count; m++)
                {
                    for (int j = 0; j < usedActionAssets[k].actionMaps[m].actions.Count; j++)
                    {
                        for (int i = 0; i < usedActionAssets[k].actionMaps[m].actions[j].bindings.Count; i++)
                        {
                            InputAction action = usedActionAssets[k].actionMaps[m].actions[j];
                            InputBinding binding = action.bindings[i];
                            InputStyleData data = GetStyleOpeningTag(action, binding, deviceString, controlSchemeName);
                            if (data == null)
                                continue;

                            //Debug.Log(data.bindingName + ": " + data.inputStyleString);
                            outputList.Add(data);
                        }

                    }
                }
            }

            return outputList;
        }

        public static bool InputStyleDataListContainsBinding(List<InputStyleData> list, string bindingName)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].bindingName == bindingName)
                    return true;
            }
            return false;
        }

        //Overrides the values in the default style sheet with the ones currently in the style lists of the manager
        public static void OverrideStylesInStyleSheetDeviceSpecific(bool gamepadStyles)
        {
            List<TMP_InputStyleHack.StyleStruct> styleUpdates = GetStyleUpdatesDeviceSpecific(gamepadStyles);

            TMP_InputStyleHack.UpdateStyles(styleUpdates);
        }

        public static List<TMP_InputStyleHack.StyleStruct> GetStyleUpdatesDeviceSpecific(bool gamepadStyles)
        {

            List<TMP_InputStyleHack.StyleStruct> styleUpdates = new List<TMP_InputStyleHack.StyleStruct>();

            List<InputStyleData> inputStyles;

            string inputIconsOpeningTag = InputIconsManagerSO.Instance.openingTag;
            string inputIconsClosingTag = InputIconsManagerSO.Instance.closingTag;

            if (!gamepadStyles)
                inputStyles = InputIconsManagerSO.Instance.inputStyleKeyboardDataList;
            else
                inputStyles = InputIconsManagerSO.Instance.inputStyleGamepadDataList;

            for (int i = 0; i < inputStyles.Count; i++)
            {
                if (inputStyles[i] == null)
                    continue;


                string style = InputIconsManagerSO.Instance.GetCustomStyleTag(inputStyles[i]);
                style = inputIconsOpeningTag + style + inputIconsClosingTag;
                styleUpdates.Add(new TMP_InputStyleHack.StyleStruct(inputStyles[i].bindingName, style, ""));



                if (!InputIconsManagerSO.Instance.isUsingFonts) //if not using fonts, can skip this
                    continue;

                //handle font codes as well
                style = GetFontTagDeviceSpecific(gamepadStyles);

                if (InputIconsManagerSO.Instance.showAllInputOptionsInStyles)
                    style += inputStyles[i].fontCode;
                else
                    style += inputStyles[i].fontCode_singleInput;

                string closingTag = "</font>";
                styleUpdates.Add(new TMP_InputStyleHack.StyleStruct("Font/" + inputStyles[i].bindingName, style, closingTag));
            }


            return styleUpdates;
        }

        public static string GetFontTagDeviceSpecific(bool gamepadStyles)
        {
            string fontTag = "";
            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
            if (gamepadStyles)
                iconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();

            if (iconSet.fontAsset != null)
                fontTag = "<font=\"" + iconSet.fontAsset.name + "\"";

            else
            {
                fontTag = "<font=\"InputIcons_Keyboard_Font SDF\"";
                if (gamepadStyles)
                    fontTag = "<font=\"InputIcons_Gamepad_Font SDF\"";
            }

            fontTag += ">";
            return fontTag;
        }

        public static string GetFontDisplaySingleInputDeviceSpecific(string bindingName, bool gamepadStyles, int IdInBindings = 0)
        {
            string output = "";

            List<InputStyleData> styleList;

            if (!gamepadStyles)
                styleList = InputIconsManagerSO.Instance.inputStyleKeyboardDataList;
            else
                styleList = InputIconsManagerSO.Instance.inputStyleGamepadDataList;

            string nameToCheckAgainst = bindingName;
            if (IdInBindings > 0)
            {
                int actualID = IdInBindings + 1;
                nameToCheckAgainst += "/" + actualID;
            }

            for (int i = 0; i < styleList.Count; i++)
            {
                if (styleList[i].bindingName == nameToCheckAgainst)
                {
                    output = GetFontTagDeviceSpecific(gamepadStyles) + styleList[i].fontCode_singleInput;
                }
            }
            return output;
        }


        //Needed in builds. Not necessarily needed in editor.
        public static void RefreshAllTMProUGUIObjects()
        {

            if (InputIconsManagerSO.Instance.textUpdateOptions == InputIconsManagerSO.TextUpdateOptions.SearchAndUpdate)
            {
                //go through all Text objects in the scene and set them dirty
                GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    TextMeshProUGUI[] tmpObjects = obj.GetComponentsInChildren<TextMeshProUGUI>();
                    foreach (TextMeshProUGUI tObj in tmpObjects)
                    {
                        tObj.SetAllDirty();

#if UNITY_EDITOR
                        if (Application.isEditor)
                            EditorUtility.SetDirty(tObj);
#endif
                    }

                    TextMeshPro[] tmpObjects2 = obj.GetComponentsInChildren<TextMeshPro>();
                    foreach (TextMeshPro tObj in tmpObjects2)
                    {
                        tObj.SetAllDirty();

#if UNITY_EDITOR
                        if (Application.isEditor)
                            EditorUtility.SetDirty(tObj);
#endif
                    }
                }
            }
            else if (InputIconsManagerSO.Instance.textUpdateOptions == InputIconsManagerSO.TextUpdateOptions.ViaInputIconsTextComponents)
            {
                InputIconsManagerSO.RefreshInputIconsTexts();
            }


        }
    }

}

using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace InputIcons
{
    [CustomEditor(typeof(II_UIRebindInputActionImageBehaviour))]
    public class II_UIRebindActionImageBehaviourEditor : Editor
    {
        private void OnEnable()
        {


        }

        public override void OnInspectorGUI()
        {

            II_UIRebindInputActionImageBehaviour rebindBehaviour = (II_UIRebindInputActionImageBehaviour)target;

            II_RebindData clonedData = II_RebindData.Clone(rebindBehaviour.rebindData);



            EditorGUI.BeginChangeCheck();

            if (clonedData.actionReference != null)
            {
                EditorGUILayout.LabelField("Rebind: "+clonedData.actionReference.action.name, II_SpritePromptEditor.HeaderStyle.Get(), GUILayout.Width(150));
                EditorGUILayout.Space(5);
            }
            else
            {
                EditorGUILayout.LabelField("Rebind: ---", II_SpritePromptEditor.HeaderStyle.Get(), GUILayout.Width(150));
                EditorGUILayout.Space(5);
            }

            clonedData.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", clonedData.actionReference, typeof(InputActionReference), false);
            if (clonedData.actionReference == null)
            {
                if (EditorGUI.EndChangeCheck())
                {
                    HandleChange(rebindBehaviour, clonedData);
                }
                return;
            }

            clonedData.bindingSearchStrategy = (II_RebindData.BindingSearchStrategy)EditorGUILayout.EnumPopup("Search Binding By", clonedData.bindingSearchStrategy);
            if (clonedData.bindingSearchStrategy == II_RebindData.BindingSearchStrategy.BindingType)
            {
                clonedData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", clonedData.deviceType);


                int widthTitles = 140;
                //GUILayout.FlexibleSpace();


                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical(BackgroundStyle.Get(BackgroundStyle.GetDefaultColor()));
                EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                EditorGUILayout.LabelField("Control Scheme Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                EditorGUILayout.LabelField("Composite Types", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                EditorGUILayout.LabelField("Binding Types", EditorStyles.boldLabel, GUILayout.Width(widthTitles));

                EditorGUILayout.LabelField("Available Binding Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));

                EditorGUILayout.EndVertical();


                EditorGUILayout.Space(5);
                //Keyboard controls
                EditorGUILayout.BeginVertical(BackgroundStyle.Get(BackgroundStyle.GetKeyboardColor()));
                DrawKeyboardSettings(clonedData);
                DrawSpriteField(clonedData.GetSpriteKeyboard(), widthTitles, clonedData, false);
                EditorGUILayout.EndVertical();


                //Gamepad controls
                EditorGUILayout.BeginVertical(BackgroundStyle.Get(BackgroundStyle.GetGamepadColor()));
                DrawGamepadSettings(clonedData);
                DrawSpriteField(clonedData.GetSpriteGamepad(), widthTitles, clonedData, true);
                EditorGUILayout.EndVertical();


                GUILayout.FlexibleSpace();
                EditorGUILayout.Space(50);
                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();
            }
            else if(clonedData.bindingSearchStrategy == II_RebindData.BindingSearchStrategy.BindingIndex)
            {
                clonedData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", clonedData.deviceType);

                int widthTitles = 140;
                if (clonedData.deviceType == InputIconsUtility.DeviceType.Auto || clonedData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                {
                    clonedData.bindingIndexKeyboard = EditorGUILayout.IntSlider("Keyboard Binding Index", clonedData.bindingIndexKeyboard, 0, clonedData.actionReference.action.bindings.Count - 1);
                    DrawSpriteField(clonedData.GetKeySpriteByBindingIndexKeyboard(), widthTitles, clonedData, false);
                }

                if (clonedData.deviceType == InputIconsUtility.DeviceType.Auto || clonedData.deviceType == InputIconsUtility.DeviceType.Gamepad)
                {
                    clonedData.bindingIndexGamepad = EditorGUILayout.IntSlider("Gamepad Binding Index", clonedData.bindingIndexGamepad, 0, clonedData.actionReference.action.bindings.Count - 1);
                    DrawSpriteField(clonedData.GetKeySpriteByBindingIndexGamepad(), widthTitles, clonedData, true);
                }
            }


            if (clonedData.deviceType == InputIconsUtility.DeviceType.Auto || clonedData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
            {
                clonedData.optionalKeyboardIconSet = (InputIconSetKeyboardSO)EditorGUILayout.ObjectField("(Optional) Keyboard Icon Set", clonedData.optionalKeyboardIconSet, typeof(InputIconSetKeyboardSO), true);
            }

            if (clonedData.deviceType == InputIconsUtility.DeviceType.Auto || clonedData.deviceType == InputIconsUtility.DeviceType.Gamepad)
            {
                clonedData.optionalGamepadIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("(Optional) Gamepad Icon Set", clonedData.optionalGamepadIconSet, typeof(InputIconSetGamepadSO), true);
            }

            EditorGUILayout.Space(5);


            EditorGUILayout.LabelField("Rebinding", EditorStyles.boldLabel);
            clonedData.canBeRebound = EditorGUILayout.Toggle(new GUIContent("Allow Rebinding", "If disabled, this action can not be rebound, but will still be displayed."), clonedData.canBeRebound);

            if (InputIconsManagerSO.Instance.rebindBehaviour == InputIconsManagerSO.RebindBehaviour.OverrideExisting)
            {
                clonedData.ignoreOtherButtons = EditorGUILayout.Toggle(new GUIContent("Ignore Other Buttons", "If enabled, will not get unbound if another action gets assigned the same key."), clonedData.ignoreOtherButtons);
            }

            if (clonedData.canBeRebound)
            {
                clonedData.keyboardCancelKey = EditorGUILayout.TextField("Keyboard Cancel Key", clonedData.keyboardCancelKey);
                clonedData.gamepadCancelKey = EditorGUILayout.TextField("Gamepad Cancel Key", clonedData.gamepadCancelKey);
            }

            EditorGUILayout.Space(5);


            EditorGUILayout.LabelField("Displays", EditorStyles.boldLabel);
            if (clonedData.actionNameDisplayText)
            {
                clonedData.actionNameDisplayText.text = EditorGUILayout.TextField("Action Display Name", clonedData.actionNameDisplayText.text);
            }

            clonedData.actionNameDisplayText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Action Name Display Text", clonedData.actionNameDisplayText, typeof(TextMeshProUGUI), true);

            clonedData.bindingDisplayImage = (Image)EditorGUILayout.ObjectField("Binding Display Image", clonedData.bindingDisplayImage, typeof(Image), true);


            EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
            clonedData.rebindButtonObject = (GameObject)EditorGUILayout.ObjectField("Rebind Button Object", clonedData.rebindButtonObject, typeof(GameObject), true);
            clonedData.resetButtonObject = (GameObject)EditorGUILayout.ObjectField("Reset Button Object", clonedData.resetButtonObject, typeof(GameObject), true);

            EditorGUILayout.LabelField("Display Object While Rebinding", EditorStyles.boldLabel);
            clonedData.listeningForInputObject = (GameObject)EditorGUILayout.ObjectField("Listening For Input Object", clonedData.listeningForInputObject, typeof(GameObject), true);

            EditorGUILayout.LabelField("Display Object If Binding Already Used", EditorStyles.boldLabel);
            clonedData.keyAlreadyUsedObject = (GameObject)EditorGUILayout.ObjectField("Binding Already Used Object", clonedData.keyAlreadyUsedObject, typeof(GameObject), true);

            InputIconsManagerSO manager = InputIconsManagerSO.Instance;
            manager.rebindBehaviour = (InputIconsManagerSO.RebindBehaviour)EditorGUILayout.EnumPopup(new GUIContent("Rebind Behavior (same for all Rebind Buttons)",
                    "Choose how to handle rebinding when the same binding already exists in the same action map."), manager.rebindBehaviour);


            if (EditorGUI.EndChangeCheck())
            {
                HandleChange(rebindBehaviour, clonedData);
            }
        }

        private void HandleChange(II_UIRebindInputActionImageBehaviour rebindBehaviour, II_RebindData clonedData)
        {
            Undo.RecordObject(target, "Rebind Button Data Changed");
            rebindBehaviour.rebindData = II_RebindData.Clone(clonedData);

            rebindBehaviour.UpdateBindingDisplay();
            rebindBehaviour.OnValidate();
            EditorUtility.SetDirty(target);
        }

        private void DrawSpriteField(Sprite sprite, int width, II_RebindData rebindData, bool isGamepad)
        {
            if ((rebindData.deviceType == InputIconsUtility.DeviceType.Gamepad && isGamepad)
                || (rebindData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse && !isGamepad)
                || rebindData.deviceType == InputIconsUtility.DeviceType.Auto)
            {
                EditorGUILayout.ObjectField("", sprite, typeof(Sprite), false, GUILayout.Width(width));
            }
        }

    

        private void DrawKeyboardSettings(II_RebindData rebindData)
        {
            int width = 100;

            EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel, GUILayout.Width(width));
            if (rebindData.deviceType == InputIconsUtility.DeviceType.Auto || rebindData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
            {
                if (InputIconsManagerSO.GetKeyboardControlSchemeCountOfAction(rebindData.actionReference) > 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    rebindData.controlSchemeIndexKeyboard = EditorGUILayout.IntField(rebindData.controlSchemeIndexKeyboard, GUILayout.Width(20));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        rebindData.controlSchemeIndexKeyboard--;

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        rebindData.controlSchemeIndexKeyboard++;

                    rebindData.controlSchemeIndexKeyboard = Mathf.Clamp(rebindData.controlSchemeIndexKeyboard, 0, InputIconsManagerSO.Instance.controlSchemeNames_Keyboard.Count - 1);

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField("0", GUILayout.Width(width));
                    rebindData.controlSchemeIndexKeyboard = 0;
                }


                HashSet<InputIconsUtility.CompositeType> foundCompositeTypes = rebindData.GetAvailableCompositeTypesOfAction(rebindData.actionReference, false);
                if (foundCompositeTypes.Count > 0)
                {
                    if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite)
                        && foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        rebindData.compositeTypeKeyboard = (InputIconsUtility.CompositeType)EditorGUILayout.EnumPopup(rebindData.compositeTypeKeyboard, GUILayout.Width(width));
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite))
                    {
                        rebindData.compositeTypeKeyboard = InputIconsUtility.CompositeType.NonComposite;
                        EditorGUILayout.LabelField("Non-Composite", GUILayout.Width(width));
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        rebindData.compositeTypeKeyboard = InputIconsUtility.CompositeType.Composite;
                        EditorGUILayout.LabelField("Composite", GUILayout.Width(width));
                    }

                }


                if (rebindData.compositeTypeKeyboard == InputIconsUtility.CompositeType.Composite)
                {
                    rebindData.bindingTypeKeyboard = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup(rebindData.bindingTypeKeyboard, GUILayout.Width(width));
                }
                else
                    EditorGUILayout.LabelField("None", GUILayout.Width(width));


                if (rebindData.GetNumberOfAvailableBindingGroupsKeyboard(rebindData.actionReference) > 1)
                {
                    //Debug.Log("ffffouuund binding count: " + sData.GetNumberOfAvailableBindingGroups(sData.actionReference));


                    EditorGUILayout.BeginHorizontal();
                    rebindData.bindingIDInAvailableListKeyboard = EditorGUILayout.IntField(rebindData.bindingIDInAvailableListKeyboard, GUILayout.Width(20));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        rebindData.bindingIDInAvailableListKeyboard--;

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        rebindData.bindingIDInAvailableListKeyboard++;

                    rebindData.bindingIDInAvailableListKeyboard = Mathf.Clamp(rebindData.bindingIDInAvailableListKeyboard, 0, rebindData.GetNumberOfAvailableBindingGroupsKeyboard(rebindData.actionReference) - 1);

                    EditorGUILayout.EndHorizontal();

                }
                else
                {
                    rebindData.bindingIDInAvailableListKeyboard = 0;
                    EditorGUILayout.LabelField("0", GUILayout.Width(width));
                }
            }
        }

        private void DrawGamepadSettings(II_RebindData rebindData)
        {
            int width = 100;

            EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(width));
            if (rebindData.deviceType == InputIconsUtility.DeviceType.Auto || rebindData.deviceType == InputIconsUtility.DeviceType.Gamepad)
            {
                if (InputIconsManagerSO.GetGamepadControlSchemeCountOfAction(rebindData.actionReference) > 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    rebindData.controlSchemeIndexGamepad = EditorGUILayout.IntField(rebindData.controlSchemeIndexGamepad, GUILayout.Width(20));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        rebindData.controlSchemeIndexGamepad--;

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        rebindData.controlSchemeIndexGamepad++;

                    rebindData.controlSchemeIndexGamepad = Mathf.Clamp(rebindData.controlSchemeIndexGamepad, 0, InputIconsManagerSO.Instance.controlSchemeNames_Gamepad.Count - 1);

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField("0", GUILayout.Width(width));
                    rebindData.controlSchemeIndexGamepad = 0;
                }


                HashSet<InputIconsUtility.CompositeType> foundCompositeTypes = rebindData.GetAvailableCompositeTypesOfAction(rebindData.actionReference, true);
                if (foundCompositeTypes.Count > 0)
                {
                    if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite)
                        && foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        rebindData.compositeTypeGamepad = (InputIconsUtility.CompositeType)EditorGUILayout.EnumPopup(rebindData.compositeTypeGamepad, GUILayout.Width(width));
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite))
                    {
                        rebindData.compositeTypeGamepad = InputIconsUtility.CompositeType.NonComposite;
                        EditorGUILayout.LabelField("Non-Composite", GUILayout.Width(width));
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        rebindData.compositeTypeGamepad = InputIconsUtility.CompositeType.Composite;
                        EditorGUILayout.LabelField("Composite", GUILayout.Width(width));
                    }

                }


                if (rebindData.compositeTypeGamepad == InputIconsUtility.CompositeType.Composite)
                {
                    rebindData.bindingTypeGamepad = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup(rebindData.bindingTypeGamepad, GUILayout.Width(width));
                }
                else
                    EditorGUILayout.LabelField("None", GUILayout.Width(width));


                if (rebindData.GetNumberOfAvailableBindingGroupsGamepad(rebindData.actionReference) > 1)
                {
                    //Debug.Log("ffffouuund binding count: " + sData.GetNumberOfAvailableBindingGroups(sData.actionReference));


                    EditorGUILayout.BeginHorizontal();
                    rebindData.bindingIDInAvailableListGamepad = EditorGUILayout.IntField(rebindData.bindingIDInAvailableListGamepad, GUILayout.Width(20));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        rebindData.bindingIDInAvailableListGamepad--;

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        rebindData.bindingIDInAvailableListGamepad++;

                    rebindData.bindingIDInAvailableListGamepad = Mathf.Clamp(rebindData.bindingIDInAvailableListGamepad, 0, rebindData.GetNumberOfAvailableBindingGroupsGamepad(rebindData.actionReference) - 1);

                    EditorGUILayout.EndHorizontal();

                }
                else
                {
                    rebindData.bindingIDInAvailableListGamepad = 0;
                    EditorGUILayout.LabelField("0", GUILayout.Width(width));
                }
            }
        }

        public static class BackgroundStyle
        {
            private static GUIStyle style = new GUIStyle();
            private static Texture2D texture = new Texture2D(1, 1);

            public static GUIStyle Get(Color color)
            {
                if (texture == null)
                    texture = new Texture2D(1, 1);

                texture.SetPixel(0, 0, color);
                texture.Apply();
                style.normal.background = texture;
                return style;
            }

            public static Color GetDefaultColor()
            {
                return new Color(0.2f, 0.2f, 0.2f);
            }

            public static Color GetKeyboardColor()
            {
                return new Color(0.15f, 0.22f, 0.15f);
            }

            public static Color GetGamepadColor()
            {
                return new Color(0.15f, 0.18f, 0.25f);
            }
        }

        public static class HeaderStyle
        {
            private static GUIStyle style = new GUIStyle();

            public static GUIStyle Get()
            {
                style.normal.textColor = Color.white;
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                return style;
            }
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 5)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            //r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }

}

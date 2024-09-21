using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputIcons.II_SpritePrompt;

namespace InputIcons
{
    [CustomEditor(typeof(II_SpritePrompt))]
    public class II_SpritePromptEditor : Editor
    {
       


        public override void OnInspectorGUI()
        {
            II_SpritePrompt spritePromptBehaviour = (II_SpritePrompt)target;

            List<SpritePromptData> savedPromptData = SpritePromptData.CloneList(spritePromptBehaviour.spritePromptDatas);

            EditorGUI.BeginChangeCheck();
            
            foreach(SpritePromptData sData in savedPromptData.ToList())
            {
                if (sData.actionReference != null)
                {
                    EditorGUILayout.LabelField(sData.actionReference.action.name, II_SpritePromptEditor.HeaderStyle.Get(), GUILayout.Width(150));
                    EditorGUILayout.Space(5);
                }

                sData.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", sData.actionReference, typeof(InputActionReference), false);
                if (sData.actionReference != null)
                {
                    sData.bindingSearchStrategy = (SpritePromptData.BindingSearchStrategy)EditorGUILayout.EnumPopup("Search Binding By", sData.bindingSearchStrategy);
                    if (sData.bindingSearchStrategy == SpritePromptData.BindingSearchStrategy.BindingType)
                    {
                        sData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", sData.deviceType);

                        sData.advancedMode = EditorGUILayout.Toggle("Advanced Mode", sData.advancedMode);

                        if(sData.advancedMode)
                        {
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
                            DrawKeyboardAdvanced(sData);
                            II_SpritePromptEditor.DrawSpriteField(sData.GetSpriteKeyboardAdvanced(), widthTitles, sData, false);
                            EditorGUILayout.EndVertical();
                        

                            //Gamepad controls
                            EditorGUILayout.BeginVertical(BackgroundStyle.Get(BackgroundStyle.GetGamepadColor()));
                            DrawGamepadAdvanced(sData);
                            II_SpritePromptEditor.DrawSpriteField(sData.GetSpriteGamepadAdvanced(), widthTitles, sData, true);
                            EditorGUILayout.EndVertical();


                            GUILayout.FlexibleSpace();
                            EditorGUILayout.Space(50);
                            GUILayout.FlexibleSpace();

                            EditorGUILayout.EndHorizontal();

                           


                            EditorGUILayout.Space(15);
                        }
                        else //normal mode
                        {

                            DrawNormalModeControls(sData);
                            /*if (sData.GetNumberOfAvailableBindings(sData.actionReference) > 1)
                            {
                                sData.bindingIDInAvailableList = EditorGUILayout.IntSlider("Binding ID In List", sData.bindingIDInAvailableList, 0, sData.GetNumberOfAvailableBindings(sData.actionReference) - 1);
                            }*/

                            DrawDeviceSpritesNormalMode(sData);
                        }

                        

                    }
                    else if (sData.bindingSearchStrategy == SpritePromptData.BindingSearchStrategy.BindingIndex)
                    {

                        DrawSearchByBindingIndexFields(sData);

                    }
                }

         
                if(sData.spriteRenderer == null)
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.HelpBox("Assign a SpriteRenderer to display binding in scene", MessageType.Warning);
                }
                sData.spriteRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Sprite Renderer", sData.spriteRenderer, typeof(SpriteRenderer), true);

                if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                {
                    sData.optionalKeyboardIconSet = (InputIconSetKeyboardSO)EditorGUILayout.ObjectField("(Optional) Keyboard Icon Set", sData.optionalKeyboardIconSet, typeof(InputIconSetKeyboardSO), true);
                }

                if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
                {
                    sData.optionalGamepadIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("(Optional) Gamepad Icon Set", sData.optionalGamepadIconSet, typeof(InputIconSetGamepadSO), true);
                }

                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                {
                    savedPromptData.Remove(sData);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(20);
                DrawUILine(Color.grey);
            }


            if (GUILayout.Button("Add"))
            {
                if (savedPromptData.Count > 0)
                    savedPromptData.Add(new SpritePromptData(savedPromptData[savedPromptData.Count - 1]));
                else
                    savedPromptData.Add(new SpritePromptData());
            }


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Prompt Data Changed");
                spritePromptBehaviour.spritePromptDatas = new List<SpritePromptData>(savedPromptData);

                spritePromptBehaviour.OnValidate();
                EditorUtility.SetDirty(target);
            }
        }

        public static void DrawSearchByBindingIndexFields(II_PromptData sData)
        {
            sData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", sData.deviceType);

            if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
            {
                sData.bindingIndexKeyboard = EditorGUILayout.IntSlider("Keyboard Binding Index", sData.bindingIndexKeyboard, 0, sData.actionReference.action.bindings.Count - 1);
                EditorGUILayout.ObjectField("", sData.GetKeySpriteByBindingIndexKeyboard(), typeof(Sprite), false, GUILayout.Width(100));
            }

            if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
            {
                sData.bindingIndexGamepad = EditorGUILayout.IntSlider("Gamepad Binding Index", sData.bindingIndexGamepad, 0, sData.actionReference.action.bindings.Count - 1);
                EditorGUILayout.ObjectField("", sData.GetSpriteByBindingIndexGamepad(), typeof(Sprite), false, GUILayout.Width(100));
            }

        }

        public static void DrawNormalModeControls(II_PromptData sData)
        {
            bool isComposite = false;
            if ((sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                && InputIconsUtility.ActionIsComposite(sData.actionReference.action, sData.GetControlSchemeNameKeyboard()))
                isComposite = true;
            else if (sData.deviceType == InputIconsUtility.DeviceType.Gamepad && InputIconsUtility.ActionIsComposite(sData.actionReference.action, sData.GetControlSchemeNameGamepad()))
                isComposite = true;

            if (isComposite)
            {
                EditorGUI.BeginDisabledGroup(true);
                InputIconsUtility.CompositeType c = (InputIconsUtility.CompositeType)EditorGUILayout.EnumPopup("Composite Type", InputIconsUtility.CompositeType.Composite);
                EditorGUI.EndDisabledGroup();

                sData.bindingType = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup("Binding Type", sData.bindingType);
                sData.compositeType = InputIconsUtility.CompositeType.Composite;
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                InputIconsUtility.CompositeType c = (InputIconsUtility.CompositeType)EditorGUILayout.EnumPopup("Composite Type", InputIconsUtility.CompositeType.NonComposite);
                EditorGUI.EndDisabledGroup();

                sData.compositeType = InputIconsUtility.CompositeType.NonComposite;
            }

            if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
            {
                if (InputIconsManagerSO.GetKeyboardControlSchemeCountOfAction(sData.actionReference) > 1)
                {
                    sData.keyboardControlSchemeIndex = EditorGUILayout.IntSlider("Keyboard Control Scheme Index", sData.keyboardControlSchemeIndex, 0, InputIconsManagerSO.Instance.controlSchemeNames_Keyboard.Count - 1);
                }
                else
                {
                    sData.AutoassignControlSchemeIndexKeyboard();
                    EditorGUILayout.LabelField("Keyboard Control Scheme Index: " + sData.keyboardControlSchemeIndex);
                }
            }


            if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
            {
                if (InputIconsManagerSO.GetGamepadControlSchemeCountOfAction(sData.actionReference) > 1)
                {
                    sData.gamepadControlSchemeIndex = EditorGUILayout.IntSlider("Gamepad Control Scheme Index", sData.gamepadControlSchemeIndex, 0, InputIconsManagerSO.Instance.controlSchemeNames_Gamepad.Count - 1);
                }
                else
                {
                    sData.AutoassignControlSchemeIndexGamepad();
                    EditorGUILayout.LabelField("Gamepad Control Scheme Index: " + sData.gamepadControlSchemeIndex);
                }
            }
               


            if ((sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                && sData.GetNumberOfAvailableBindings(sData.actionReference, false) > 1)
            {
                sData.bindingIDInAvailableList = EditorGUILayout.IntSlider("Binding ID In List", sData.bindingIDInAvailableList, 0, sData.GetNumberOfAvailableBindings(sData.actionReference, false)-1);
            }
            else if (sData.deviceType == InputIconsUtility.DeviceType.Gamepad 
                && sData.GetNumberOfAvailableBindings(sData.actionReference, true) > 1)
            {
                sData.bindingIDInAvailableList = EditorGUILayout.IntSlider("Binding ID In List", sData.bindingIDInAvailableList, 0, sData.GetNumberOfAvailableBindings(sData.actionReference, true)-1);
            }
            else
                sData.bindingIDInAvailableList = 0;

        }

        public static void DrawDeviceSpritesNormalMode(II_PromptData sData)
        {
            int witdh = 130;

            EditorGUILayout.BeginHorizontal();


            EditorGUILayout.BeginVertical(BackgroundStyle.Get(BackgroundStyle.GetKeyboardColor()));
            EditorGUILayout.LabelField("Keyboard", GUILayout.Width(witdh));
            if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                EditorGUILayout.ObjectField("", sData.GetKeySpriteKeyboard(), typeof(Sprite), false, GUILayout.Width(witdh));
            else
                EditorGUILayout.LabelField("", GUILayout.Width(witdh));
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(BackgroundStyle.Get(BackgroundStyle.GetGamepadColor()));
            EditorGUILayout.LabelField("Gamepad", GUILayout.Width(witdh));
            if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
                EditorGUILayout.ObjectField("", sData.GetKeySpriteGamepad(), typeof(Sprite), false, GUILayout.Width(witdh));
            else
                EditorGUILayout.LabelField("", GUILayout.Width(witdh));
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        public static void DrawKeyboardAdvanced(II_PromptData sData)
        {
            int width = 100;

            EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel, GUILayout.Width(width));
            if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
            {
                
                EditorGUILayout.BeginHorizontal();
                sData.keyboardControlSchemeIndexAdvanced = EditorGUILayout.IntField(sData.keyboardControlSchemeIndexAdvanced, GUILayout.Width(20));

                if (GUILayout.Button("-", GUILayout.Width(20)))
                    sData.keyboardControlSchemeIndexAdvanced--;

                if (GUILayout.Button("+", GUILayout.Width(20)))
                    sData.keyboardControlSchemeIndexAdvanced++;

                sData.keyboardControlSchemeIndexAdvanced = Mathf.Clamp(sData.keyboardControlSchemeIndexAdvanced, 0, InputIconsManagerSO.Instance.controlSchemeNames_Keyboard.Count - 1);

                EditorGUILayout.EndHorizontal();


                
                HashSet<InputIconsUtility.CompositeType> foundCompositeTypes = sData.GetAvailableCompositeTypesOfActionAdvanced(sData.actionReference, false);
                if (foundCompositeTypes.Count > 0)
                {
                    if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite)
                        && foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        EditorGUI.BeginDisabledGroup(false);
                        
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite))
                    {
                        sData.compositeTypeKeyboardAdvanced = InputIconsUtility.CompositeType.NonComposite;
                        EditorGUI.BeginDisabledGroup(true);
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        sData.compositeTypeKeyboardAdvanced = InputIconsUtility.CompositeType.Composite;
                        EditorGUI.BeginDisabledGroup(true);
                    }

                    sData.compositeTypeKeyboardAdvanced = (InputIconsUtility.CompositeType)EditorGUILayout.EnumPopup(sData.compositeTypeKeyboardAdvanced, GUILayout.Width(width));
                    EditorGUI.EndDisabledGroup();
                }
                else
                    EditorGUILayout.LabelField("", GUILayout.Width(width));


                if (sData.compositeTypeKeyboardAdvanced == InputIconsUtility.CompositeType.Composite)
                {
                    sData.bindingTypeKeyboardAdvanced = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup(sData.bindingTypeKeyboardAdvanced, GUILayout.Width(width));
                }
                else
                    EditorGUILayout.LabelField("None", GUILayout.Width(width));


                if (sData.GetNumberOfAvailableBindingGroupsKeyboardAdvanced(sData.actionReference) > 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    sData.bindingIDInAvailableListKeyboardAdvanced = EditorGUILayout.IntField(sData.bindingIDInAvailableListKeyboardAdvanced, GUILayout.Width(20));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        sData.bindingIDInAvailableListKeyboardAdvanced--;

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        sData.bindingIDInAvailableListKeyboardAdvanced++;

                    sData.bindingIDInAvailableListKeyboardAdvanced = Mathf.Clamp(sData.bindingIDInAvailableListKeyboardAdvanced, 0, sData.GetNumberOfAvailableBindingGroupsKeyboardAdvanced(sData.actionReference) - 1);

                    EditorGUILayout.EndHorizontal();

                }
                else
                {
                    sData.bindingIDInAvailableListKeyboardAdvanced = 0;
                    EditorGUILayout.LabelField("0", GUILayout.Width(width));
                }
            } 
        }

        public static void DrawSpriteField(Sprite sprite, int width, II_PromptData sdata, bool isGamepad)
        {
            if((sdata.deviceType == InputIconsUtility.DeviceType.Gamepad && isGamepad)
                ||(sdata.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse && !isGamepad)
                || sdata.deviceType == InputIconsUtility.DeviceType.Auto)
            {
                EditorGUILayout.ObjectField("", sprite, typeof(Sprite), false, GUILayout.Width(width));
            } 
        }



        public static void DrawGamepadAdvanced(II_PromptData sData)
        {
            int width = 100;
            EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(width));
            if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
            {
                
                EditorGUILayout.BeginHorizontal();
                sData.gamepadControlSchemeIndexAdvanced = EditorGUILayout.IntField(sData.gamepadControlSchemeIndexAdvanced, GUILayout.Width(20));

                if (GUILayout.Button("-", GUILayout.Width(20)))
                    sData.gamepadControlSchemeIndexAdvanced--;

                if (GUILayout.Button("+", GUILayout.Width(20)))
                    sData.gamepadControlSchemeIndexAdvanced++;

                sData.gamepadControlSchemeIndexAdvanced = Mathf.Clamp(sData.gamepadControlSchemeIndexAdvanced, 0, InputIconsManagerSO.Instance.controlSchemeNames_Gamepad.Count - 1);

                EditorGUILayout.EndHorizontal();



                HashSet<InputIconsUtility.CompositeType> foundCompositeTypes = sData.GetAvailableCompositeTypesOfActionAdvanced(sData.actionReference, true);
                if (foundCompositeTypes.Count > 0)
                {
                    if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite)
                        && foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        EditorGUI.BeginDisabledGroup(false);
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite))
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        sData.compositeTypeGamepadAdvanced = InputIconsUtility.CompositeType.NonComposite;
                    }
                    else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        sData.compositeTypeGamepadAdvanced = InputIconsUtility.CompositeType.Composite;
                    }

                    sData.compositeTypeGamepadAdvanced = (InputIconsUtility.CompositeType)EditorGUILayout.EnumPopup(sData.compositeTypeGamepadAdvanced, GUILayout.Width(width - 10));
                    EditorGUI.EndDisabledGroup();
                }
                else
                    EditorGUILayout.LabelField("", GUILayout.Width(width));



                if (sData.compositeTypeGamepadAdvanced == InputIconsUtility.CompositeType.Composite)
                {
                    sData.bindingTypeGamepadAdvanced = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup(sData.bindingTypeGamepadAdvanced, GUILayout.Width(width));
                }
                else
                    EditorGUILayout.LabelField("None", GUILayout.Width(width));


                if (sData.GetNumberOfAvailableBindingGroupsGamepadAdvanced(sData.actionReference) > 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    sData.bindingIDInAvailableListGamepadAdvanced = EditorGUILayout.IntField(sData.bindingIDInAvailableListGamepadAdvanced, GUILayout.Width(20));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        sData.bindingIDInAvailableListGamepadAdvanced--;

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        sData.bindingIDInAvailableListGamepadAdvanced++;

                    sData.bindingIDInAvailableListGamepadAdvanced = Mathf.Clamp(sData.bindingIDInAvailableListGamepadAdvanced, 0, sData.GetNumberOfAvailableBindingGroupsGamepadAdvanced(sData.actionReference) - 1);

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    sData.bindingIDInAvailableListGamepadAdvanced = 0;
                    EditorGUILayout.LabelField("0", GUILayout.Width(width));
                }

                EditorGUILayout.Space(5);

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
                if (EditorGUIUtility.isProSkin)
                    return new Color(0.2f, 0.2f, 0.2f);
                else
                    return new Color(0.68f, 0.68f, 0.68f);
            }

            public static Color GetKeyboardColor()
            {
                if (EditorGUIUtility.isProSkin)
                    return new Color(0.15f, 0.22f, 0.15f);
                else
                    return new Color(0.45f, 0.90f, 0.42f);
            }

            public static Color GetGamepadColor()
            {
                if (EditorGUIUtility.isProSkin)
                    return new Color(0.15f, 0.18f, 0.25f);
                else
                    return new Color(0.40f, 0.75f, 0.93f);
            }
        }

        public static class HeaderStyle
        {
            private static GUIStyle style = new GUIStyle();

            public static GUIStyle Get()
            {
                if (EditorGUIUtility.isProSkin)
                    style.normal.textColor = Color.white;
                else
                    style.normal.textColor = Color.black;

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

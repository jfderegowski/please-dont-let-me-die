using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputIcons
{
    [CustomEditor(typeof(II_TextPromptDataSO))]
    public class II_TextPromptDataSOEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            II_TextPromptDataSO textPromptBehaviour = (II_TextPromptDataSO)target;

            List<II_TextPrompt.TextPromptData> savedPromptData = II_TextPrompt.TextPromptData.CloneList(textPromptBehaviour.textPromptDatas);

            EditorGUI.BeginChangeCheck();

            int currentIndex = -1;
            foreach (II_TextPrompt.TextPromptData sData in savedPromptData.ToList())
            {
                currentIndex++;
                if (sData.actionReference != null)
                {
                    EditorGUILayout.LabelField(sData.actionReference.action.name, II_SpritePromptEditor.HeaderStyle.Get(), GUILayout.Width(150));
                    EditorGUILayout.Space(5);
                }

                sData.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", sData.actionReference, typeof(InputActionReference), false);


                if (sData.actionReference != null)
                {

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    sData.showAllInfo = EditorGUILayout.Foldout(sData.showAllInfo, "Controls");
                    if (sData.showAllInfo)
                    {
                        sData.actionDisplayType = (II_TextPrompt.TextPromptData.ActionDisplayType)EditorGUILayout.EnumPopup("Action Display Type", sData.actionDisplayType);

                        //for single binding display
                        if (sData.actionDisplayType == II_TextPrompt.TextPromptData.ActionDisplayType.SingleBinding)
                        {
                            sData.bindingSearchStrategy = (II_TextPrompt.TextPromptData.BindingSearchStrategy)EditorGUILayout.EnumPopup("Search Binding By", sData.bindingSearchStrategy);
                            if (sData.bindingSearchStrategy == II_TextPrompt.TextPromptData.BindingSearchStrategy.BindingType)
                            {
                                sData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", sData.deviceType);

                                sData.advancedMode = EditorGUILayout.Toggle("Advanced Mode", sData.advancedMode);

                                if (sData.advancedMode)
                                {
                                    int widthTitles = 140;

                                    EditorGUILayout.BeginHorizontal();

                                    EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetDefaultColor()));
                                    EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                                    EditorGUILayout.LabelField("Control Scheme Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                                    EditorGUILayout.LabelField("Composite Types", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                                    EditorGUILayout.LabelField("Binding Types", EditorStyles.boldLabel, GUILayout.Width(widthTitles));

                                    EditorGUILayout.LabelField("Available Binding Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));

                                    EditorGUILayout.EndVertical();


                                    EditorGUILayout.Space(5);
                                    //Keyboard controls
                                    EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetKeyboardColor()));

                                    II_SpritePromptEditor.DrawKeyboardAdvanced(sData);
                                    II_SpritePromptEditor.DrawSpriteField(sData.GetSpriteKeyboardAdvanced(), widthTitles, sData, false);

                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.Space(5);

                                    //Gamepad controls
                                    EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));
                                    II_SpritePromptEditor.DrawGamepadAdvanced(sData);
                                    II_SpritePromptEditor.DrawSpriteField(sData.GetSpriteGamepadAdvanced(), widthTitles, sData, true);
                                    EditorGUILayout.EndVertical();


                                    GUILayout.FlexibleSpace();
                                    EditorGUILayout.Space(50);
                                    GUILayout.FlexibleSpace();

                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space(15);
                                }
                                else
                                {
                                    II_SpritePromptEditor.DrawNormalModeControls(sData);
                                    II_SpritePromptEditor.DrawDeviceSpritesNormalMode(sData);
                                }



                            }
                            else if (sData.bindingSearchStrategy == II_TextPrompt.TextPromptData.BindingSearchStrategy.BindingIndex)
                            {
                                II_SpritePromptEditor.DrawSearchByBindingIndexFields(sData);


                            }


                        }

                        //All Matching Bindings Single
                        else if (sData.actionDisplayType == II_TextPrompt.TextPromptData.ActionDisplayType.AllMatchingBindingsSingle)
                        {
                            sData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", sData.deviceType);

                            sData.advancedMode = EditorGUILayout.Toggle("Advanced Mode", sData.advancedMode);

                            if (sData.advancedMode)
                            {
                                int widthTitles = 140;

                                EditorGUILayout.BeginHorizontal();

                                EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetDefaultColor()));
                                EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                                EditorGUILayout.LabelField("Control Scheme Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                                EditorGUILayout.LabelField("Available Binding Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));

                                EditorGUILayout.EndVertical();


                                EditorGUILayout.Space(5);
                                //Keyboard controls
                                EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetKeyboardColor()));

                                DrawKeyboardAdvanced(sData);

                                List<Sprite> sprites = sData.GetSpritesKeyboardAllMatchingBindingsSingle(true);
                                //Debug.Log("sprite count: " + sprites.Count);
                                for (int i = 0; i < sprites.Count; i++)
                                {
                                    II_SpritePromptEditor.DrawSpriteField(sprites[i], widthTitles, sData, false);
                                }


                                EditorGUILayout.EndVertical();


                                EditorGUILayout.Space(5);

                                //Gamepad controls
                                EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));

                                DrawGamepadAdvanced(sData);
                                List<Sprite> spritesGamepad = sData.GetSpritesGamepadAllMatchingBindingsSingle(true);
                                //Debug.Log("sprite count: " + sprites.Count);
                                for (int i = 0; i < spritesGamepad.Count; i++)
                                {
                                    II_SpritePromptEditor.DrawSpriteField(spritesGamepad[i], widthTitles, sData, true);
                                }
                                EditorGUILayout.EndVertical();


                                GUILayout.FlexibleSpace();
                                EditorGUILayout.Space(50);
                                GUILayout.FlexibleSpace();

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space(15);
                            }
                            else
                            {
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



                                if (sData.GetNumberOfAvailableBindingsAllMatchingBindings(sData.actionReference) > 1)
                                {
                                    sData.bindingIDInAvailableListAllMatchingBindings = EditorGUILayout.IntSlider("Binding ID All List", sData.bindingIDInAvailableListAllMatchingBindings, 0, sData.GetNumberOfAvailableBindingsAllMatchingBindings(sData.actionReference) - 1);
                                }
                                else
                                    sData.bindingIDInAvailableListAllMatchingBindings = 0;



                                int width = 110;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetKeyboardColor()));

                                EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel, GUILayout.Width(width));
                                List<Sprite> sprites = sData.GetSpritesKeyboardAllMatchingBindingsSingle(false);
                                //Debug.Log("sprite count: " + sprites.Count);
                                for (int i = 0; i < sprites.Count; i++)
                                {
                                    II_SpritePromptEditor.DrawSpriteField(sprites[i], width, sData, false);
                                }
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));

                                EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(width));
                                List<Sprite> spritesGamepad = sData.GetSpritesGamepadAllMatchingBindingsSingle(false);
                                //Debug.Log("sprite count: " + sprites.Count);
                                for (int i = 0; i < spritesGamepad.Count; i++)
                                {
                                    II_SpritePromptEditor.DrawSpriteField(spritesGamepad[i], width, sData, true);
                                }
                                EditorGUILayout.EndVertical();

                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndHorizontal();
                            }




                        }

                        //All Matching Bindings (no advanced mode necessary)
                        else
                        {
                            sData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", sData.deviceType);
                            sData.delimiter = EditorGUILayout.TextField("Delimiter", sData.delimiter);

                            if (InputIconsManagerSO.GetKeyboardControlSchemeCountOfAction(sData.actionReference) > 1)
                            {
                                if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                                    sData.keyboardControlSchemeIndex = EditorGUILayout.IntSlider("Keyboard Control Scheme Index", sData.keyboardControlSchemeIndex, 0, InputIconsManagerSO.Instance.controlSchemeNames_Keyboard.Count - 1);
                            }
                            else
                                sData.AutoassignControlSchemeIndexKeyboard();

                            if (InputIconsManagerSO.GetGamepadControlSchemeCountOfAction(sData.actionReference) > 1)
                            {
                                if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
                                    sData.gamepadControlSchemeIndex = EditorGUILayout.IntSlider("Gamepad Control Scheme Index", sData.gamepadControlSchemeIndex, 0, InputIconsManagerSO.Instance.controlSchemeNames_Gamepad.Count - 1);
                                else
                                    sData.AutoassignControlSchemeIndexGamepad();
                            }


                            int width = 110;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetKeyboardColor()));

                            EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel, GUILayout.Width(width));
                            List<Sprite> sprites = sData.GetSpritesKeyboardAllMatchingBindings(false);
                            //Debug.Log("sprite count: " + sprites.Count);
                            for (int i = 0; i < sprites.Count; i++)
                            {
                                II_SpritePromptEditor.DrawSpriteField(sprites[i], width, sData, false);
                            }
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));

                            EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(width));
                            List<Sprite> spritesGamepad = sData.GetSpritesGamepadAllMatchingBindings(false);
                            //Debug.Log("sprite count: " + sprites.Count);
                            for (int i = 0; i < spritesGamepad.Count; i++)
                            {
                                II_SpritePromptEditor.DrawSpriteField(spritesGamepad[i], width, sData, true);
                            }
                            EditorGUILayout.EndVertical();

                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();

                        }

                        sData.allowTinting = EditorGUILayout.Toggle("Allow Sprite Tinting", sData.allowTinting);
                        sData.useSDFFont = EditorGUILayout.Toggle("Use Font (experimental, icon switching not supported)", sData.useSDFFont);

                        if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                        {
                            sData.optionalKeyboardIconSet = (InputIconSetKeyboardSO)EditorGUILayout.ObjectField("(Optional) Keyboard Icon Set", sData.optionalKeyboardIconSet, typeof(InputIconSetKeyboardSO), true);
                        }

                        if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
                        {
                            sData.optionalGamepadIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("(Optional) Gamepad Icon Set", sData.optionalGamepadIconSet, typeof(InputIconSetGamepadSO), true);
                        }
                    }

                    //EditorGUILayout.EndVertical();

                    //sData. = (SpriteRenderer)EditorGUILayout.ObjectField("Sprite Renderer", sData.spriteRenderer, typeof(SpriteRenderer), true);



                    EditorGUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Move Up", EditorStyles.miniButtonRight, GUILayout.Width(80)))
                    {
                        if (textPromptBehaviour.CanMoveUp(currentIndex))
                        {
                            Undo.RecordObject(target, "Prompt Data Changed");
                            textPromptBehaviour.textPromptDatas = new List<II_TextPrompt.TextPromptData>(savedPromptData);
                        }

                        textPromptBehaviour.MoveDataUp(currentIndex);

                        //textPromptBehaviour.OnValidate();
                        EditorUtility.SetDirty(target);
                        return;
                    }
                    if (GUILayout.Button("Move Down", EditorStyles.miniButtonRight, GUILayout.Width(80)))
                    {
                        if (textPromptBehaviour.CanMoveDown(currentIndex))
                        {
                            Undo.RecordObject(target, "Prompt Data Changed");
                            textPromptBehaviour.textPromptDatas = new List<II_TextPrompt.TextPromptData>(savedPromptData);
                        }

                        textPromptBehaviour.MoveDataDown(currentIndex);

                        //textPromptBehaviour.OnValidate();
                        EditorUtility.SetDirty(target);
                        return;
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                    {
                        savedPromptData.Remove(sData);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space(20);
                    II_SpritePromptEditor.DrawUILine(Color.grey);

                }




            }


            if (GUILayout.Button("Add"))
            {
                if (savedPromptData.Count > 0)
                    savedPromptData.Add(new II_TextPrompt.TextPromptData(savedPromptData[savedPromptData.Count - 1]));
                else
                    savedPromptData.Add(new II_TextPrompt.TextPromptData());
            }



            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Prompt Data Changed");
                textPromptBehaviour.textPromptDatas = new List<II_TextPrompt.TextPromptData>(savedPromptData);

                //textPromptBehaviour.OnValidate();
                EditorUtility.SetDirty(target);
            }

        }





        public static void DrawKeyboardAdvanced(II_TextPrompt.TextPromptData sData)
        {
            int width = 100;

            EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel, GUILayout.Width(width));

            if (sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
                return;


            EditorGUILayout.BeginHorizontal();
            sData.keyboardControlSchemeIndexAdvanced = EditorGUILayout.IntField(sData.keyboardControlSchemeIndexAdvanced, GUILayout.Width(20));

            if (GUILayout.Button("-", GUILayout.Width(20)))
                sData.keyboardControlSchemeIndexAdvanced--;

            if (GUILayout.Button("+", GUILayout.Width(20)))
                sData.keyboardControlSchemeIndexAdvanced++;

            sData.keyboardControlSchemeIndexAdvanced = Mathf.Clamp(sData.keyboardControlSchemeIndexAdvanced, 0, InputIconsManagerSO.Instance.controlSchemeNames_Keyboard.Count - 1);

            EditorGUILayout.EndHorizontal();



            int keyboardGroupCount = sData.GetGroupCountAllSpritesSingleKeyboard();
            if (keyboardGroupCount > 1)
            {
                EditorGUILayout.BeginHorizontal();
                sData.bindingIDInAvailableListAllMatchingBindingsSingleKeyboard = EditorGUILayout.IntField(sData.bindingIDInAvailableListAllMatchingBindingsSingleKeyboard, GUILayout.Width(20));

                if (GUILayout.Button("-", GUILayout.Width(20)))
                    sData.bindingIDInAvailableListAllMatchingBindingsSingleKeyboard--;

                if (GUILayout.Button("+", GUILayout.Width(20)))
                    sData.bindingIDInAvailableListAllMatchingBindingsSingleKeyboard++;

                sData.bindingIDInAvailableListAllMatchingBindingsSingleKeyboard = Mathf.Clamp(sData.bindingIDInAvailableListAllMatchingBindingsSingleKeyboard, 0, keyboardGroupCount - 1);

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("0", GUILayout.Width(width));
                sData.bindingIDInAvailableListAllMatchingBindingsSingleKeyboard = 0;
            }

            EditorGUILayout.Space(5);
        }

        public static void DrawGamepadAdvanced(II_TextPrompt.TextPromptData sData)
        {
            int width = 100;

            EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(width));

            if (sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                return;


            EditorGUILayout.BeginHorizontal();
            sData.gamepadControlSchemeIndexAdvanced = EditorGUILayout.IntField(sData.gamepadControlSchemeIndexAdvanced, GUILayout.Width(20));

            if (GUILayout.Button("-", GUILayout.Width(20)))
                sData.gamepadControlSchemeIndexAdvanced--;

            if (GUILayout.Button("+", GUILayout.Width(20)))
                sData.gamepadControlSchemeIndexAdvanced++;

            sData.gamepadControlSchemeIndexAdvanced = Mathf.Clamp(sData.gamepadControlSchemeIndexAdvanced, 0, InputIconsManagerSO.Instance.controlSchemeNames_Gamepad.Count - 1);
            EditorGUILayout.EndHorizontal();



            int keyboardGroupCount = sData.GetGroupCountAllSpritesSingleGamepad();
            if (keyboardGroupCount > 1)
            {
                EditorGUILayout.BeginHorizontal();
                sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad = EditorGUILayout.IntField(sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad, GUILayout.Width(20));

                if (GUILayout.Button("-", GUILayout.Width(20)))
                    sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad--;

                if (GUILayout.Button("+", GUILayout.Width(20)))
                    sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad++;

                sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad = Mathf.Clamp(sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad, 0, keyboardGroupCount - 1);

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("0", GUILayout.Width(width));
                sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad = 0;
            }

            EditorGUILayout.Space(5);
        }

        public static void DrawKeyboardAllAdvanced(II_TextPrompt.TextPromptData sData)
        {
            int width = 100;

            EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel, GUILayout.Width(width));

            if (sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
                return;

            if (InputIconsManagerSO.GetKeyboardControlSchemeCountOfAction(sData.actionReference) > 1)
            {
                EditorGUILayout.BeginHorizontal();
                sData.keyboardControlSchemeIndexAdvanced = EditorGUILayout.IntField(sData.keyboardControlSchemeIndexAdvanced, GUILayout.Width(20));

                if (GUILayout.Button("-", GUILayout.Width(20)))
                    sData.keyboardControlSchemeIndexAdvanced--;

                if (GUILayout.Button("+", GUILayout.Width(20)))
                    sData.keyboardControlSchemeIndexAdvanced++;

                sData.keyboardControlSchemeIndexAdvanced = Mathf.Clamp(sData.keyboardControlSchemeIndexAdvanced, 0, InputIconsManagerSO.Instance.controlSchemeNames_Keyboard.Count - 1);

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("0", GUILayout.Width(width));
                sData.keyboardControlSchemeIndexAdvanced = 0;
            }


            EditorGUILayout.Space(5);


        }

        public static void DrawGamepadAllAdvanced(II_TextPrompt.TextPromptData sData)
        {
            int width = 100;

            EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(width));

            if (sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                return;

            if (InputIconsManagerSO.GetKeyboardControlSchemeCountOfAction(sData.actionReference) > 1)
            {
                EditorGUILayout.BeginHorizontal();
                sData.gamepadControlSchemeIndexAdvanced = EditorGUILayout.IntField(sData.gamepadControlSchemeIndexAdvanced, GUILayout.Width(20));

                if (GUILayout.Button("-", GUILayout.Width(20)))
                    sData.gamepadControlSchemeIndexAdvanced--;

                if (GUILayout.Button("+", GUILayout.Width(20)))
                    sData.gamepadControlSchemeIndexAdvanced++;

                sData.gamepadControlSchemeIndexAdvanced = Mathf.Clamp(sData.gamepadControlSchemeIndexAdvanced, 0, InputIconsManagerSO.Instance.controlSchemeNames_Gamepad.Count - 1);

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("0", GUILayout.Width(width));
                sData.gamepadControlSchemeIndexAdvanced = 0;
            }


            EditorGUILayout.Space(5);


        }

    }
}


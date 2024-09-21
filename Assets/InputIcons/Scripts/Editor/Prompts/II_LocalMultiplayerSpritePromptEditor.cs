using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace InputIcons
{
    [CustomEditor(typeof(II_LocalMultiplayerSpritePrompt))]
    public class II_LocalMultiplayerSpritePromptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            II_LocalMultiplayerSpritePrompt spritePrompts = (II_LocalMultiplayerSpritePrompt)target;
            List<II_LocalMultiplayerSpritePrompt.SpritePromptData> savedPromptData = II_LocalMultiplayerSpritePrompt.SpritePromptData.CloneList(spritePrompts.spritePromptDatas);

            EditorGUI.BeginChangeCheck();


            foreach (II_LocalMultiplayerSpritePrompt.SpritePromptData sData in savedPromptData.ToList())
            {
                if (sData.actionReference != null)
                {
                    EditorGUILayout.LabelField(sData.actionReference.action.name, II_SpritePromptEditor.HeaderStyle.Get(), GUILayout.Width(150));
                    EditorGUILayout.Space(5);
                }

                sData.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", sData.actionReference, typeof(InputActionReference), false);

                if (sData.actionReference != null)
                {
                    sData.playerID = EditorGUILayout.IntField("Player ID", sData.playerID);

                    if (Application.isPlaying)
                    {
                        InputDevice device = InputIconsManagerSO.localMultiplayerManagement.GetDeviceForPlayer(sData.playerID);
                        if (device != null)
                        {
                            EditorGUILayout.LabelField("Device is " + device.name, EditorStyles.boldLabel, GUILayout.Width(250));
                            string playerControlScheme = InputIconsManagerSO.localMultiplayerManagement.GetControlSchemeForPlayer(sData.playerID);
                            EditorGUILayout.LabelField("Control scheme is '" + playerControlScheme + "'", EditorStyles.boldLabel, GUILayout.Width(250));
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No device yet", EditorStyles.boldLabel, GUILayout.Width(250));
                        }
                    }


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


                    EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetKeyboardColor()));
                    EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                    DrawKeyboard(sData);
                    DrawSpriteField(sData.GetSpriteKeyboard(), widthTitles, sData, false);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));
                    EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                    DrawGamepad(sData);
                    DrawSpriteField(sData.GetSpriteGamepad(), widthTitles, sData, false);

                    EditorGUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();



                    EditorGUILayout.Space(5);
                    if (sData.spriteRenderer == null)
                    {
                        EditorGUILayout.Space(10);
                        EditorGUILayout.HelpBox("Assign a Sprite Renderer to display the binding in the scene", MessageType.Warning);
                    }
                    sData.spriteRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Sprite Renderer", sData.spriteRenderer, typeof(SpriteRenderer), true);
                    sData.optionalKeyboardIconSet = (InputIconSetKeyboardSO)EditorGUILayout.ObjectField("(Optional) Keyboard Icon Set", sData.optionalKeyboardIconSet, typeof(InputIconSetKeyboardSO), true);
                    sData.optionalGamepadIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("(Optional) Gamepad Icon Set", sData.optionalGamepadIconSet, typeof(InputIconSetGamepadSO), true);




                }


                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                {
                    savedPromptData.Remove(sData);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(20);

            }


            if (GUILayout.Button("Add"))
            {
                if (savedPromptData.Count > 0)
                {
                    savedPromptData.Add(new II_LocalMultiplayerSpritePrompt.SpritePromptData(savedPromptData[savedPromptData.Count - 1]));
                }
                else
                    savedPromptData.Add(new II_LocalMultiplayerSpritePrompt.SpritePromptData());

            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Prompt Data Changed");
                spritePrompts.spritePromptDatas = new List<II_LocalMultiplayerSpritePrompt.SpritePromptData>(savedPromptData);

                spritePrompts.OnValidate();
                EditorUtility.SetDirty(target);

            }

        }

        public static void DrawSpriteField(Sprite sprite, int width, II_LocalMultiplayerSpritePrompt.II_PromptDataLocalMultiplayer sdata, bool isGamepad)
        {

            EditorGUILayout.ObjectField("", sprite, typeof(Sprite), false, GUILayout.Width(width));

        }

        public static void DrawKeyboard(II_LocalMultiplayerSpritePrompt.II_PromptDataLocalMultiplayer sData)
        {
            int width = 100;

            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                string controlScheme = InputIconsManagerSO.localMultiplayerManagement.GetControlSchemeForPlayer(sData.playerID);
                EditorGUILayout.IntField(InputIconsManagerSO.GetKeyboardControlSchemeID(controlScheme), GUILayout.Width(20));
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (InputIconsManagerSO.GetKeyboardControlSchemeCountOfAction(sData.actionReference) > 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    sData.controlSchemeIndexKeyboard = EditorGUILayout.IntField(sData.controlSchemeIndexKeyboard, GUILayout.Width(20));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        sData.controlSchemeIndexKeyboard--;

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        sData.controlSchemeIndexKeyboard++;

                    sData.controlSchemeIndexKeyboard = Mathf.Clamp(sData.controlSchemeIndexKeyboard, 0, InputIconsManagerSO.Instance.controlSchemeNames_Keyboard.Count - 1);

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    sData.AutoassignControlSchemeIndexKeyboard();
                    EditorGUILayout.LabelField(sData.controlSchemeIndexKeyboard.ToString(), GUILayout.Width(width));
                }
            }



            HashSet<InputIconsUtility.CompositeType> foundCompositeTypes = sData.GetAvailableCompositeTypesOfAction(sData.actionReference, false);
            if (foundCompositeTypes.Count > 0)
            {
                if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite)
                    && foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                {
                    sData.compositeTypeKeyboard = (InputIconsUtility.CompositeType)EditorGUILayout.EnumPopup(sData.compositeTypeKeyboard, GUILayout.Width(width));
                }
                else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite))
                {
                    sData.compositeTypeKeyboard = InputIconsUtility.CompositeType.NonComposite;
                    EditorGUILayout.LabelField("Non-Composite", GUILayout.Width(width));
                }
                else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                {
                    sData.compositeTypeKeyboard = InputIconsUtility.CompositeType.Composite;
                    EditorGUILayout.LabelField("Composite", GUILayout.Width(width));
                }

            }
            else
            {
                sData.compositeTypeKeyboard = InputIconsUtility.CompositeType.NonComposite;
                EditorGUILayout.LabelField("Non-Composite", GUILayout.Width(width));
            }


            if (sData.compositeTypeKeyboard == InputIconsUtility.CompositeType.Composite)
            {
                sData.bindingTypeKeyboard = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup(sData.bindingTypeKeyboard, GUILayout.Width(width));
            }
            else
                EditorGUILayout.LabelField("None", GUILayout.Width(width));


            if (sData.GetNumberOfAvailableBindingGroupsKeyboard(sData.actionReference) > 1)
            {
                EditorGUILayout.BeginHorizontal();
                sData.bindingIndexKeyboard = EditorGUILayout.IntField(sData.bindingIndexKeyboard, GUILayout.Width(20));

                if (GUILayout.Button("-", GUILayout.Width(20)))
                    sData.bindingIndexKeyboard--;

                if (GUILayout.Button("+", GUILayout.Width(20)))
                    sData.bindingIndexKeyboard++;

                sData.bindingIndexKeyboard = Mathf.Clamp(sData.bindingIndexKeyboard, 0, sData.GetNumberOfAvailableBindingGroupsKeyboard(sData.actionReference) - 1);

                EditorGUILayout.EndHorizontal();

            }
            else
            {
                sData.bindingIndexKeyboard = 0;
                EditorGUILayout.LabelField("0", GUILayout.Width(width));
            }

        }


        public static void DrawGamepad(II_LocalMultiplayerSpritePrompt.II_PromptDataLocalMultiplayer sData)
        {
            int width = 100;

            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                string controlScheme = InputIconsManagerSO.localMultiplayerManagement.GetControlSchemeForPlayer(sData.playerID);
                EditorGUILayout.IntField(InputIconsManagerSO.GetGamepadControlSchemeID(controlScheme), GUILayout.Width(20));
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (InputIconsManagerSO.GetGamepadControlSchemeCountOfAction(sData.actionReference) > 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    sData.controlSchemeIndexGamepad = EditorGUILayout.IntField(sData.controlSchemeIndexGamepad, GUILayout.Width(20));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        sData.controlSchemeIndexGamepad--;

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        sData.controlSchemeIndexGamepad++;

                    sData.controlSchemeIndexGamepad = Mathf.Clamp(sData.controlSchemeIndexGamepad, 0, InputIconsManagerSO.Instance.controlSchemeNames_Gamepad.Count - 1);

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    sData.AutoassignControlSchemeIndexGamepad();
                    EditorGUILayout.LabelField(sData.controlSchemeIndexGamepad.ToString(), GUILayout.Width(width));
                }
            }




            HashSet<InputIconsUtility.CompositeType> foundCompositeTypes = sData.GetAvailableCompositeTypesOfAction(sData.actionReference, true);
            if (foundCompositeTypes.Count > 0)
            {
                if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite)
                    && foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                {
                    sData.compositeTypeGamepad = (InputIconsUtility.CompositeType)EditorGUILayout.EnumPopup(sData.compositeTypeGamepad, GUILayout.Width(width));
                }
                else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.NonComposite))
                {
                    sData.compositeTypeGamepad = InputIconsUtility.CompositeType.NonComposite;
                    EditorGUILayout.LabelField("Non-Composite", GUILayout.Width(width));
                }
                else if (foundCompositeTypes.Contains(InputIconsUtility.CompositeType.Composite))
                {
                    sData.compositeTypeGamepad = InputIconsUtility.CompositeType.Composite;
                    EditorGUILayout.LabelField("Composite", GUILayout.Width(width));
                }

            }
            else
            {
                sData.compositeTypeGamepad = InputIconsUtility.CompositeType.NonComposite;
                EditorGUILayout.LabelField("Non-Composite", GUILayout.Width(width));
            }


            if (sData.compositeTypeGamepad == InputIconsUtility.CompositeType.Composite)
            {
                sData.bindingTypeGamepad = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup(sData.bindingTypeGamepad, GUILayout.Width(width));
            }
            else
                EditorGUILayout.LabelField("None", GUILayout.Width(width));


            if (sData.GetNumberOfAvailableBindingGroupsGamepad(sData.actionReference) > 1)
            {
                //Debug.Log("ffffouuund binding count: " + sData.GetNumberOfAvailableBindingGroups(sData.actionReference));


                EditorGUILayout.BeginHorizontal();
                sData.bindingIndexGamepad = EditorGUILayout.IntField(sData.bindingIndexGamepad, GUILayout.Width(20));

                if (GUILayout.Button("-", GUILayout.Width(20)))
                    sData.bindingIndexGamepad--;

                if (GUILayout.Button("+", GUILayout.Width(20)))
                    sData.bindingIndexGamepad++;

                sData.bindingIndexGamepad = Mathf.Clamp(sData.bindingIndexGamepad, 0, sData.GetNumberOfAvailableBindingGroupsGamepad(sData.actionReference) - 1);

                EditorGUILayout.EndHorizontal();

            }
            else
            {
                sData.bindingIndexGamepad = 0;
                EditorGUILayout.LabelField("0", GUILayout.Width(width));
            }

        }
    }

}

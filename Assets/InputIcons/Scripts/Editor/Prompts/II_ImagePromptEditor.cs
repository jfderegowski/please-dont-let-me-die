using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InputIcons.II_ImagePrompt;

namespace InputIcons
{
    [CustomEditor(typeof(II_ImagePrompt))]
    public class II_ImagePromptEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            II_ImagePrompt imagePromptBehaviour = (II_ImagePrompt)target;
            
            List<ImagePromptData> savedPromptData = ImagePromptData.CloneList(imagePromptBehaviour.spritePromptDatas);

            EditorGUI.BeginChangeCheck();

            foreach (II_ImagePrompt.ImagePromptData sData in savedPromptData.ToList())
            {
                if (sData.actionReference != null)
                {
                    EditorGUILayout.LabelField(sData.actionReference.action.name, II_SpritePromptEditor.HeaderStyle.Get(), GUILayout.Width(150));
                    EditorGUILayout.Space(5);
                }

                sData.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", sData.actionReference, typeof(InputActionReference), false);
                if (sData.actionReference != null)
                {
                    sData.bindingSearchStrategy = (ImagePromptData.BindingSearchStrategy)EditorGUILayout.EnumPopup("Search Binding By", sData.bindingSearchStrategy);
                    if (sData.bindingSearchStrategy == ImagePromptData.BindingSearchStrategy.BindingType)
                    {
                        sData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", sData.deviceType);

                        sData.advancedMode = EditorGUILayout.Toggle("Advanced Mode", sData.advancedMode);

                        if (sData.advancedMode)
                        {
                            int widthTitles = 140;
                            //GUILayout.FlexibleSpace();


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
                        else //normal mode
                        {

                            II_SpritePromptEditor.DrawNormalModeControls(sData);
                            II_SpritePromptEditor.DrawDeviceSpritesNormalMode(sData);
                        }



                    }
                    else if (sData.bindingSearchStrategy == ImagePromptData.BindingSearchStrategy.BindingIndex)
                    {
                        II_SpritePromptEditor.DrawSearchByBindingIndexFields(sData);
                    }
                }


                if (sData.image == null)
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.HelpBox("Assign a UI Image to display binding in scene", MessageType.Warning);
                }
                sData.image = (Image)EditorGUILayout.ObjectField("Image Renderer", sData.image, typeof(Image), true);

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
                II_SpritePromptEditor.DrawUILine(Color.grey);
            }


            if (GUILayout.Button("Add"))
            {
                if (savedPromptData.Count > 0)
                    savedPromptData.Add(new ImagePromptData(savedPromptData[savedPromptData.Count - 1]));
                else
                    savedPromptData.Add(new ImagePromptData());
            }


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Prompt Data Changed");
                imagePromptBehaviour.spritePromptDatas = new List<ImagePromptData>(savedPromptData);

                imagePromptBehaviour.OnValidate();
                EditorUtility.SetDirty(target);
            }
        }

    }


}

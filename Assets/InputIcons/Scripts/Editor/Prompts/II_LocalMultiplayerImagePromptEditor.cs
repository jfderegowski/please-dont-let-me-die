using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

namespace InputIcons
{
    [CustomEditor(typeof(II_LocalMultiplayerImagePrompt))]
    public class II_LocalMultiplayerImagePromptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            II_LocalMultiplayerImagePrompt imagePrompts = (II_LocalMultiplayerImagePrompt)target;
            List<II_LocalMultiplayerImagePrompt.ImagePromptData> savedPromptData = II_LocalMultiplayerImagePrompt.ImagePromptData.CloneList(imagePrompts.spritePromptDatas);


            EditorGUI.BeginChangeCheck();

            foreach (II_LocalMultiplayerImagePrompt.ImagePromptData sData in savedPromptData.ToList())
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
                    II_LocalMultiplayerSpritePromptEditor.DrawKeyboard(sData);
                    II_LocalMultiplayerSpritePromptEditor.DrawSpriteField(sData.GetSpriteKeyboard(), widthTitles, sData, false);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));
                    EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                    II_LocalMultiplayerSpritePromptEditor.DrawGamepad(sData);
                    II_LocalMultiplayerSpritePromptEditor.DrawSpriteField(sData.GetSpriteGamepad(), widthTitles, sData, false);

                    EditorGUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();



                    EditorGUILayout.Space(5);
                    if (sData.image == null)
                    {
                        EditorGUILayout.Space(10);
                        EditorGUILayout.HelpBox("Assign a UI Image to display the binding in the scene", MessageType.Warning);
                    }
                    sData.image = (Image)EditorGUILayout.ObjectField("Image Renderer", sData.image, typeof(Image), true);
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
                    savedPromptData.Add(new II_LocalMultiplayerImagePrompt.ImagePromptData(savedPromptData[savedPromptData.Count - 1]));
                }
                else
                    savedPromptData.Add(new II_LocalMultiplayerImagePrompt.ImagePromptData());

            }


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Prompt Data Changed");
                imagePrompts.spritePromptDatas = new List<II_LocalMultiplayerImagePrompt.ImagePromptData>(savedPromptData);

                imagePrompts.OnValidate();
                EditorUtility.SetDirty(target);

            }

        }
    }

}

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputIcons.II_LocalMultiplayerTextPrompt;

namespace InputIcons
{
    [CustomEditor(typeof(II_LocalMultiplayerTextPromptDataSO))]
    public class II_LocalMultiplayerTextPromptDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
            II_LocalMultiplayerTextPromptDataSO textPromptBehaviour = (II_LocalMultiplayerTextPromptDataSO)target;
        List<II_LocalMultiplayerTextPrompt.TextPromptData> savedPromptData = II_LocalMultiplayerTextPrompt.TextPromptData.CloneList(textPromptBehaviour.textPromptDatas);


        EditorGUI.BeginChangeCheck();

       
        EditorGUILayout.Space(5);
        int currentIndex = -1;
        foreach (TextPromptData sData in savedPromptData.ToList())
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



                    sData.actionDisplayType = (TextPromptData.ActionDisplayType)EditorGUILayout.EnumPopup("Action Display Type", sData.actionDisplayType);

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
                    int widthTitles = 110;

                    //Single binding
                    if (sData.actionDisplayType == TextPromptData.ActionDisplayType.SingleBinding)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetDefaultColor()));
                        EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                        EditorGUILayout.LabelField("Control Scheme Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                        EditorGUILayout.LabelField("Composite Types", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                        EditorGUILayout.LabelField("Binding Types", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                        EditorGUILayout.LabelField("Available Binding Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                        EditorGUILayout.EndVertical();

                        //Keyboard controls
                        EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetKeyboardColor()));
                        II_LocalMultiplayerSpritePromptEditor.DrawKeyboard(sData);
                        II_LocalMultiplayerSpritePromptEditor.DrawSpriteField(sData.GetSpriteKeyboard(), widthTitles, sData, false);
                        EditorGUILayout.EndVertical();


                        //Gamepad controls
                        EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));
                        II_LocalMultiplayerSpritePromptEditor.DrawGamepad(sData);
                        II_LocalMultiplayerSpritePromptEditor.DrawSpriteField(sData.GetSpriteGamepad(), widthTitles, sData, false);
                        EditorGUILayout.EndVertical();

                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                    //All Matching Bindings Single
                    else if (sData.actionDisplayType == TextPromptData.ActionDisplayType.AllMatchingBindingsSingle)
                    {

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetDefaultColor()));
                        EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                        EditorGUILayout.LabelField("Control Scheme Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
                        EditorGUILayout.LabelField("Available Binding Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));

                        EditorGUILayout.EndVertical();


                        EditorGUILayout.Space(5);
                        //Keyboard controls
                        EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetKeyboardColor()));

                        DrawKeyboardAllBindingsSingle(sData);

                        List<Sprite> sprites = sData.GetSpritesKeyboardAllMatchingBindingsSingle();
                        //Debug.Log("sprite count: " + sprites.Count);
                        for (int i = 0; i < sprites.Count; i++)
                        {
                            II_LocalMultiplayerSpritePromptEditor.DrawSpriteField(sprites[i], widthTitles, sData, false);
                        }

                        EditorGUILayout.EndVertical();


                        EditorGUILayout.Space(5);

                        //Gamepad controls
                        EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));

                        DrawGamepadAllBindingsSingle(sData);
                        List<Sprite> spritesGamepad = sData.GetSpritesGamepadAllMatchingBindingsSingle();
                        //Debug.Log("sprite count: " + sprites.Count);
                        for (int i = 0; i < spritesGamepad.Count; i++)
                        {
                            II_LocalMultiplayerSpritePromptEditor.DrawSpriteField(spritesGamepad[i], widthTitles, sData, true);
                        }
                        EditorGUILayout.EndVertical();


                        GUILayout.FlexibleSpace();
                        EditorGUILayout.Space(50);
                        GUILayout.FlexibleSpace();

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(15);
                    }

                    //All Matching Bindings
                    else
                    {
                        sData.delimiter = EditorGUILayout.TextField("Delimiter", sData.delimiter);

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
                                sData.controlSchemeIndexKeyboard = EditorGUILayout.IntSlider("Keyboard Control Scheme Index", sData.controlSchemeIndexKeyboard, 0, InputIconsManagerSO.Instance.controlSchemeNames_Keyboard.Count - 1);
                            else
                                sData.AutoassignControlSchemeIndexKeyboard();

                            if (InputIconsManagerSO.GetGamepadControlSchemeCountOfAction(sData.actionReference) > 1)
                                sData.controlSchemeIndexGamepad = EditorGUILayout.IntSlider("Gamepad Control Scheme Index", sData.controlSchemeIndexGamepad, 0, InputIconsManagerSO.Instance.controlSchemeNames_Gamepad.Count - 1);
                            else
                                sData.AutoassignControlSchemeIndexGamepad();
                        }



                        int width = 110;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetKeyboardColor()));

                        EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel, GUILayout.Width(width));
                        List<Sprite> sprites = sData.GetSpritesKeyboardAllMatchingBindings();
                        //Debug.Log("sprite count: " + sprites.Count);
                        for (int i = 0; i < sprites.Count; i++)
                        {
                            II_LocalMultiplayerSpritePromptEditor.DrawSpriteField(sprites[i], width, sData, false);
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));

                        EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(width));
                        List<Sprite> spritesGamepad = sData.GetSpritesGamepadAllMatchingBindings();
                        //Debug.Log("sprite count: " + sprites.Count);
                        for (int i = 0; i < spritesGamepad.Count; i++)
                        {
                            II_LocalMultiplayerSpritePromptEditor.DrawSpriteField(spritesGamepad[i], width, sData, true);
                        }
                        EditorGUILayout.EndVertical();

                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }


                    sData.allowTinting = EditorGUILayout.Toggle("Allow Tinting", sData.allowTinting);
                    sData.useSDFFont = EditorGUILayout.Toggle("Use Font (experimental, icon switching not supported)", sData.useSDFFont);

                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(5);


                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Move Up", EditorStyles.miniButtonRight, GUILayout.Width(80)))
                {
                    if (textPromptBehaviour.CanMoveUp(currentIndex))
                    {
                        Undo.RecordObject(target, "Prompt Data Changed");
                        textPromptBehaviour.textPromptDatas = new List<TextPromptData>(savedPromptData);
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
                        textPromptBehaviour.textPromptDatas = new List<TextPromptData>(savedPromptData);
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

                EditorGUILayout.Space(20);
            }
        }


        if (GUILayout.Button("Add"))
        {
            if (savedPromptData.Count > 0)
                savedPromptData.Add(new TextPromptData(savedPromptData[savedPromptData.Count - 1]));
            else
                savedPromptData.Add(new TextPromptData());
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Prompt Data Changed");
            textPromptBehaviour.textPromptDatas = new List<TextPromptData>(savedPromptData);

            //textPromptBehaviour.OnValidate();
            EditorUtility.SetDirty(target);
        }

    }




    public static void DrawKeyboardAllBindingsSingle(II_LocalMultiplayerTextPrompt.TextPromptData sData)
    {
        int width = 100;

        EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel, GUILayout.Width(width));

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

    public static void DrawGamepadAllBindingsSingle(II_LocalMultiplayerTextPrompt.TextPromptData sData)
    {
        int width = 100;

        EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel, GUILayout.Width(width));

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




        int gamepadGroupCount = sData.GetGroupCountAllSpritesSingleGamepad();
        if (gamepadGroupCount > 1)
        {
            EditorGUILayout.BeginHorizontal();
            sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad = EditorGUILayout.IntField(sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad, GUILayout.Width(20));

            if (GUILayout.Button("-", GUILayout.Width(20)))
                sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad--;

            if (GUILayout.Button("+", GUILayout.Width(20)))
                sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad++;

            sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad = Mathf.Clamp(sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad, 0, gamepadGroupCount - 1);

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("0", GUILayout.Width(width));
            sData.bindingIDInAvailableListAllMatchingBindingsSingleGamepad = 0;
        }

        EditorGUILayout.Space(5);
    }
}

}


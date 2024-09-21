using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.InputSystem;


namespace InputIcons
{
    [CustomEditor(typeof(II_UIRebindInputActionBehaviour))]

    public class II_UIRebindInputActionBehaviourEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            II_UIRebindInputActionBehaviour rebindBehaviour = (II_UIRebindInputActionBehaviour)target;

            EditorGUILayout.HelpBox("This component is deprecated as of Input Icons version 3.0.0. Consider using the more feature rich II_UIRebindInputActionImageBehaviour instead.", MessageType.Warning);


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Binding", EditorStyles.boldLabel);
            rebindBehaviour.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", rebindBehaviour.actionReference, typeof(InputActionReference), false);
            if (InputIconsUtility.ActionIsComposite(rebindBehaviour.actionReference.action))
            {
                rebindBehaviour.bindingType = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup("Binding Type", rebindBehaviour.bindingType);
            }


            rebindBehaviour.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device", rebindBehaviour.deviceType);
            rebindBehaviour.m_bindingIndex = EditorGUILayout.IntField("Binding ID In List", rebindBehaviour.m_bindingIndex);

            if (InputIconsManagerSO.GetKeyboardControlSchemeCountOfAction(rebindBehaviour.actionReference) > 1)
            {
                if (rebindBehaviour.deviceType == InputIconsUtility.DeviceType.Auto || rebindBehaviour.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                    rebindBehaviour.keyboardControlSchemeIndex = EditorGUILayout.IntSlider("Keyboard Control Scheme Index", rebindBehaviour.keyboardControlSchemeIndex, 0, InputIconsManagerSO.Instance.controlSchemeNames_Keyboard.Count - 1);
            }
            else
                rebindBehaviour.keyboardControlSchemeIndex = 0;

            if (InputIconsManagerSO.GetGamepadControlSchemeCountOfAction(rebindBehaviour.actionReference) > 1)
            {
                if (rebindBehaviour.deviceType == InputIconsUtility.DeviceType.Auto || rebindBehaviour.deviceType == InputIconsUtility.DeviceType.Gamepad)
                    rebindBehaviour.gamepadControlSchemeIndex = EditorGUILayout.IntSlider("Gamepad Control Scheme Index", rebindBehaviour.gamepadControlSchemeIndex, 0, InputIconsManagerSO.Instance.controlSchemeNames_Gamepad.Count - 1);
                else
                    rebindBehaviour.gamepadControlSchemeIndex = 0;
            }

            //if (InputIconsManagerSO.Instance.isUsingFonts)
            rebindBehaviour.displayType = (II_UIRebindInputActionBehaviour.DisplayType)EditorGUILayout.EnumPopup("Display Type", rebindBehaviour.displayType);


            EditorGUILayout.Space(5);


            EditorGUILayout.LabelField("Rebinding", EditorStyles.boldLabel);
            rebindBehaviour.canBeRebound = EditorGUILayout.Toggle(new GUIContent("Allow Rebinding", "If disabled, this action can not be rebound, but will still be displayed."), rebindBehaviour.canBeRebound);

            if (InputIconsManagerSO.Instance.rebindBehaviour == InputIconsManagerSO.RebindBehaviour.OverrideExisting)
            {
                rebindBehaviour.ignoreOtherButtons = EditorGUILayout.Toggle(new GUIContent("Ignore Other Buttons", "If enabled, will not get unbound if another action gets assigned the same key."), rebindBehaviour.ignoreOtherButtons);
            }

            if (rebindBehaviour.canBeRebound)
            {
                rebindBehaviour.keyboardCancelKey = EditorGUILayout.TextField("Keyboard Cancel Key", rebindBehaviour.keyboardCancelKey);
                rebindBehaviour.gamepadCancelKey = EditorGUILayout.TextField("Gamepad Cancel Key", rebindBehaviour.gamepadCancelKey);
            }

            EditorGUILayout.Space(5);


            EditorGUILayout.LabelField("Display Texts", EditorStyles.boldLabel);
            if (rebindBehaviour.actionNameDisplayText)
            {
                rebindBehaviour.actionNameDisplayText.text = EditorGUILayout.TextField("Action Display Name", rebindBehaviour.actionNameDisplayText.text);
            }

            rebindBehaviour.actionNameDisplayText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Action Name Display Text", rebindBehaviour.actionNameDisplayText, typeof(TextMeshProUGUI), true);

            rebindBehaviour.bindingNameDisplayText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Binding Name Display Text", rebindBehaviour.bindingNameDisplayText, typeof(TextMeshProUGUI), true);


            EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
            rebindBehaviour.rebindButtonObject = (GameObject)EditorGUILayout.ObjectField("Rebind Button Object", rebindBehaviour.rebindButtonObject, typeof(GameObject), true);
            rebindBehaviour.resetButtonObject = (GameObject)EditorGUILayout.ObjectField("Reset Button Object", rebindBehaviour.resetButtonObject, typeof(GameObject), true);

            EditorGUILayout.LabelField("Display Object While Rebinding", EditorStyles.boldLabel);
            rebindBehaviour.listeningForInputObject = (GameObject)EditorGUILayout.ObjectField("Listening For Input Object", rebindBehaviour.listeningForInputObject, typeof(GameObject), true);

            EditorGUILayout.LabelField("Display Object If Binding Already Used", EditorStyles.boldLabel);
            rebindBehaviour.keyAlreadyUsedObject = (GameObject)EditorGUILayout.ObjectField("Binding Already Used Object", rebindBehaviour.keyAlreadyUsedObject, typeof(GameObject), true);

            InputIconsManagerSO manager = InputIconsManagerSO.Instance;
            manager.rebindBehaviour = (InputIconsManagerSO.RebindBehaviour)EditorGUILayout.EnumPopup(new GUIContent("Rebind Behavior (same for all Rebind Buttons)",
                    "Choose how to handle rebinding when the same binding already exists in the same action map."), manager.rebindBehaviour);


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(rebindBehaviour.bindingNameDisplayText);
                EditorUtility.SetDirty(rebindBehaviour.actionNameDisplayText);
                EditorUtility.SetDirty(rebindBehaviour);

                serializedObject.ApplyModifiedProperties();

                if (Application.isEditor)
                {
                    rebindBehaviour.UpdateBindingDisplay();
                    rebindBehaviour.OnValidate();
                    EditorUtility.SetDirty(target);

                }
            }


        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InputIcons
{
    public class InputIconsIconChangeWindow : EditorWindow
    {

        GUIStyle buttonStyle;
        GUIStyle textStyle;


        [MenuItem("Tools/Input Icons/Input Icons Switcher", priority = 3)]
        public static void ShowWindow()
        {
            const int width = 270;
            const int height = 220;

            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;

            EditorWindow window = GetWindow<InputIconsIconChangeWindow>("Input Icons Switcher");
            window.position = new Rect(x, y, width, height);
        }

        private void OnGUI()
        {
            textStyle = new GUIStyle(EditorStyles.label);
            textStyle.wordWrap = true;


            buttonStyle = EditorStyles.miniButtonMid;
            GUILayout.Label("Use the following buttons to display icons of the desired device and test how they look.", textStyle);

            if (GUILayout.Button("Keyboard Icons"))
            {
                InputIconsManagerSO.SetDeviceAndRefreshDisplayedIcons(InputIconSetConfiguratorSO.InputIconsDevice.Keyboard);
            }

            if (GUILayout.Button("PS3 Icons"))
            {
                InputIconsManagerSO.SetDeviceAndRefreshDisplayedIcons(InputIconSetConfiguratorSO.InputIconsDevice.PS3);
            }

            if (GUILayout.Button("PS4 Icons"))
            {
                InputIconsManagerSO.SetDeviceAndRefreshDisplayedIcons(InputIconSetConfiguratorSO.InputIconsDevice.PS4);
            }

            if (GUILayout.Button("PS5 Icons"))
            {
                InputIconsManagerSO.SetDeviceAndRefreshDisplayedIcons(InputIconSetConfiguratorSO.InputIconsDevice.PS5);
            }

            if (GUILayout.Button("Nintendo Icons"))
            {
                InputIconsManagerSO.SetDeviceAndRefreshDisplayedIcons(InputIconSetConfiguratorSO.InputIconsDevice.Switch);
            }

            if (GUILayout.Button("XBox Icons"))
            {
                InputIconsManagerSO.SetDeviceAndRefreshDisplayedIcons(InputIconSetConfiguratorSO.InputIconsDevice.XBox);
            }

            if (GUILayout.Button("Fallback Icons"))
            {
                InputIconsManagerSO.SetDeviceAndRefreshDisplayedIcons(InputIconSetConfiguratorSO.InputIconsDevice.Fallback);
            }
        }

    }
}

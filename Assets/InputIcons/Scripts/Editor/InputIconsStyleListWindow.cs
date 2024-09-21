using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.Compilation;
using System.IO;
using UnityEditorInternal;

namespace InputIcons
{
    public class InputIconsStyleListWindow : EditorWindow
    {
        private Vector2 scrollPos;

        private InputIconsManagerSO managerSO;
        public static SerializedObject serializedManager;

        private ReorderableList styleTagKeyboardDatas;
        private ReorderableList styleTagGamepadDatas;

        [MenuItem("Tools/Input Icons/Input Icons TMPro Style List Window", priority = 2)]
        public static void ShowWindow()
        {
            const int width = 700;
            const int height = 700;

            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;

            EditorWindow window = GetWindow<InputIconsStyleListWindow>("Input Icons TMPro Style List");
            window.position = new Rect(x, y, width, height);
        }

        void OnFocus()
        {
            RedrawLists();
        }


        protected void OnEnable()
        {
            // load values
            var data = EditorPrefs.GetString("InputIconsSetupWindow", JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);

            position.Set(position.x, position.y, 1000, 800);

            managerSO = InputIconsManagerSO.Instance;
            InputIconsManagerSO.Instance.UpdateInputStyleData();
            InputIconsManagerSO.onStyleSheetsUpdated += RedrawLists;

            RedrawLists();
        }

        protected void OnDisable()
        {
            // save values
            var data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString("InputIconsSetupWindow", data);
            InputIconsManagerSO.onStyleSheetsUpdated -= RedrawLists;
        }

        public void RedrawLists()
        {
            serializedManager = new SerializedObject(InputIconsManagerSO.Instance);
            //InputIconsManagerSO.UpdateStyleData();
            DrawCustomContextList();
        }

       

      
        private void OnGUI()
        {

            scrollPos =
               EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));

            EditorGUILayout.Space(10);
            GUILayout.Label("Below are styles which you can copy into a TMPro text component.");

            EditorGUILayout.HelpBox("When you make changes to your Input Action Assets (changing action names " +
                "or adding new actions), you need to update the TMP style sheet through the Input Icons setup window.", MessageType.Info);

            EditorGUILayout.Space(5);
            GUILayout.Label("Available keyboard styles");
            styleTagKeyboardDatas.DoLayoutList();
            styleTagKeyboardDatas.displayAdd = false;
            styleTagKeyboardDatas.displayRemove = false;
            styleTagKeyboardDatas.draggable = false;


            EditorGUILayout.Space(5);
            GUILayout.Label("Available gamepad styles");
            styleTagGamepadDatas.DoLayoutList();
            styleTagGamepadDatas.displayAdd = false;
            styleTagGamepadDatas.displayRemove = false;
            styleTagGamepadDatas.draggable = false;

            EditorGUILayout.EndScrollView();
        }

        void DrawCustomContextList()
        {
            float width = 150;
            float currentX = 5;

            try
            {
                styleTagKeyboardDatas = new ReorderableList(serializedManager, serializedManager.FindProperty("inputStyleKeyboardDataList"), true, true, true, true);

             

                styleTagKeyboardDatas.drawHeaderCallback = (Rect rect) =>
                {
                    currentX = 5;
                    width = 100;
                    EditorGUI.LabelField(new Rect(rect.x + currentX, rect.y, width-5, EditorGUIUtility.singleLineHeight), "TMPro Style Tag");
                    currentX += width;
                    EditorGUI.LabelField(new Rect(rect.x + currentX, rect.y, width-5, EditorGUIUtility.singleLineHeight), "TMPro Font Style Tag");
                    currentX += width;
                    width = 300;
                    EditorGUI.LabelField(new Rect(rect.x + currentX, rect.y, width-5, EditorGUIUtility.singleLineHeight), "Binding");

                };

                styleTagKeyboardDatas.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {

                    var element = styleTagKeyboardDatas.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += 2;
                    width = 100;
                    currentX = 5;
                    EditorGUI.PropertyField(new Rect(rect.x + currentX, rect.y, width-5, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("tmproReferenceText"), GUIContent.none);

                    currentX += width;
                    EditorGUI.PropertyField(new Rect(rect.x + currentX, rect.y, width-5, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("fontReferenceText"), GUIContent.none);

                    currentX += width;
                    width = 300;
                    EditorGUI.PropertyField(new Rect(rect.x + currentX, rect.y, width-5, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("bindingName"), GUIContent.none);
                };




                styleTagGamepadDatas = new ReorderableList(serializedManager, serializedManager.FindProperty("inputStyleGamepadDataList"), true, true, true, true);

                styleTagGamepadDatas.drawHeaderCallback = (Rect rect) =>
                {
                    currentX = 5;
                    width = 100;
                    EditorGUI.LabelField(new Rect(rect.x + currentX, rect.y, width - 5, EditorGUIUtility.singleLineHeight), "TMPro Style Tag");
                    currentX += width;
                    EditorGUI.LabelField(new Rect(rect.x + currentX, rect.y, width - 5, EditorGUIUtility.singleLineHeight), "TMPro Font Style Tag");
                    currentX += width;
                    width = 300;
                    EditorGUI.LabelField(new Rect(rect.x + currentX, rect.y, width - 5, EditorGUIUtility.singleLineHeight), "Binding");
                };

                styleTagGamepadDatas.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {

                    var element = styleTagGamepadDatas.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += 2;
                    width = 100;
                    currentX = 5;
                    EditorGUI.PropertyField(new Rect(rect.x + currentX, rect.y, width - 5, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("tmproReferenceText"), GUIContent.none);

                    currentX += width;
                    EditorGUI.PropertyField(new Rect(rect.x + currentX, rect.y, width - 5, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("fontReferenceText"), GUIContent.none);

                    currentX += width;
                    width = 300;
                    EditorGUI.PropertyField(new Rect(rect.x + currentX, rect.y, width - 5, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("bindingName"), GUIContent.none);
                };
            }
            catch (System.Exception)
            {

            }
        }
    }
}

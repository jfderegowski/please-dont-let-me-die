using System;
using System.Linq;
using System.Text;
using SaveSystem.Runtime.Serializable;
using UnityEditor;
using UnityEngine;

namespace NoReleaseDate.SaveSystem.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidDrawer : PropertyDrawer
    {
        private static readonly string[] _guidParts = { "Part1", "Part2", "Part3", "Part4" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            GUI.enabled = false;
            if (GetGuidParts(property).All(x => x != null))
                // EditorGUI.TextField(position, BuildGuidString(GetGuidParts(property)));
                EditorGUI.LabelField(label.text == "" ? position : EditorGUI.PrefixLabel(position, label),
                    BuildGuidString(GetGuidParts(property)));
            else 
                EditorGUI.LabelField(position, "GUID Not Initialized");
            GUI.enabled = true;

            var hasClicked = Event.current.type == EventType.MouseUp && Event.current.button == 1;

            if (hasClicked && position.Contains(Event.current.mousePosition))
            {
                ShowContextMenu(property);
                Event.current.Use();
            }

            EditorGUI.EndProperty();
        }

        private static void ShowContextMenu(SerializedProperty property)
        {
            var menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Copy"), false, () => Copy(property));
            
            if (CanPaste()) menu.AddItem(new GUIContent("Paste"), false, () => Paste(property));
            else menu.AddDisabledItem(new GUIContent("Paste"));
            
            menu.AddSeparator("");
            
            menu.AddItem(new GUIContent("Reset"), false, () => Reset(property));
            
            menu.AddItem(new GUIContent("Regenerate"), false, () => Regenerate(property));
            
            menu.ShowAsContext();
        }

        private static void Copy(SerializedProperty property)
        {
            if (GetGuidParts(property).Any(x => x == null)) return;

            var guid = BuildGuidString(GetGuidParts(property));
            EditorGUIUtility.systemCopyBuffer = guid;
            
            Debug.Log($"GUID copied to clipboard: {guid}");
        }

        private static void Paste(SerializedProperty property)
        {
            const string warning = "Are you sure you want to paste the GUID from the clipboard?";
            
            if (!EditorUtility.DisplayDialog("Paste GUID", warning, "Yes", "No")) return;
            
            var hexString = EditorGUIUtility.systemCopyBuffer;
            
            if (!SerializableGuid.TryConvertFromHexString(hexString, out var guid)) return;

            property.FindPropertyRelative(_guidParts[0]).uintValue = guid.Part1;
            property.FindPropertyRelative(_guidParts[1]).uintValue = guid.Part2;
            property.FindPropertyRelative(_guidParts[2]).uintValue = guid.Part3;
            property.FindPropertyRelative(_guidParts[3]).uintValue = guid.Part4;
            
            property.serializedObject.ApplyModifiedProperties();
            
            Debug.Log($"GUID pasted from clipboard: {hexString}");
        }

        private static void Reset(SerializedProperty property)
        {
            const string warning = "Are you sure you want to reset the GUID?";
            
            if (!EditorUtility.DisplayDialog("Reset GUID", warning, "Yes", "No")) return;

            foreach (var part in GetGuidParts(property)) 
                part.uintValue = 0;

            property.serializedObject.ApplyModifiedProperties();
            
            Debug.Log("GUID has been reset.");
        }

        private static void Regenerate(SerializedProperty property)
        {
            const string warning = "Are you sure you want to regenerate the GUID?";
            
            if (!EditorUtility.DisplayDialog("Reset GUID", warning, "Yes", "No")) return;

            var bytes = Guid.NewGuid().ToByteArray();
            var guidParts = GetGuidParts(property);

            for (var i = 0; i < _guidParts.Length; i++) 
                guidParts[i].uintValue = BitConverter.ToUInt32(bytes, i * 4);

            property.serializedObject.ApplyModifiedProperties();

            Debug.Log($"GUID has been regenerated to: {BuildGuidString(guidParts)}");
        }

        private static bool CanPaste()
        {
            var guidString = EditorGUIUtility.systemCopyBuffer;
            
            return SerializableGuid.IsHexString(guidString);
        }

        private static SerializedProperty[] GetGuidParts(SerializedProperty property)
        {
            var values = new SerializedProperty[_guidParts.Length];
            
            for (var i = 0; i < _guidParts.Length; i++) 
                values[i] = property.FindPropertyRelative(_guidParts[i]);

            return values;
        }

        private static string BuildGuidString(SerializedProperty[] guidParts) =>
            new StringBuilder()
                .AppendFormat("{0:X8}", guidParts[0].uintValue)
                .AppendFormat("{0:X8}", guidParts[1].uintValue)
                .AppendFormat("{0:X8}", guidParts[2].uintValue)
                .AppendFormat("{0:X8}", guidParts[3].uintValue)
                .ToString();
    }
}

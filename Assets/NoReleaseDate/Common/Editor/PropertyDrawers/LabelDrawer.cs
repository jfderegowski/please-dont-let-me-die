using NoReleaseDate.Common.Runtime.PropertyAttributes;
using UnityEditor;
using UnityEngine;

namespace NoReleaseDate.Common.Editor.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelAttribute = (LabelAttribute)attribute;

            label.text = labelAttribute.Label;

            EditorGUI.PropertyField(position, property, label);
        }
    }
}
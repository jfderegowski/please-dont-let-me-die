using NoReleaseDate.Common.Runtime.Properties;
using UnityEditor;
using UnityEngine;

namespace NoReleaseDate.Common.Editor.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(HasValue<>))]
    public class HasValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get properties
            var hasValueProp = property.FindPropertyRelative("hasValue");
            var valueProp = property.FindPropertyRelative("value");
        
            // Calculate rects
            var defName = property.displayName;
        
            var hasValueRec = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
        
            var hasValueLabel = new GUIContent($"Has {label.text}");
            var valueLabel = new GUIContent(defName);
        
            // Draw the boolean toggle
            EditorGUI.PropertyField(hasValueRec, hasValueProp, hasValueLabel);

            // If the boolean is true, draw the value field
            if (!hasValueProp.boolValue) return;

            if (valueProp == null) return;
        
            if (valueProp.propertyType == SerializedPropertyType.Generic)
                EditorGUI.PropertyField(valueRect, valueProp, valueLabel, true);
            else EditorGUI.PropertyField(valueRect, valueProp, valueLabel);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hasValueProp = property.FindPropertyRelative("hasValue");
            var valueProp = property.FindPropertyRelative("value");
            var height = EditorGUIUtility.singleLineHeight;

            if (!hasValueProp.boolValue) return height;
        
            if (valueProp == null) return height;
        
            if (valueProp.propertyType == SerializedPropertyType.Generic)
                height += EditorGUI.GetPropertyHeight(valueProp, true) + EditorGUIUtility.standardVerticalSpacing;
            else height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return height;
        }
    }
}

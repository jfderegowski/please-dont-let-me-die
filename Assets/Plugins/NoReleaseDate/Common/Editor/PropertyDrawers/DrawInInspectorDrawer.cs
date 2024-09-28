using NoReleaseDate.Common.Runtime.PropertyAttributes;
using UnityEditor;
using UnityEngine;

namespace NoReleaseDate.Common.Editor.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(DrawInInspector), true)]
    public class DrawInInspectorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var h = 0f;

            if (property.propertyType != SerializedPropertyType.ObjectReference || !property.objectReferenceValue)
                return h;
            
            var so = new SerializedObject(property.objectReferenceValue);
            var iterator = so.GetIterator();
            
            for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                h += EditorGUI.GetPropertyHeight(iterator, true);

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, "Mismatched PropertyDrawer.");
                return;
            }

            var obj = property.objectReferenceValue;
            var objRect = GetRect(ref position, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.ObjectField(objRect, property, label);
            
            if (!obj) return;
            
            var so = new SerializedObject(property.objectReferenceValue);
            var iterator = so.GetIterator();
            for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                // Continue if the property is the script reference
                if (iterator.propertyPath == "m_Script") continue;
                
                var h = EditorGUI.GetPropertyHeight(iterator, true);
                var r = GetRect(ref position, h);
                
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(r, iterator, true);
                if (EditorGUI.EndChangeCheck())
                    so.ApplyModifiedProperties();
            }
        }

        private static Rect GetRect(ref Rect position, float height)
        {
            var result = new Rect(position.xMin, position.yMin, position.width, height);
            
            position = new Rect(position.xMin, position.yMin + height, position.width,
                Mathf.Max(0f, position.height - height));
            
            return result;
        }
    }
}
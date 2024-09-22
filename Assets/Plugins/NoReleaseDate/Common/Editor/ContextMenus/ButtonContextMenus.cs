using EventButtonSystem.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NoReleaseDate.Common.Editor.Editor.ContextMenus
{
    public static class ButtonContextMenus
    {
        [MenuItem("CONTEXT/Button/Change to EventButton")]
        public static void ChangeToEventButton(MenuCommand menuCommand)
        {
            var button = (Button)menuCommand.context; 
            
            var gameObject = button.gameObject;

            Object.DestroyImmediate(button);
            
            gameObject.AddComponent<ButtonWithEvents>();
        }
    }
}
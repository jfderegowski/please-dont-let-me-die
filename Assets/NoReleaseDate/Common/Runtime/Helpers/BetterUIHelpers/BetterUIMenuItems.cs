using TheraBytes.BetterUi;
using UnityEditor;
using UnityEngine;

namespace NoReleaseDate.Common.Runtime.Helpers.BetterUIHelpers
{
    public static class BetterUIMenuItems
    {
        [MenuItem("CONTEXT/BetterAxisAlignedLayoutGroup/Remove Size Modifiers")]
        private static void BetterAxisAlignedLayoutGroupRemoveSizeModifiers(MenuCommand command)
        {
            var betterAxisAlignedLayoutGroup = (BetterAxisAlignedLayoutGroup)command.context;
            
            RemoveSizeModifiers(betterAxisAlignedLayoutGroup, betterAxisAlignedLayoutGroup.SpacingSizer);
            RemoveSizeModifiers(betterAxisAlignedLayoutGroup, betterAxisAlignedLayoutGroup.PaddingSizer);
        }
        
        [MenuItem("CONTEXT/BetterContentSizeFitter/Remove Size Modifiers")]
        private static void BetterContentSizeFitterRemoveSizeModifiers(MenuCommand command)
        {
            var betterContentSizeFitter = (BetterContentSizeFitter)command.context;
            
            RemoveSizeModifiers(betterContentSizeFitter, betterContentSizeFitter.CurrentPadding);
        }
        
        [MenuItem("CONTEXT/BetterTextMeshProUGUI/Remove Size Modifiers")]
        private static void BetterTextMeshProUGUIRemoveSizeModifiers(MenuCommand command)
        {
            var betterTextMeshProUGUIComponent = (Component)command.context;
            
            if (!betterTextMeshProUGUIComponent) return;
            
            var betterTextMeshProUGUI = betterTextMeshProUGUIComponent.GetComponent<BetterTextMeshProUGUI>();
            
            RemoveSizeModifiers(betterTextMeshProUGUIComponent, betterTextMeshProUGUI.FontSizer);
            RemoveSizeModifiers(betterTextMeshProUGUIComponent, betterTextMeshProUGUI.MarginSizer);
            RemoveSizeModifiers(betterTextMeshProUGUIComponent, betterTextMeshProUGUI.MaxFontSizer);
            RemoveSizeModifiers(betterTextMeshProUGUIComponent, betterTextMeshProUGUI.MinFontSizer);
        }
        
        private static void RemoveSizeModifiers<T>(Component container, ScreenDependentSize<T> sizer)
        {
            Undo.RecordObject(container, "Remove Size Modifiers");
            
            foreach (var mod in sizer.GetModifiers()) 
                mod.SizeModifiers.Clear();
        }
    }
}

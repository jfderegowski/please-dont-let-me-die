using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NoReleaseDate.Common.ContextMenus
{
    public static class CanvasScalerContextMenus
    {
        [MenuItem("CONTEXT/CanvasScaler/Set FullHD Values")]
        public static void SetFullHd(MenuCommand command)
        {
            var canvasScaler = (CanvasScaler)command.context;
            
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0;
            canvasScaler.referencePixelsPerUnit = 100;
        }
    }
}

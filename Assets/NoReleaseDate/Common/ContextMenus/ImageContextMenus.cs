using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NoReleaseDate.Common.ContextMenus
{
    public static class ImageContextMenus
    {
        [MenuItem("CONTEXT/Image/Set 1x1 White Pixel")]
        private static void SetWhitePixel(MenuCommand command)
        {
            var image = (Image)command.context;
            
            const string spritePath = "Assets/NoReleaseDate/UI/Sprites/Performance/1x1_WhitePixel.png";
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            
            image.sprite = sprite;
        }
        
        [MenuItem("CONTEXT/Image/Set 1x1 Black Pixel")]
        private static void SetBlackPixel(MenuCommand command)
        {
            var image = (Image)command.context;
            
            const string spritePath = "Assets/NoReleaseDate/UI/Sprites/Performance/1x1_BlackPixel.png";
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            image.sprite = sprite;
        }
    }
}
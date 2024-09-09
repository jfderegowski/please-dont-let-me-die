using NoReleaseDate.Common.Runtime.Extensions;
using UnityEngine;

namespace NoReleaseDate.ThemeSystem.Runtime
{
    /// <summary>
    /// Color Palette for the UI Theme System
    /// </summary>
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "No Release Date/Theme System/Color Palette")]
    public class ThemeColorPalette : ScriptableObject
    {
        public Color firstColor = new Color().FromHex("#fffef5");
        public Color secondColor = new Color().FromHex("#313c50");
        public Color thirdColor = new Color().FromHex("#475866");
        public Color fourthColor = new Color().FromHex("#ad5143");

        public Color GetColor(ColorType colorType) =>
            colorType switch
            {
                ColorType.First => firstColor,
                ColorType.Second => secondColor,
                ColorType.Third => thirdColor,
                ColorType.Fourth => fourthColor,
                _ => Color.white
            };
    }
}
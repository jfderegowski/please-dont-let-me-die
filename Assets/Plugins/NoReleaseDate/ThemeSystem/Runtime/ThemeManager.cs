using NoReleaseDate.SingletonSystem.Runtime;
using UnityEngine;

namespace NoReleaseDate.ThemeSystem.Runtime
{
    [CreateAssetMenu(fileName = "ThemeManager", menuName = "No Release Date/Theme System/Theme Manager")]
    public class ThemeManager : SingletonScriptableObject<ThemeManager>
    {
        public ThemeColorPalette current => defaultColorPalette;
        
        public ThemeColorPalette defaultColorPalette;
    }
}

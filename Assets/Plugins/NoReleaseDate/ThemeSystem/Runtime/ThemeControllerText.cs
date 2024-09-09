using TMPro;
using UnityEngine;

namespace NoReleaseDate.ThemeSystem.Runtime
{
    [AddComponentMenu("No Release Date/Theme System/Theme Controller Text")]
    [RequireComponent(typeof(TMP_Text))]
    public class ThemeControllerText : ThemeController
    {
        public ColorType colorType;

        private TMP_Text _text;
        
        protected override void Awake()
        {
            _text = GetComponent<TMP_Text>();
            
            base.Awake();
        }

        protected override void UpdateTheme()
        {
            if (!themeManager || !_text) return;
            
            if (_text.color == themeManager.current.GetColor(colorType)) return;
            
            _text.color = themeManager.current.GetColor(colorType);
        }
    }
}
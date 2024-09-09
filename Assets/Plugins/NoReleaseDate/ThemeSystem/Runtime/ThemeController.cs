using UnityEngine;

namespace NoReleaseDate.ThemeSystem.Runtime
{
    [ExecuteInEditMode]
    public abstract class ThemeController : MonoBehaviour
    {
        public static ThemeManager themeManager => ThemeManager.instance;

        protected virtual void Awake()
        {
            UpdateTheme();

            if (!Application.isPlaying) return;
            enabled = false;
        }

#if UNITY_EDITOR
        
        private void Update()
        {
            if (Application.isPlaying)
            {
                enabled = false;
                return;
            }
            
            UpdateTheme();
        }

#endif

        protected abstract void UpdateTheme();
    }
}
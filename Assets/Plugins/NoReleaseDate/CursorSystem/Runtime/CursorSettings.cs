using System;
using UnityEngine;

namespace NoReleaseDate.CursorSystem.Runtime
{
    /// <summary>
    /// Cursor settings class. It contains the cursor settings.
    /// </summary>
    [Serializable]
    public class CursorSettings
    {
        /// <summary>
        /// When this CursorSettings is applied, the cursor will be visible if this is true.
        /// </summary>
        public bool IsVisible = true;
        /// <summary>
        /// When this CursorSettings is applied, the cursor LockMode will be set to this.
        /// </summary>
        public CursorLockMode LockMode = CursorLockMode.None;
        /// <summary>
        /// When this CursorSettings is applied, the cursor texture will be set to this.
        /// </summary>
        public Texture2D Texture;
        /// <summary>
        /// When this CursorSettings is applied, the cursor hotspot will be set to this.
        /// </summary>
        public Vector2 Hotspot = Vector2.zero;
        /// <summary>
        /// When this CursorSettings is applied, the cursor mode will be set to this.
        /// </summary>
        public CursorMode CursorMode = CursorMode.Auto;

        /// <summary>
        /// CursorSettings builder class.
        /// Helps to create a CursorSettings object.
        /// </summary>
        public class Builder
        {
            private readonly CursorSettings _cursorSettings = new();
            
            public Builder SetVisible(bool isVisible)
            {
                _cursorSettings.IsVisible = isVisible;
                return this;
            }
            
            public Builder SetLockMode(CursorLockMode lockMode)
            {
                _cursorSettings.LockMode = lockMode;
                return this;
            }
            
            public Builder SetTexture(Texture2D texture)
            {
                _cursorSettings.Texture = texture;
                return this;
            }
            
            public Builder SetHotspot(Vector2 hotspot)
            {
                _cursorSettings.Hotspot = hotspot;
                return this;
            }
            
            public Builder SetCursorMode(CursorMode cursorMode)
            {
                _cursorSettings.CursorMode = cursorMode;
                return this;
            }
            
            public CursorSettings Build() => _cursorSettings;
        }
        
        /// <summary>
        /// The cursor will be visible and unlocked.
        /// The "unlocked" means that the cursor will be free to move outside the game window.
        /// </summary>
        public static CursorSettings showAndUnlock =>
            new Builder().SetVisible(true).SetLockMode(CursorLockMode.None).Build();
        
        /// <summary>
        /// The cursor will be visible and locked.
        /// The "locked" means that the cursor will be locked in the center of the game window.
        /// </summary>
        public static CursorSettings showAndLock =>
            new Builder().SetVisible(true).SetLockMode(CursorLockMode.Locked).Build();
        
        /// <summary>
        /// The cursor will be visible and confined.
        /// The "confined" means that the cursor will be free to move inside the game window.
        /// </summary>
        public static CursorSettings showAndConfined =>
            new Builder().SetVisible(true).SetLockMode(CursorLockMode.Confined).Build();
        
        /// <summary>
        /// The cursor will be hidden and locked.
        /// The "locked" means that the cursor will be locked in the center of the game window.
        /// </summary>
        public static CursorSettings hideAndLock =>
            new Builder().SetVisible(false).SetLockMode(CursorLockMode.Locked).Build();
        
        /// <summary>
        /// The cursor will be hidden and unlocked.
        /// The "unlocked" means that the cursor will be free to move outside the game window.
        /// </summary>
        public static CursorSettings hideAndUnlock =>
            new Builder().SetVisible(false).SetLockMode(CursorLockMode.None).Build();
        
        /// <summary>
        /// The cursor will be hidden and confined.
        /// The "confined" means that the cursor will be free to move inside the game window.
        /// </summary>
        public static CursorSettings hideAndConfined =>
            new Builder().SetVisible(false).SetLockMode(CursorLockMode.Confined).Build();
    }
}
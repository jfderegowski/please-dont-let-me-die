using System;
using System.Collections.Generic;
using NoReleaseDate.SingletonSystem.Runtime;
using UnityEngine;

namespace NoReleaseDate.CursorSystem.Runtime
{
    /// <summary>
    /// Cursor manager class. It manages the cursor settings and the cursor handlers.
    /// </summary>
    public class CursorManager : Singleton<CursorManager>
    {
        /// <summary>
        /// Event that is called when a cursor handler is registered.
        /// ICursorHandler is the registered cursor handler.
        /// </summary>
        public event Action<ICursorHandler> onRegister;
        /// <summary>
        /// Event that is called when a cursor handler is unregistered.
        /// ICursorHandler is the unregistered cursor handler.
        /// </summary>
        public event Action<ICursorHandler> onUnregister;

        /// <summary>
        /// The current cursor handler.
        /// </summary>
        public ICursorHandler currentCursorHandler => cursorHandlers.Count > 0 ? cursorHandlers[0] : null;
        /// <summary>
        /// The list of cursor handlers.
        /// </summary>
        public List<ICursorHandler> cursorHandlers { get; } = new();
        /// <summary>
        /// The current cursor settings.
        /// </summary>
        public CursorSettings currentCursorSettings => currentCursorHandler?.cursorSettings;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            UnregisterAll();
        }

        /// <summary>
        /// Register a cursor handler.
        /// </summary>
        /// <param name="cursorHandler">The cursor handler to register.</param>
        public void Register(ICursorHandler cursorHandler)
        {
            cursorHandlers.Insert(0, cursorHandler);
            
            ActiveCursorHandler(cursorHandler);
            
            onRegister?.Invoke(cursorHandler);
        }
        
        /// <summary>
        /// Unregister a cursor handler.
        /// </summary>
        /// <param name="cursorHandler">The cursor handler to unregister.</param>
        public void Unregister(ICursorHandler cursorHandler)
        {
            cursorHandlers.Remove(cursorHandler);
            
            ActiveCursorHandler(currentCursorHandler);
            
            onUnregister?.Invoke(cursorHandler);
        }
        
        private void UnregisterAll()
        {
            for (var i = 0; i < cursorHandlers.Count; i++) 
                Unregister(cursorHandlers[i]);
        }
        
        private void ActiveCursorHandler(ICursorHandler cursorHandler)
        {
            if (cursorHandler == null) return;
            
            if (currentCursorHandler != cursorHandler)
            {
                cursorHandlers.Remove(cursorHandler);
                cursorHandlers.Insert(0, cursorHandler);
            }
            
            SetCursorSettings(cursorHandler.cursorSettings);
        }
        
        private static void SetCursorSettings(CursorSettings cursorSettings)
        {
            Cursor.visible = cursorSettings.IsVisible;
            Cursor.lockState = cursorSettings.LockMode;
            Cursor.SetCursor(cursorSettings.Texture, cursorSettings.Hotspot, cursorSettings.CursorMode);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using NoReleaseDate.SingletonSystem.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace FirstSelectedSystem.Runtime
{
    public class FirstSelectedManager : Singleton<FirstSelectedManager>
    {
        public RectTransform currentFirstSelected => firstSelectedList.Count > 0 ? firstSelectedList[0] : null;
        
        public List<RectTransform> firstSelectedList { get; } = new();

        [SerializeField] private InputActionReference _navigateAction;

        protected override void Awake()
        {
            base.Awake();
            
            _navigateAction.action.performed += OnNavigatePerformed;
            _navigateAction.action.Enable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _navigateAction.action.Disable();
            
            UnregisterAll();
        }
        
        private async void OnNavigatePerformed(CallbackContext context)
        {
            if (EventSystem.current.currentSelectedGameObject) return;
            
            await Task.Yield();

            SelectCurrent();
        }
        
        public void Register(RectTransform toBeSelected)
        {
            if (!toBeSelected) return;
            
            firstSelectedList.Insert(0, toBeSelected);

            SelectCurrent();
        }

        public void Unregister(RectTransform toBeSelected)
        {
            if (!toBeSelected) return;
            if (!firstSelectedList.Contains(toBeSelected)) return;

            firstSelectedList.Remove(toBeSelected);
            
            SelectCurrent();
        }
        
        private void SelectCurrent()
        {
            if (!currentFirstSelected) return;
            
            EventSystem.current.SetSelectedGameObject(currentFirstSelected.gameObject);
        }

        private void UnregisterAll()
        {
            if (firstSelectedList == null) return;
            
            for (var i = 0; i < firstSelectedList.Count; i++)
                Unregister(firstSelectedList[i]);
        }
    }
}

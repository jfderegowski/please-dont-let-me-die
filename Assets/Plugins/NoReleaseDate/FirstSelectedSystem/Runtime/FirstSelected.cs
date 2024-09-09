using UnityEngine;

namespace FirstSelectedSystem.Runtime
{
    public class FirstSelected : MonoBehaviour
    {
        [field: SerializeField] public virtual RectTransform toBeSelected { get; private set; }
        
        private void OnEnable() => 
            FirstSelectedManager.instance.Register(toBeSelected);
        
        private void OnDisable() =>
            FirstSelectedManager.instance.Unregister(toBeSelected);
    }
}
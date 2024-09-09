using UnityEngine;

namespace UIRebuilderSystem.Runtime
{
    /// <summary>
    /// Rebuilds the UI element that this component is attached to.
    /// </summary>
    public class RebuildUI : MonoBehaviour
    {
        private enum UnityLifecycle
        {
            Awake,
            OnEnable,
            Start
        }

        [Tooltip("Select when to rebuild the UI.")]
        [SerializeField] private UnityLifecycle _rebuildOn = UnityLifecycle.Start;
    
        private void Awake()
        {
            if (_rebuildOn == UnityLifecycle.Awake)
                Rebuild();
        }
    
        private void OnEnable()
        {
            if (_rebuildOn == UnityLifecycle.OnEnable)
                Rebuild();
        }
    
        private void Start()
        {
            if (_rebuildOn == UnityLifecycle.Start)
                Rebuild();
        }
    
        private void Rebuild()
        {
            UIRebuilder.instance.Rebuild(GetComponent<RectTransform>());
        }
    }
}

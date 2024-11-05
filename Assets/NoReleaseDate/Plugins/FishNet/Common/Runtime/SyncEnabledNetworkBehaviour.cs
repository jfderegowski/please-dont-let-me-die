using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace NoReleaseDate.FishNet.Common.Runtime
{
    [DefaultExecutionOrder(-5)]
    public class SyncEnabledNetworkBehaviour : NetworkBehaviour
    {
        private readonly SyncVar<bool> _enabled = new();

        protected virtual void Awake()
        {
            _enabled.SetInitialValues(enabled);
            
            _enabled.OnChange += OnEnabledChange;
        }
        
        protected virtual void OnEnable() => SetEnabled(true);

        protected virtual void OnDisable() => SetEnabled(false);

        protected virtual void OnDestroy() => _enabled.OnChange -= OnEnabledChange;
        
        private void OnEnabledChange(bool prev, bool next, bool asServer) => enabled = next;

        private void SetEnabled(bool value)
        {
            if (!NetworkObject || !IsClientInitialized) return;
            
            if (!_enabled.OnStartServerCalled) return;

            if (!_enabled.IsInitialized) return;
            
            if (_enabled.Value == value) return;
            
            SetEnabledServerRpc(value);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetEnabledServerRpc(bool value) => _enabled.Value = value;
    }
}
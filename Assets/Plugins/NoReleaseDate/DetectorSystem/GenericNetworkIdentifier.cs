using FishNet;
using FishNet.Object;
using UnityEngine;

namespace Plugins.NoReleaseDate.DetectorSystem
{
    public class GenericNetworkIdentifier : NetworkIdentifier<GenericNetworkIdentifier>
    {
        [SerializeField] private int _debugId;
        [SerializeField] private GameObject _prefab;
        
        [ContextMenu("Spawn Prefab")]
        private void SpawnPrefab() => SpawnPrefabServerRpc();
        
        [ServerRpc(RequireOwnership = false)]
        private void SpawnPrefabServerRpc()
        {
            var go = Instantiate(_prefab);
            
            InstanceFinder.ServerManager.Spawn(go);
        }
        
        [ContextMenu("Spawn Duplicate")]
        private void Spawn() => SpawnServerRpc();
        
        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc()
        {
            var go = Instantiate(gameObject);
            
            InstanceFinder.ServerManager.Spawn(go);
        }

        private void Update() => _debugId = id;
    }
}
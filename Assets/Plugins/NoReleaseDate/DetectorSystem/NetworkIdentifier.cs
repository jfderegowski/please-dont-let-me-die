using System.Collections.Generic;
using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plugins.NoReleaseDate.DetectorSystem
{
    public class NetworkIdentifier<T> : SyncEnabledNetworkBehaviour
    {
        public static readonly List<NetworkIdentifier<T>> all = new();
        
        public int id
        {
            get => _id.Value;
            [ServerRpc(RequireOwnership = false)]
            private set => _id.Value = value;
        }
        
        public bool isIdInitialized => _id.Value != -1;

        private readonly SyncVar<int> _id = new(-1);
        
        protected override void Awake()
        {
            base.Awake();
            
            _id.OnChange += OnIdChange;
            
            all.Add(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _id.OnChange -= OnIdChange;
            
            all.Remove(this);
        }

        public override void OnStartClient()
        {
            base.OnStartServer();
            
            Initialize();
        }

        private void OnIdChange(int prev, int next, bool asServer)
        {
            if(!IsServerInitialized) return;
            
            if (all.Exists(networkIdentifier => networkIdentifier.id == next && networkIdentifier != this))
                id = GetValidId();
            
            Debug.Log($"{GetType().Name} with ID {prev} has changed to ID {next}", gameObject);
        }

        private void Initialize()
        {
            if (isIdInitialized) return;
            
            var isDuplicate = all.Exists(networkIdentifier => networkIdentifier != this && networkIdentifier.id == id);
            
            if (!isIdInitialized || isDuplicate) id = GetValidId();

            Debug.Log($"OnStartClient {GetType().Name} with ID {id}", gameObject);
        }

        private static int GetValidId()
        {
            var validId = GetRandomId();
            
            while (all.Exists(badge => badge.id == validId))
                validId = GetRandomId();
            
            return validId;
        }
        
        private static int GetRandomId() => Random.Range(1, 1000001);
    }
}
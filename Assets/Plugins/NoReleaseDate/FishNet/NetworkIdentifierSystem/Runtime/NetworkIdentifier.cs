using System.Collections.Generic;
using UnityEngine;

namespace NoReleaseDate.FishNet.NetworkIdentifierSystem.Runtime
{
    public class NetworkIdentifier<T> : NetworkIdentifierBase
    {
        public static readonly List<NetworkIdentifier<T>> all = new();
        
        protected override void Awake()
        {
            base.Awake();
            
            idSync.OnChange += OnIdChange;
            
            all.Add(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            idSync.OnChange -= OnIdChange;
            
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
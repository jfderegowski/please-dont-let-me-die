using FishNet.Object;
using FishNet.Object.Synchronizing;
using NoReleaseDate.FishNet.Common.Runtime;

namespace NoReleaseDate.FishNet.NetworkIdentifierSystem.Runtime
{
    public class NetworkIdentifierBase : SyncEnabledNetworkBehaviour
    {
        public int id
        {
            get => idSync.Value;
            [ServerRpc(RequireOwnership = false)]
            internal set => idSync.Value = value;
        }
        
        public bool isIdInitialized => idSync.Value != -1;

        internal readonly SyncVar<int> idSync = new(-1);
    }
}
using System;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using NoReleaseDate.TickerSystem.Runtime;
using UnityEngine;

namespace Plugins.NoReleaseDate.DetectorSystem
{
    public abstract class Detector<T> : NetworkBehaviour where T : class
    {
        // public event Action<T, RaycastHit> onDetectedChangeBefore;
        // public event Action<T, RaycastHit> onDetectedChange;
        // public event Action<T, RaycastHit> onDetectedChangeAfter;
        //
        // public event Action<T, RaycastHit> onDetectedChangeBeforeServer;
        // public event Action<T, RaycastHit> onDetectedChangeServer;
        // public event Action<T, RaycastHit> onDetectedChangeAfterServer;
        //
        // public event Action<T, RaycastHit> onDetectedChangeBeforeObserver;
        // public event Action<T, RaycastHit> onDetectedChangeObserver;
        // public event Action<T, RaycastHit> onDetectedChangeAfterObserver;
        //
        // public DetectedArgs<T> detectedArgs
        // {
        //     get => _detectedArgsSync.value;
        //     [ServerRpc] set
        //     {
        //         if (detectedArgs.detected == value.detected) return;
        //
        //         onDetectedChangeBefore?.Invoke(_detectedArgs.detected, _detectedArgs.hit);
        //
        //         _detectedArgsSync. = value;
        //     
        //         onDetectedChange?.Invoke(_detectedArgs.detected, _detectedArgs.hit);
        //     
        //         onDetectedChangeAfter?.Invoke(_detectedArgs.detected, _detectedArgs.hit);
        //     }
        // }
        //
        // // SyncVar<>
        //
        // protected virtual Ray ray => new(transform.position, transform.forward);
        //
        // protected virtual float rayMaxDistance => float.PositiveInfinity;
        //
        // protected virtual LayerMask ignoredLayerMask => -5;
        //
        // protected virtual QueryTriggerInteraction queryTriggerInteraction => QueryTriggerInteraction.UseGlobal;
        //
        // protected virtual float raycastFrequency => 0.1f;
        //
        // // private readonly DetectedArgsSync<T> _detectedArgsSync = new();
        //
        // public override void OnOwnershipClient(NetworkConnection prevOwner)
        // {
        //     base.OnOwnershipClient(prevOwner);
        //
        //     if (IsOwner) Ticker.instance.Register(OnTick, raycastFrequency);
        //     else Ticker.instance.Unregister(OnTick);
        // }
        //
        // private void OnTick(float time) => ShootRaycast();
        //
        // protected virtual void ShootRaycast()
        // {
        //     var isHitSomething = Physics.Raycast(ray, out var hit, rayMaxDistance, ignoredLayerMask, queryTriggerInteraction);
        //
        //     if (!isHitSomething)
        //     {
        //         // detectedArgs = (null, hit);
        //         return;
        //     }
        //
        //     var newDetectedObject = OnHitSomething(hit);
        //     
        //     // detectedArgs = (newDetectedObject, hit);
        // }
        //
        // protected virtual T OnHitSomething(RaycastHit hit)
        // {
        //     var hitTransform = hit.transform;
        //
        //     if (!hitTransform) return null;
        //     
        //     T newDetectedObject = default;
        //
        //     if (typeof(T) == typeof(GameObject))
        //         return hitTransform.gameObject as T;
        //     
        //     if (hitTransform.TryGetComponent(out T component)) newDetectedObject = component;
        //     else if (hitTransform.parent.TryGetComponent(out T parentComponent)) newDetectedObject = parentComponent;
        //     else if (hitTransform.root.TryGetComponent(out T rootComponent)) newDetectedObject = rootComponent;
        //
        //     return newDetectedObject;
        // }
    }
}
using System;
using Plugins.SaveSystem.DataStructure;
using SaveSystem.Runtime.Serializable;
using UnityEngine;
using static Plugins.SaveSystem.DataStructure.SaveKey;

namespace Plugins.SaveSystem.Examples.SaveGameSystem.SavableClasses
{
    [Flags]
    public enum RigidbodyDataToSaveTypes
    {
        Position = 1 << 1,
        Rotation = 1 << 2,
        Scale = 1 << 3,
        Mass = 1 << 4,
        Drag = 1 << 5,
        AngularDrag = 1 << 6,
        AutomaticCenterOfMass = 1 << 7,
        AutomaticTensor = 1 << 8,
        UseGravity = 1 << 9,
        IsKinematic = 1 << 10,
        Interpolation = 1 << 11,
        CollisionDetection = 1 << 12,
        Constraints = 1 << 13,
        LayerOverride = 1 << 14,
        Everything = ~(-1 << 15)
    }
    
    public class SavableRigidbody : SavableMonoBehaviour
    {
        private Rigidbody Rigidbody => _rigidbody ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();

        [SerializeField] private RigidbodyDataToSaveTypes _dataToSaveTypes =
            RigidbodyDataToSaveTypes.Position | RigidbodyDataToSaveTypes.Rotation;

        private Rigidbody _rigidbody;

        [SerializeField] private SaveKey _positionSaveKey = RandomKey.WithComment("Position");
        [SerializeField] private SaveKey _rotationSaveKey = RandomKey.WithComment("Rotation");
        [SerializeField] private SaveKey _scaleSaveKey = RandomKey.WithComment("Scale");
        [SerializeField] private SaveKey _massSaveKey = RandomKey.WithComment("Mass");
        [SerializeField] private SaveKey _dragSaveKey = RandomKey.WithComment("Drag");
        [SerializeField] private SaveKey _angularDragSaveKey = RandomKey.WithComment("AngularDrag");
        [SerializeField] private SaveKey _automaticCenterOfMassSaveKey = RandomKey.WithComment("AutomaticCenterOfMass");
        [SerializeField] private SaveKey _automaticTensorSaveKey = RandomKey.WithComment("AutomaticTensor");
        [SerializeField] private SaveKey _useGravitySaveKey = RandomKey.WithComment("UseGravity");
        [SerializeField] private SaveKey _isKinematicSaveKey = RandomKey.WithComment("IsKinematic");
        [SerializeField] private SaveKey _interpolationSaveKey = RandomKey.WithComment("Interpolation");
        [SerializeField] private SaveKey _collisionDetectionSaveKey = RandomKey.WithComment("CollisionDetection");
        [SerializeField] private SaveKey _constraintsSaveKey = RandomKey.WithComment("Constraints");
        [SerializeField] private SaveKey _layerOverrideIncludeSaveKey = RandomKey.WithComment("LayerOverrideInclude");
        [SerializeField] private SaveKey _layerOverrideExcludeSaveKey = RandomKey.WithComment("LayerOverrideExclude");

        private ClassData GetCurrentData()
        {
            var saveData = new ClassData();

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Position))
                saveData.SetKey(_positionSaveKey, new SerializableVector3(Rigidbody.position));
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Rotation))
                saveData.SetKey(_rotationSaveKey, new SerializableQuaternion(Rigidbody.rotation));

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Scale))
                saveData.SetKey(_scaleSaveKey, new SerializableVector3(transform.localScale));
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Mass))
                saveData.SetKey(_massSaveKey, Rigidbody.mass);
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Drag))
                saveData.SetKey(_dragSaveKey, Rigidbody.linearDamping);
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.AngularDrag))
                saveData.SetKey(_angularDragSaveKey, Rigidbody.angularDamping);
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.AutomaticCenterOfMass))
                saveData.SetKey(_automaticCenterOfMassSaveKey, new SerializableVector3(Rigidbody.centerOfMass));
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.AutomaticTensor))
                saveData.SetKey(_automaticTensorSaveKey, new SerializableVector3(Rigidbody.inertiaTensor));
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.UseGravity))
                saveData.SetKey(_useGravitySaveKey, Rigidbody.useGravity);
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.IsKinematic))
                saveData.SetKey(_isKinematicSaveKey, Rigidbody.isKinematic);
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Interpolation))
                saveData.SetKey(_interpolationSaveKey, Rigidbody.interpolation);
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.CollisionDetection))
                saveData.SetKey(_collisionDetectionSaveKey, Rigidbody.collisionDetectionMode);
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Constraints))
                saveData.SetKey(_constraintsSaveKey, Rigidbody.constraints);

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.LayerOverride))
                saveData.SetKey(_layerOverrideIncludeSaveKey, Rigidbody.includeLayers)
                    .SetKey(_layerOverrideExcludeSaveKey, Rigidbody.excludeLayers);

            return saveData;
        }

        #region SaveBehaviour
        
        public override ClassData DefSaveData => GetCurrentData();
        public override ClassData DataToSave => GetCurrentData();

        public override void OnLoad(ClassData classData)
        {
            if (classData == null) return;
            
            
            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Position))
                Rigidbody.position = classData.GetKey(_positionSaveKey, new SerializableVector3(Rigidbody.position));

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Rotation))
                Rigidbody.rotation = classData.GetKey(_rotationSaveKey, new SerializableQuaternion(Rigidbody.rotation));

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Scale))
                transform.localScale = classData.GetKey(_scaleSaveKey, new SerializableVector3(transform.localScale));

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Mass))
                Rigidbody.mass = classData.GetKey(_massSaveKey, Rigidbody.mass);

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Drag))
                Rigidbody.linearDamping = classData.GetKey(_dragSaveKey, Rigidbody.linearDamping);

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.AngularDrag))
                Rigidbody.angularDamping = classData.GetKey(_angularDragSaveKey, Rigidbody.angularDamping);

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.AutomaticCenterOfMass))
                Rigidbody.centerOfMass = classData.GetKey(_automaticCenterOfMassSaveKey, new SerializableVector3(Rigidbody.centerOfMass));

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.AutomaticTensor))
                Rigidbody.inertiaTensor = classData.GetKey(_automaticTensorSaveKey, new SerializableVector3(Rigidbody.inertiaTensor));

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.UseGravity))
                Rigidbody.useGravity = classData.GetKey(_useGravitySaveKey, Rigidbody.useGravity);

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.IsKinematic))
                Rigidbody.isKinematic = classData.GetKey(_isKinematicSaveKey, Rigidbody.isKinematic);

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Interpolation))
                Rigidbody.interpolation = classData.GetKey(_interpolationSaveKey, Rigidbody.interpolation);

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.CollisionDetection))
                Rigidbody.collisionDetectionMode =
                    classData.GetKey(_collisionDetectionSaveKey, Rigidbody.collisionDetectionMode);

            if (_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.Constraints))
                Rigidbody.constraints = classData.GetKey(_constraintsSaveKey, Rigidbody.constraints);

            if (!_dataToSaveTypes.HasFlag(RigidbodyDataToSaveTypes.LayerOverride)) return;
            
            Rigidbody.includeLayers = classData.GetKey(_layerOverrideIncludeSaveKey, Rigidbody.includeLayers);
            Rigidbody.excludeLayers = classData.GetKey(_layerOverrideExcludeSaveKey, Rigidbody.excludeLayers);
        }

        #endregion
    }
}
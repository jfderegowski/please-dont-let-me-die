using NoReleaseDate.Common.Runtime.Properties;
using Plugins.SaveSystem.DataStructure;
using SaveSystem.Runtime.Serializable;
using UnityEngine;
using static Plugins.SaveSystem.DataStructure.SaveKey;

namespace Plugins.SaveSystem.Examples.SaveGameSystem.SavableClasses
{
    public class SavableTransform : SavableMonoBehaviour
    {
        [SerializeField] private HasValue<SaveKey> _positionSaveKey = new(RandomKey.WithComment("Position"), true);
        [SerializeField] private HasValue<SaveKey> _rotationSaveKey = new(RandomKey.WithComment("Rotation"), true);
        [SerializeField] private HasValue<SaveKey> _scaleSaveKey = new(RandomKey.WithComment("Scale"));

        private SerializableVector3 CurrentPosition => transform.position;
        private SerializableQuaternion CurrentRotation => transform.rotation;
        private SerializableVector3 CurrentScale => transform.localScale;

        private ClassData GetCurrentData()
        {
            var saveData = new ClassData();

            if (_positionSaveKey.hasValue)
                saveData.SetKey(_positionSaveKey, CurrentPosition);

            if (_rotationSaveKey.hasValue)
                saveData.SetKey(_rotationSaveKey, CurrentRotation);

            if (_scaleSaveKey.hasValue)
                saveData.SetKey(_scaleSaveKey, CurrentScale);

            return saveData;
        }
        
        #region SaveBehaviour
        
        public override ClassData DefSaveData => GetCurrentData();
        public override ClassData DataToSave => GetCurrentData();

        public override void OnLoad(ClassData savedData)
        {
            if (savedData == null) return;

            if (_positionSaveKey.hasValue) 
                transform.position = savedData.GetKey(_positionSaveKey, CurrentPosition);

            if (_rotationSaveKey.hasValue) 
                transform.rotation = savedData.GetKey(_rotationSaveKey, CurrentRotation);

            if (_scaleSaveKey.hasValue) 
                transform.localScale = savedData.GetKey(_scaleSaveKey, CurrentScale);
        }

        #endregion
    }
}
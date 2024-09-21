using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputIcons.II_ImagePrompt;

namespace InputIcons
{
    public class II_ImagePromptBehaviour : MonoBehaviour
    {
        public List<ImagePromptData> spritePromptDatas = new List<ImagePromptData>();


        private void Start()
        {
            UpdateDisplayedImages();
            InputIconsManagerSO.onBindingsChanged += UpdateDisplayedImages;
            InputIconsManagerSO.onControlsChanged += UpdateDisplayedSprites;
        }

        private void OnDestroy()
        {
            InputIconsManagerSO.onBindingsChanged -= UpdateDisplayedImages;
            InputIconsManagerSO.onControlsChanged -= UpdateDisplayedSprites;
        }

        public void UpdateDisplayedImages()
        {
            foreach (ImagePromptData s in spritePromptDatas)
            {
                s.UpdateDisplayedSprite();
            }
#if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate();
#endif
        }
        private void UpdateDisplayedSprites(InputDevice inputDevice)
        {
            UpdateDisplayedImages();
        }


#if UNITY_EDITOR
        public void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

        private void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            if (this == null) return;
            UpdateDisplayedImages();
        }
#endif

     
    }
}

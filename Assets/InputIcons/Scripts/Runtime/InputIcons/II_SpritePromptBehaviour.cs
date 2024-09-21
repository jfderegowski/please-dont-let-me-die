using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputIcons.II_SpritePrompt;

namespace InputIcons
{
    public class II_SpritePromptBehaviour : MonoBehaviour
    {

        public List<SpritePromptData> spritePromptDatas = new List<SpritePromptData>();


        private void Start()
        {
            UpdateDisplayedSprites();
            InputIconsManagerSO.onBindingsChanged += UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged += UpdateDisplayedSprites;
        }

        private void OnDestroy()
        {
            InputIconsManagerSO.onBindingsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged -= UpdateDisplayedSprites;
        }

        public void UpdateDisplayedSprites()
        {
            foreach (SpritePromptData s in spritePromptDatas)
            {
                s.UpdateDisplayedSprite();
            }

        }
        private void UpdateDisplayedSprites(InputDevice inputDevice)
        {
            UpdateDisplayedSprites();
        }


#if UNITY_EDITOR
        public void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

        private void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            if (this == null) return;

            UpdateDisplayedSprites();
        }
#endif

    }
}


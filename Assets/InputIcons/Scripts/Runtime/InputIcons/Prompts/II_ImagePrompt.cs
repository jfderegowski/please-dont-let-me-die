using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InputIcons.II_SpritePrompt;

namespace InputIcons
{
    public class II_ImagePrompt : MonoBehaviour
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
            foreach (ImagePromptData s in spritePromptDatas)
            {
                s.ValidatePromptData();
            }
            UpdateDisplayedImages();
        }
#endif

        [System.Serializable]
        public class ImagePromptData : II_PromptData
        {
            
            public Image image;

            public ImagePromptData() : base()
            {

            }

            public ImagePromptData(ImagePromptData data) : base(data)
            {
                if (data == null)
                    return;

                image = data.image;
            }

            public static List<ImagePromptData> CloneList(List<ImagePromptData> originalList)
            {
                if (originalList == null)
                    return null;

                List<ImagePromptData> clonedList = new List<ImagePromptData>();

                foreach (var item in originalList)
                {
                    ImagePromptData clonedItem = new ImagePromptData(item);
                    clonedList.Add(clonedItem);
                }

                return clonedList;
            }

            public void UpdateDisplayedSprite()
            {
                if (actionReference == null)
                {
                    if (image != null)
                        image.sprite = null;
                    return;
                }


                if (image)
                    image.sprite = GetKeySprite();
            }

        }
    }
}

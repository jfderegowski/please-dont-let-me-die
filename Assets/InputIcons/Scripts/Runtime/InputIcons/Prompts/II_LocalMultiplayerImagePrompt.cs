using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InputIcons
{
    public class II_LocalMultiplayerImagePrompt : MonoBehaviour
{

        public List<ImagePromptData> spritePromptDatas = new List<ImagePromptData>();

        private void Awake()
        {
            UpdateDisplayedSprites();
            InputIconsManagerSO.onBindingsChanged += UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged += UpdateDisplayedSprites;
            InputIconsManagerSO.onInputUsersChanged += UpdateDisplayedSprites;
        }

        private void OnDestroy()
        {
            InputIconsManagerSO.onBindingsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onInputUsersChanged -= UpdateDisplayedSprites;
        }

        public void UpdateDisplayedSprites()
        {
            //Debug.Log("update sprites");
            foreach (ImagePromptData s in spritePromptDatas)
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

        [System.Serializable]
        public class ImagePromptData : II_LocalMultiplayerSpritePrompt.II_PromptDataLocalMultiplayer
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
                {
                    Sprite aSprite = GetKeySprite();
                    if (aSprite != null)
                    {
                        image.sprite = aSprite;
                    }

                }

            }

        }
    }
}

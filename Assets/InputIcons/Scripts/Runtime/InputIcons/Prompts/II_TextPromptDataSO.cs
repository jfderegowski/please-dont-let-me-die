using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputIcons
{
    [CreateAssetMenu(menuName = "InputIcons/TextPromptDataSO")]
    public class II_TextPromptDataSO : ScriptableObject
    {

        public List<II_TextPrompt.TextPromptData> textPromptDatas = new List<II_TextPrompt.TextPromptData>();


        public bool CanMoveUp(int id)
        {
            if (id < 0)
                return false;

            if (id > textPromptDatas.Count - 1)
                return false;

            return true;
        }

        public bool CanMoveDown(int id)
        {
            if (id < 1)
                return false;

            if (id > textPromptDatas.Count - 2)
                return false;

            return true;
        }

        public void MoveDataUp(int id)
        {
            if (!CanMoveUp(id))
                return;

            if (id > 0)
            {
                II_TextPrompt.TextPromptData data = textPromptDatas[id];
                // Swap the item with the previous one
                II_TextPrompt.TextPromptData temp = textPromptDatas[id - 1];
                textPromptDatas[id - 1] = data;
                textPromptDatas[id] = temp;
            }
        }

        public void MoveDataDown(int id)
        {
            if (!CanMoveDown(id))
                return;

            if (id < textPromptDatas.Count - 1 && id != -1)
            {
                II_TextPrompt.TextPromptData data = textPromptDatas[id];
                // Swap the item with the next one
                II_TextPrompt.TextPromptData temp = textPromptDatas[id + 1];
                textPromptDatas[id + 1] = data;
                textPromptDatas[id] = temp;
            }
        }
    }

}


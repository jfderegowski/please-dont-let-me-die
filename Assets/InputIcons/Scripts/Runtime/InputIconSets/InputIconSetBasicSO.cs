using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Runtime.CompilerServices;

namespace InputIcons
{
    public abstract class InputIconSetBasicSO : ScriptableObject
    {
        protected enum SearchPattern { One, All };

        public string iconSetName;

        public Color deviceDisplayColor;

        public TMP_FontAsset fontAsset;

        public InputSpriteData unboundData = new InputSpriteData("Unbound Sprite", null, "", "");
        public InputSpriteData fallbackData = new InputSpriteData("Fallback Sprite", null, "FallbackSprite", "");

        public List<CustomInputContextIcon> customContextIcons = new List<CustomInputContextIcon>();



        public bool HasSprite(string spriteName)
        {
            spriteName = InputIconsUtility.GetCorrectKeyForSpecialCases(spriteName);

            if(this is InputIconSetKeyboardSO)
            {
                spriteName = spriteName.Replace(" ", "");
            }


            List<InputSpriteData> spriteData = GetAllInputSpriteData();
            for (int i = 0; i < spriteData.Count; i++)
            {
                if (spriteData[i].textMeshStyleTag.ToUpper() == spriteName.ToUpper())
                {
                    if (spriteData[i].sprite == null)
                        return false;
                    else
                        return true;
                }
                   
            }

            return false;
        }

        public virtual Sprite GetSprite(string spriteName)
        { 
           
            spriteName = InputIconsUtility.GetCorrectKeyForSpecialCases(spriteName);
            if (this is InputIconSetKeyboardSO)
            {
                spriteName = spriteName.Replace(" ", "");
            }

            List<InputSpriteData> spriteData = GetAllInputSpriteData();
            for (int i = 0; i < spriteData.Count; i++)
            {
                if (spriteData[i].textMeshStyleTag.ToUpper() == spriteName.ToUpper())
                {
                    if (spriteData[i].sprite != null)
                        return spriteData[i].sprite;
                    else
                        return fallbackData.sprite;
                }

            }

            return unboundData.sprite;
        }

        public virtual List<Sprite> GetSprites(List<string> spriteNames)
        {
            List<Sprite> outputList = new List<Sprite>();
            List<InputSpriteData> spriteData = GetAllInputSpriteData();

            foreach(string spriteName in spriteNames)
            {
                outputList.Add(GetSprite(spriteName));
            }

            /*
            for (int x=0; x< spriteNames.Count; x++)
            {
                for (int i = 0; i < spriteData.Count; i++)
                {
                    if (spriteData[i].textMeshStyleTag.ToUpper() == InputIconsUtility.GetCorrectKeyForSpecialCases(spriteNames[x]).ToUpper())
                    {
                        if (spriteData[i].sprite != null)
                            outputList.Add(spriteData[i].sprite);
                        else
                            outputList.Add(fallbackData.sprite);

                    }
                }
            }
            */
            return outputList;
        }

        public virtual List<Sprite> GetSprites(List<List<string>> spriteNameGroups)
        {
            List<Sprite> outputList = new List<Sprite>();
            List<InputSpriteData> spriteData = GetAllInputSpriteData();

            foreach (List<string> spriteNameGroup in spriteNameGroups)
            {
                foreach(string spriteName in spriteNameGroup)
                {
                    outputList.Add(GetSprite(spriteName));
                }
            }

            /*
            for (int x = 0; x < spriteNameGroups.Count; x++)
            {
                for(int y = 0; y< spriteNameGroups[x].Count; y++)
                {
                    for (int i = 0; i < spriteData.Count; i++)
                    {
                        if (spriteData[i].textMeshStyleTag.ToUpper() == InputIconsUtility.GetCorrectKeyForSpecialCases(spriteNameGroups[x][y]).ToUpper())
                        {
                            if (spriteData[i].sprite != null)
                                outputList.Add(spriteData[i].sprite);
                            else
                                outputList.Add(fallbackData.sprite);
                        }
                    }
                }
         
            }
            */

            return outputList;
        }

        public virtual string GetSpriteTag(List<string> spriteNames, bool allowTinting)
        {
            string s = "";
            for(int i=0; i< spriteNames.Count; i++)
            {
                s += GetSpriteTag(spriteNames[i], allowTinting);
            }
            return s;
        }

        public virtual string GetSpriteTag(List<List<string>> spriteNameGroups, bool allowTinting, string delimiter)
        {
            string s = "";
            for (int i = 0; i < spriteNameGroups.Count; i++)
            {
                if(i>0)
                {
                    s += delimiter;
                }

                for(int j=0; j < spriteNameGroups[i].Count; j++)
                {
                    s += GetSpriteTag(spriteNameGroups[i][j], allowTinting);
                }
              
            }
            return s;
        }

        public string GetSpriteTag(string keyName, bool allowTinting = false)
        {
            keyName = InputIconsUtility.GetCorrectKeyForSpecialCases(keyName);
            
            if(!HasSprite(keyName))
            {
                keyName = unboundData.textMeshStyleTag;
            }

            string output = InputIconsManagerSO.TEXT_OPENING_TAG_VALUE + "<sprite=\"" + iconSetName + "\" name=\"" + keyName.ToUpper() + "\"";
            if (allowTinting)
                output += ", tint";

            output += ">";
            output += InputIconsManagerSO.TEXT_CLOSING_TAG_VALUE;
            return output;
        }


        public string GetFontTag( string keyName)
        {
            keyName = InputIconsUtility.GetCorrectKeyForSpecialCases(keyName);

            if (!HasSprite(keyName))
            {
                keyName = unboundData.textMeshStyleTag;
            }

            string fontTag = "";

            if (fontAsset != null)
                fontTag = "<font=\"" + fontAsset.name + "\"";
            else
            {
                fontTag = "<font=\"InputIcons_Keyboard_Font SDF\"";
            }

            fontTag += ">";

            fontTag += "\\u" + GetFontCode(keyName);

            fontTag += "</font>";

            return fontTag;
        }

        public string GetFontTag(List<string> keyNames)
        {
            string fontTag;

             if (fontAsset != null)
                fontTag = "<font=\"" + fontAsset.name + "\">";
            else
            {
                fontTag = "<font=\"InputIcons_Keyboard_Font SDF\">";
            }

            for(int i=0; i < keyNames.Count; i++)
            {
                fontTag += "\\u" + GetFontCode(keyNames[i]);
            }

            fontTag += "</font>";

            return fontTag;
        }

        public string GetFontTag(List<List<string>> spriteNameGroups, string delimiter)
        {
            string fontTag;

            if (fontAsset != null)
                fontTag = "<font=\"" + fontAsset.name + "\">";
            else
            {
                fontTag = "<font=\"InputIcons_Keyboard_Font SDF\">";
            }

            for(int i=0; i<spriteNameGroups.Count; i++)
            {
                if(i>0)
                {
                    fontTag += delimiter;
                }
                for(int j=0; j < spriteNameGroups[i].Count; j++)
                {
                    fontTag += "\\u" + GetFontCode(spriteNameGroups[i][j]);
                }
            }

            return fontTag;
        }


        public virtual string GetFontCode(string keyName)
        {
            keyName = InputIconsUtility.GetCorrectKeyForSpecialCases(keyName);

            List<InputSpriteData> spriteData = GetAllInputSpriteData();
            for (int i = 0; i < spriteData.Count; i++)
            {
                if (spriteData[i].textMeshStyleTag.ToUpper() == keyName.ToUpper())
                {
                    return spriteData[i].fontCode;
                }

            }

            return unboundData.fontCode;
        }

        public abstract void TryGrabSprites();
        public abstract List<InputSpriteData> GetAllInputSpriteData();


        public abstract void ApplyFontCodes(List<KeyValuePair<string, string>> fontCodes);

        protected Sprite GetSpriteFromList(List<Sprite> spriteList, string[] spriteTags, SearchPattern pattern)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                int count = 0;
                for (int j = 0; j < spriteTags.Length; j++)
                {

                    if (spriteList[i].name.ToUpper().Contains(spriteTags[j].ToUpper()))
                    {
                        count++;
                        if (pattern == SearchPattern.One)
                            return spriteList[i];
                    }
                }
                if (pattern == SearchPattern.All && count >= spriteTags.Length)
                    return spriteList[i];
            }

            string s = "";
            for (int i = 0; i< spriteTags.Length; i++)
                s += spriteTags[i].ToString()+" ";

            //InputIconsLogger.Log("Sprite not found, "+ s);
            return null;
        }

        protected List<Sprite> GetSpritesAtPath(string path)
        {
            List<Sprite> sprites = new List<Sprite>();
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets( "t:Sprite", new string[] {path} );


            foreach (string o in guids)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(o).ToString();
                //Debug.Log(spritePath);
                Sprite s = (Sprite)AssetDatabase.LoadAssetAtPath(spritePath, typeof(Sprite));
                sprites.Add(s);
            }
#endif
            return sprites;
        }
    }

    [System.Serializable]
    public class CustomInputContextIcon
    {

        public Sprite customInputContextSprite;
        public string fontCode;
        public string textMeshStyleTag;
    }

    [System.Serializable]
    public class InputSpriteData
    {
        [HideInInspector]
        private string buttonName;
        public string textMeshStyleTag;
        public Sprite sprite;
        public string fontCode;


        public InputSpriteData(string buttonName, Sprite aSprite, string tag, string aFontCode = "")
        {
            this.buttonName = buttonName;
            sprite = aSprite;
            textMeshStyleTag = tag;
            fontCode = aFontCode;
        }

        public void SetFontCode(string code)
        {
            fontCode = code;
        }

        public string GetButtonName()
        {
            if (buttonName == "")
                return textMeshStyleTag;
            return buttonName;
        }

    }
}
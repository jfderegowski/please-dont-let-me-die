using System;
using NoReleaseDate.Common.Runtime.Properties;

namespace NoReleaseDate.Plugins.SaveSystem.Runtime.Settings
{
    [Serializable]
    public class SaveSettings
    {
        public HasValue<int> FileLimit = new(100, true);
        public HasValue<JsonSettings> JsonSettings = new(new JsonSettings(), true);
        public HasValue<EncryptionSettings> EncryptionSettings = new(new EncryptionSettings(), true);
    }
}
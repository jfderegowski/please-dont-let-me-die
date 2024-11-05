using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoReleaseDate.Common.Runtime.Serializable;
using NoReleaseDate.Plugins.SaveSystem.Runtime.Encryption;
using NoReleaseDate.Plugins.SaveSystem.Runtime.Settings;
using UnityEngine;

namespace NoReleaseDate.Plugins.SaveSystem.Runtime.DataStructure
{
    public class SaveData : BaseData<ClassData>
    {
        public async Task Save(string path, SaveSettings saveSettings = null,
            Action onBeforeSave = null, Action onAfterSave = null)
        {
            onBeforeSave?.Invoke();

            try
            {
                await Task.Run(Write);
                
                Debug.Log($"[SAVE-MANAGER] Saved to File: {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SAVE-MANAGER] Error saving to file: {path}\n{e.Message}");
            }
            
            onAfterSave?.Invoke();
            
            return;

            async Task Write()
            {
                saveSettings ??= DefaultSaveSettings.instance.SaveSettings;

                var saveDataCopy = new SaveData { Data = Data };
                var jsonSerializerSettings = saveSettings.JsonSettings.hasValue
                    ? saveSettings.JsonSettings.Value.JsonSerializerSettings
                    : new JsonSerializerSettings();
                
                var jsonString = JsonConvert.SerializeObject(saveDataCopy, jsonSerializerSettings);

                if (saveSettings.EncryptionSettings.hasValue)
                {
                    var password = saveSettings.EncryptionSettings.Value.Password;
                    var salt = saveSettings.EncryptionSettings.Value.Salt;
                    var initVector = saveSettings.EncryptionSettings.Value.InitVector;
                    
                    jsonString = Aes.Encrypt(jsonString, password, salt, initVector);
                }
                
                await File.WriteAllTextAsync(path, jsonString);

                // Delete excess files
                if (saveSettings.FileLimit.hasValue) 
                    await SaveSystemHelpers.DeleteExcessFiles(Path.GetDirectoryName(path), saveSettings.FileLimit.Value);
            }
        }

        public async Task Load(string path, SaveSettings saveSettings = null,
            Action onBeforeLoad = null, Action onAfterLoad = null)
        {
            onBeforeLoad?.Invoke();
            
            try
            {
                var fileSaveData = await Task.Run(Read);

                Data = fileSaveData.Data;

                Debug.Log($"[SAVE-MANAGER] Loaded from File: {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SAVE-MANAGER] Error loading from file: {path}\n{e.Message}");
                return;
            }

            onAfterLoad?.Invoke();

            return;

            async Task<SaveData> Read()
            {
                saveSettings ??= DefaultSaveSettings.instance.SaveSettings;
                
                var jsonSerializerSettings = saveSettings.JsonSettings.hasValue
                    ? saveSettings.JsonSettings.Value.JsonSerializerSettings
                    : new JsonSerializerSettings();
                
                var jsonText = await File.ReadAllTextAsync(path);
                
                if (saveSettings.EncryptionSettings.hasValue)
                {
                    var password = saveSettings.EncryptionSettings.Value.Password;
                    var salt = saveSettings.EncryptionSettings.Value.Salt;
                    var initVector = saveSettings.EncryptionSettings.Value.InitVector;
                    
                    jsonText = Aes.Decrypt(jsonText, password, salt, initVector);
                }

                var saveData = !string.IsNullOrEmpty(jsonText)
                    ? JsonConvert.DeserializeObject<SaveData>(jsonText, jsonSerializerSettings)
                    : new SaveData();

                return saveData;
            }
        }

        internal class JsonConverter : JsonConverter<SaveData>
        {
            public override void WriteJson(JsonWriter writer, SaveData value, JsonSerializer serializer)
            {
                writer.WriteComment(" Save System Made by: NO PUBLISH DATE ");
                writer.WriteWhitespace("\n");
                writer.WriteComment($" Save at time: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ");
                writer.WriteWhitespace("\n\n");

                writer.WriteStartObject();

                foreach (var saveData in value.Data)
                {
                    if (saveData.Key.Comment.hasValue)
                    {
                        writer.WriteWhitespace("\n\t");
                        writer.WriteComment(saveData.Key.Comment);
                    }

                    writer.WritePropertyName(saveData.Key.StringKey.hasValue
                        ? saveData.Key.StringKey
                        : saveData.Key.Key.ToHexString());

                    writer.WriteStartObject();

                    foreach (var clasData in saveData.Value.Data)
                    {
                        if (clasData.Key.Comment.hasValue)
                        {
                            writer.WriteWhitespace("\n\t\t");
                            writer.WriteComment(clasData.Key.Comment);
                        }
                        
                        writer.WritePropertyName(clasData.Key.StringKey.hasValue
                            ? clasData.Key.StringKey
                            : clasData.Key.Key.ToHexString());
                        
                        serializer.Serialize(writer, clasData.Value);
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }

            public override SaveData ReadJson(JsonReader reader, Type objectType, SaveData existingValue,
                bool hasExistingValue, JsonSerializer serializer)
            {
                // Validate object type (optional, adjust based on your logic)
                if (objectType != typeof(SaveData))
                    throw new ArgumentException("Expected SaveData type");

                // Skip comments and whitespace at the beginning
                reader.Read();
                while (reader.TokenType == JsonToken.Comment)
                    reader.Read();

                // Ensure the start of an object is expected
                if (reader.TokenType != JsonToken.StartObject)
                    throw new JsonException("Expected start of object");

                // Skip the start of the object
                reader.Read();

                // Create or use existing SaveData object
                // var result = hasExistingValue ? existingValue : new SaveData();
                var saveData = new SaveData();

                // Read key-value pairs until the end of the object
                while (reader.TokenType != JsonToken.EndObject)
                {
                    var saveDataComment = string.Empty;
                    if (reader.TokenType == JsonToken.Comment)
                    {
                        saveDataComment = (string)reader.Value;
                        reader.Read();
                    }

                    if (reader.TokenType != JsonToken.PropertyName)
                        throw new JsonException("Expected property name");

                    // Deserialize the key
                    var statDataKeyTmp = (string)reader.Value;
                    var statDataKey = statDataKeyTmp.Length == 32
                        ? new SaveKey(SerializableGuid.FromHexString(statDataKeyTmp), saveDataComment)
                        : new SaveKey(statDataKeyTmp, saveDataComment);
                    
                    reader.Read(); // Move to the value

                    // Create a new inner dictionary to store the key-value pairs
                    var classData = new ClassData();

                    if (reader.TokenType != JsonToken.StartObject)
                        throw new JsonException("Expected start of inner object");

                    // Read key-value pairs within the inner object
                    reader.Read(); // Move to the first element in the inner object
                    while (reader.TokenType != JsonToken.EndObject)
                    {
                        var classDataComment = string.Empty;
                        if (reader.TokenType == JsonToken.Comment)
                        {
                            classDataComment = (string)reader.Value;
                            reader.Read();
                        }

                        if (reader.TokenType != JsonToken.PropertyName)
                            throw new JsonException("Expected property name in inner object");

                        
                        // Deserialize the key
                        var classDataKeyTmp = (string)reader.Value;
                        var classDataKey = classDataKeyTmp.Length == 32
                            ? new SaveKey(SerializableGuid.FromHexString(classDataKeyTmp), classDataComment)
                            : new SaveKey(classDataKeyTmp, classDataComment);
                        
                        reader.Read(); // Move to the value

                        // Deserialize the value using a suitable deserializer based on its type
                        var classDataValue = reader.TokenType == JsonToken.Null ? null : serializer.Deserialize(reader);

                        classData.SetKey(classDataKey, classDataValue);
                        reader.Read(); // Move to the next element in the inner object
                    }

                    // Add the parsed key-value pair to the result's data
                    saveData.SetKey(statDataKey, classData);
                    reader.Read(); // Move to the next key-value pair in the outer object
                }

                return saveData;
            }
        }
    }
}
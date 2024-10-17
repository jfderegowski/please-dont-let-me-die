using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaveSystem.Runtime.DataStructure;
using SaveSystem.Runtime.Extensions;
using SaveSystem.Runtime.Serializable;
using UnityEngine;

namespace Plugins.SaveSystem.DataStructure
{
    public class SaveData : BaseData<ClassData>
    {
        public SaveData()
        {

        }

        public SaveData(SaveData data) : base(data.Data)
        {

        }

        public static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.Indented,
            Converters = { new SaveDataJsonConverter() }
        };

        public async Task<string> Save(string folderPath, int fileLimit = 0,
            Action onAfterSave = null, Action onBeforeSave = null)
        {
            if (onBeforeSave != null) await onBeforeSave.InvokeAsync();

            var fileName = await GetUniqueFileName(folderPath);
            var savePath = $"{folderPath}/{fileName}.sav";
            var saveResult = await WriteSaveDataToFile(savePath, this);
            await DeleteExcessFiles(folderPath, fileLimit);

            if (!saveResult) return null;

            Debug.Log($"[SAVE-MANAGER] Saved to File: {savePath}");

            if (onAfterSave != null) await onAfterSave.InvokeAsync();

            return savePath;
        }

        public async Task Load(string filePath,
            Action onAfterLoad = null, Action onBeforeLoad = null)
        {
            if (onBeforeLoad != null) await onBeforeLoad.InvokeAsync();

            Debug.Log($"[SAVE-MANAGER] Loading from File: {filePath}");

            var fileSaveData = await Task.Run(ReadFromFileTask);

            Debug.Log($"[SAVE-MANAGER] 0000 Loaded from File: {filePath}");

            Data = fileSaveData.Data;

            Debug.Log($"[SAVE-MANAGER] Loaded from File: {filePath}");

            if (onAfterLoad != null) await onAfterLoad.InvokeAsync();

            return;

            Task<SaveData> ReadFromFileTask()
            {
                var jsonText = File.ReadAllText(filePath);

                var saveData = !string.IsNullOrEmpty(jsonText)
                    ? JsonConvert.DeserializeObject<SaveData>(jsonText, JsonSerializerSettings)
                    : new SaveData();

                return Task.FromResult(saveData);
            }
        }

        public FileInfo[] GetSaveFiles(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return new DirectoryInfo(folderPath).GetFiles();
        }

        private async Task<string> GetUniqueFileName(string folderPath) =>
            await Task.Run(() =>
            {
                var filesNames = GetSaveFiles(folderPath).Select(f => f.Name).ToList();

                var fileName = Guid.NewGuid().ToString();

                while (filesNames.Contains(fileName))
                    fileName = Guid.NewGuid().ToString();

                return fileName;
            });

        private async Task DeleteExcessFiles(string folderPath, int fileLimit)
        {
            if (fileLimit <= 0) return;

            await Task.Run(() =>
            {
                var saveFiles = GetSaveFiles(folderPath).SortOldestFirst().ToArray();
                var filesToDelete = saveFiles.Length - fileLimit;

                for (var i = 0; i < filesToDelete; i++)
                    saveFiles[i].Delete();
            });
        }

        private static async Task<bool> WriteSaveDataToFile(string newFilePath, SaveData saveData)
        {
            // Copy the data to avoid errors of modifying the original data
            var saveDataCopy = new SaveData(saveData);

            return await Task.Run(async () =>
            {
                try
                {
                    var jsonString = JsonConvert.SerializeObject(saveDataCopy, JsonSerializerSettings);

                    await File.WriteAllTextAsync(newFilePath, jsonString);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SAVE-MANAGER] Error saving to file: {newFilePath}\n{e.Message}");
                    return false;
                }

                return true;
            });
        }

        private class SaveDataJsonConverter : JsonConverter<SaveData>
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

                        classData.SetKey(classDataKey, classDataValue, classDataComment);
                        reader.Read(); // Move to the next element in the inner object
                    }

                    // Add the parsed key-value pair to the result's data
                    saveData.SetKey(statDataKey, classData, saveDataComment);
                    reader.Read(); // Move to the next key-value pair in the outer object
                }

                return saveData;
            }
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NoReleaseDate.Common.Runtime.Extensions;

namespace NoReleaseDate.Plugins.SaveSystem
{
    public static class SaveSystemHelpers
    {
        public static async Task<string> GetUniqueFileName(string folderPath) =>
            await Task.Run(() =>
            {
                var filesNames = GetSaveFiles(folderPath).Select(f => f.Name).ToList();

                var fileName = Guid.NewGuid().ToString();

                while (filesNames.Contains(fileName))
                    fileName = Guid.NewGuid().ToString();

                return fileName;
            });
        
        public static FileInfo[] GetSaveFiles(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return new DirectoryInfo(folderPath).GetFiles();
        }

        public static async Task DeleteExcessFiles(string folderPath, int fileLimit)
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
    }
}
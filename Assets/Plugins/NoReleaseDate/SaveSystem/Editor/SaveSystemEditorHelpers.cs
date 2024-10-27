using System.IO;

namespace NoReleaseDate.SaveSystem.Editor
{
    public class SaveSystemEditorHelpers
    {
        public static void OpenFolder(string folderPath, bool createIfNotExists = true)
        {
            if (string.IsNullOrEmpty(folderPath) || string.IsNullOrWhiteSpace(folderPath))
                return;

            if (!Directory.Exists(folderPath))
            {
                if (createIfNotExists) 
                    Directory.CreateDirectory(folderPath);
                else return;
            }
            
            System.Diagnostics.Process.Start(folderPath);
        }
        
        public static void OpenFile(string filePath, bool createIfNotExists = true)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath))
                return;

            if (!File.Exists(filePath))
            {
                if (createIfNotExists) 
                    File.Create(filePath).Close();
                else return;
            }
            
            System.Diagnostics.Process.Start(filePath);
        }
    }
}
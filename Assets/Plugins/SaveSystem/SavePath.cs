using System.IO;

namespace SaveSystem
{
    public static class SavePath
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
            
        public static string GetFolderPath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            return path;
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
            
        public static string GetFilePath(string path)
        {
            if (!File.Exists(path))
                File.Create(path).Close();
            
            return path;
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NoReleaseDate.Common.Runtime.Extensions
{
    public static class FileInfoExtensions
    {
        public static IEnumerable<FileInfo> SortLatestFirst(this IEnumerable<FileInfo> saveFiles) =>
            saveFiles.OrderByDescending(f => f.LastWriteTime).ToArray();

        public static IEnumerable<FileInfo> SortOldestFirst(this IEnumerable<FileInfo> saveFiles) =>
            saveFiles.OrderBy(f => f.LastWriteTime).ToArray();

        public static FileInfo GetLatest(this IEnumerable<FileInfo> saveFiles) =>
            saveFiles.SortLatestFirst().FirstOrDefault();

        public static FileInfo GetOldest(this IEnumerable<FileInfo> saveFiles) =>
            saveFiles.SortOldestFirst().FirstOrDefault();
    }
}
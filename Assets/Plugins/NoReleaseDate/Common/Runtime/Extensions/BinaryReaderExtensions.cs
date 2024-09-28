using System.IO;
using NoReleaseDate.Common.Runtime.Serializable;

namespace NoReleaseDate.Common.Runtime.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static SerializableGuid Read(this BinaryReader reader) =>
            new(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
    }
}

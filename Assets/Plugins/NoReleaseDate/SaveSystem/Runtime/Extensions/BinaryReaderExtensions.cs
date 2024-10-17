using System.IO;
using SaveSystem.Runtime.Serializable;

namespace SaveSystem.Runtime.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static SerializableGuid Read(this BinaryReader reader) =>
            new(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
    }
}

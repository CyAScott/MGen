using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable SYSLIB0011 // Type or member is obsolete

namespace MGen.Tests.SerializationSupport.DotNet
{
    static class SerializerHelper
    {
        static readonly BinaryFormatter formatter = new();

        public static T Clone<T>(this T value)
            where T : ISerializable
        {
            var stream = new MemoryStream();

            formatter.Serialize(stream, value);

            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }
}

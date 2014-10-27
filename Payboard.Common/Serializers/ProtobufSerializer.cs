using System.IO;
using ProtoBuf;

namespace Payboard.Common.Serializers
{
    /// <summary>
    ///     Wraps protobuf.net so that it can be swapped out with other serializers in situations where performance and size
    ///     isn't so critical
    /// </summary>
    public class ProtobufSerializer : ISerializer
    {
        public MemoryStream SerializeToStream<T>(T obj)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, obj);
            stream.Position = 0;
            return stream;
        }

        public string SerializeToString<T>(T obj)
        {
            var ms = SerializeToStream(obj);
            var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public T Deserialize<T>(Stream stream)
        {
            stream.Position = 0;
            return Serializer.Deserialize<T>(stream);
        }

        public T Deserialize<T>(string str)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(str);
            ms.Position = 0;
            return Deserialize<T>(ms);
        }
    }
}
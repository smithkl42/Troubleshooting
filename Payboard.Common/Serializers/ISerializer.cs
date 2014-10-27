using System.IO;

namespace Payboard.Common.Serializers
{
    public interface ISerializer
    {
        MemoryStream SerializeToStream<T>(T obj);
        string SerializeToString<T>(T obj);
        T Deserialize<T>(Stream stream);
        T Deserialize<T>(string str);
    }
}

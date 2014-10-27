using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Payboard.Common.Serializers
{
    public class JsonSerializer : ISerializer
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings;

        static JsonSerializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
        }

        public MemoryStream SerializeToStream<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
            var stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
            stream.Position = 0;
            return stream;
        }

        public string SerializeToString<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
            return json;
        }

        public T Deserialize<T>(Stream stream)
        {
            stream.Position = 0;
            var reader = new StreamReader(stream, Encoding.Unicode);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public T Deserialize<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
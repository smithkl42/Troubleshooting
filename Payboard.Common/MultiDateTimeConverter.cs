using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Payboard.Common
{
    public class MultiDateTimeConverter : DateTimeConverterBase
    {
        public MultiDateTimeConverter(params DateTimeConverterBase[] converters)
        {
            Converters = converters.ToList();
        }

        public List<DateTimeConverterBase> Converters { get; set; }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Exception lastException = null;
            foreach (var converter in Converters)
            {
                try
                {
                    converter.WriteJson(writer, value, serializer);
                    return;
                }
                catch(Exception ex)
                {
                    lastException = ex;
                }
            }
            if (lastException != null)
            {
                throw lastException;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Exception lastException = null;
            foreach (var converter in Converters)
            {
                try
                {
                    return converter.ReadJson(reader, objectType, existingValue, serializer);
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }
            if (lastException != null)
            {
                throw lastException;
            }
            return null;
        }
    }
}

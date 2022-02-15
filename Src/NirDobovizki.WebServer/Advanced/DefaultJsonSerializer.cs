using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public class DefaultJsonSerializer : IJsonSerializer
    {
        public object Deserialize(string json, Type targetType)
        {
#if NETSTANDARD2_0
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, targetType);
#else
            var opt = new System.Text.Json.JsonSerializerOptions();
            opt.IncludeFields = true;
            return System.Text.Json.JsonSerializer.Deserialize(json, targetType, opt);
#endif
        }

        public string Serialize(object value)
        {
#if NETSTANDARD2_0
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
#else
            var opt = new System.Text.Json.JsonSerializerOptions();
            opt.IncludeFields = true;
            return System.Text.Json.JsonSerializer.Serialize(value, opt);
#endif
        }
    }
}

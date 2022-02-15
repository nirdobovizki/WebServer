using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public class RequestParamConverter
    {
        static Dictionary<Type, Func<string, object>> _converters =
            new Dictionary<Type, Func<string, object>>
            {
                // string
                {typeof(string),s=>s },
                // integral types
                {typeof(int),s=>int.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(uint),s=>uint.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(int?),s=>s==null?(int?)null:int.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(uint?),s=>s==null?(uint?)null:uint.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(short),s=>short.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(ushort),s=>ushort.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(short?),s=>s==null?(int?)null:short.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(ushort?),s=>s==null?(uint?)null:ushort.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(byte),s=>byte.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(sbyte),s=>sbyte.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(byte?),s=>s==null?(byte?)null:byte.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(sbyte?),s=>s==null?(sbyte?)null:sbyte.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(long),s=>long.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(ulong),s=>ulong.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(long?),s=>s==null?(long?)null:long.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(ulong?),s=>s==null?(ulong?)null:ulong.Parse(s,CultureInfo.InvariantCulture) },
                // floating point
                {typeof(float),s=>float.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(float?),s=>s==null?(float?)null:float.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(double),s=>double.Parse(s,CultureInfo.InvariantCulture) },
                {typeof(double?),s=>s==null?(double?)null:double.Parse(s,CultureInfo.InvariantCulture) },
                // date time
                {typeof(DateTime), _=>throw new NotSupportedException("Date and time types not supported in parameters")},
                {typeof(DateTimeOffset), _=>throw new NotSupportedException("Date and time types not supported in parameters")},
                {typeof(TimeSpan), _=>throw new NotSupportedException("Date and time types not supported in parameters")},
                // bool
                {typeof(bool), s=>(string.Compare(s,"true",StringComparison.OrdinalIgnoreCase)==0||string.Compare(s,"yes",StringComparison.OrdinalIgnoreCase)==0||s=="1")?true:
                                  (string.Compare(s,"false",StringComparison.OrdinalIgnoreCase)==0||string.Compare(s,"no",StringComparison.OrdinalIgnoreCase)==0||s=="0")?false:
                                  throw new FormatException($"s is not bool")}
            };
        public static object ConvertTo(Type targetType, string stringValue)
        {
            _converters.TryGetValue(targetType, out var converter);
            try
            {
                return converter(stringValue);
            }
            catch (ArgumentNullException)
            {
                throw new BadRequestException($"missing paramter");
            }
            catch (FormatException)
            {
                throw new BadRequestException($"value \"{stringValue}\" can't be convetetd to {targetType}");
            }
        }

    }
}

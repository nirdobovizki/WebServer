using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NirDobovizki.WebServer.Advanced
{
    public interface IJsonSerializer
    {
        string Serialize(object value);
        object Deserialize(string json, Type targetType);
    }
}

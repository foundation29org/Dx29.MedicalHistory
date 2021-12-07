using System;
using System.Text;
using System.Security.Cryptography;

using Newtonsoft.Json;

namespace Dx29
{
    static public class ObjectExtensions
    {
        static public string Serialize(this object obj, bool indented = true)
        {
            return JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
        }

        static public T Deserialize<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        static public string AsString(this byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        static public Int32 AsInt32(this string str)
        {
            Int32.TryParse(str, out int value);
            return value;
        }

        static public Int64 AsInt64(this string str)
        {
            Int64.TryParse(str, out long value);
            return value;
        }

        static public double AsDouble(this string str)
        {
            Double.TryParse(str, out double result);
            return result;
        }

        static public bool EqualsNoCase(this string strA, string strB)
        {
            return String.Equals(strA, strB, StringComparison.OrdinalIgnoreCase);
        }

        static public string HashString(this string str)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(str));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}

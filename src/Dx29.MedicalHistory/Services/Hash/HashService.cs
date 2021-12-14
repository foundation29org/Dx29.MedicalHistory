using System;
using System.Text;
using System.Security.Cryptography;

namespace Dx29.Services
{
    abstract public class HashService
    {
        public HashService(string secret, int iterations, int size)
        {
            Salt = Encoding.UTF8.GetBytes(secret);
            Iterations = iterations;
            Size = size;
        }

        public byte[] Salt { get; }
        public int Iterations { get; }
        public int Size { get; }

        public string GetHash(string value, string prefix)
        {
            value = value.ToLower();
            using (var rfc = new Rfc2898DeriveBytes(value, Salt, Iterations))
            {
                var hash = rfc.GetBytes(Size);
                var arrayString = ByteArrayToString(hash);
                return prefix == null ? arrayString : $"{prefix}-{arrayString}";
            }
        }

        static private string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "").ToLower();
        }
    }
}

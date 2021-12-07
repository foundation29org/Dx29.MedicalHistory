using System;

namespace Dx29.Services
{
    public class RecordHashService : HashService
    {
        public RecordHashService(string secret, int iterations, int size) : base(secret, iterations, size)
        {
        }

        public string GetHash(string value)
        {
            return base.GetHash(value, null);
        }
    }
}

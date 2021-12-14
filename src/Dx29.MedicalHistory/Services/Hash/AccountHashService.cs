using System;

namespace Dx29.Services
{
    public class AccountHashService : HashService
    {
        public AccountHashService(string secret, int iterations, int size) : base(secret, iterations, size)
        {
        }

        public string GetHash(string value)
        {
            return base.GetHash(value, "acc");
        }
    }
}

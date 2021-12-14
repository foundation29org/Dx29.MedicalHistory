using System;

namespace Dx29.Data
{
    public enum SharedWithStatus
    {
        Shared,
        Pending,
        Revoked
    }

    public class SharedWith
    {
        public SharedWith()
        {
        }
        public SharedWith(string userId, SharedWithStatus status) : this()
        {
            UserId = userId;
            Status = status.ToString();
            LastUpdate = DateTimeOffset.UtcNow;
        }

        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }
}

using System;

namespace Dx29.Data
{
    public enum SharedByStatus
    {
        Pending,
        Accepted,
        Rejected,
        Revoked
    }

    public class SharedBy
    {
        public SharedBy()
        {
        }
        public SharedBy(string userId, string caseId, SharedByStatus status) : this()
        {
            UserId = userId;
            CaseId = caseId;
            Status = status.ToString();
            LastUpdate = DateTimeOffset.UtcNow;
        }

        public string UserId { get; set; }
        public string CaseId { get; set; }
        public string Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }
}

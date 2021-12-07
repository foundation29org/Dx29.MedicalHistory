using System;
using System.Collections.Generic;

using Dx29.Tools;

namespace Dx29.Data
{
    #region Gender
    public enum Gender
    {
        Unknown,
        Male,
        Female
    }
    #endregion

    #region CaseStatus
    public enum CaseStatus
    {
        Private,
        Shared,
        Sharing,
        PendingShared,
        StopShared,
        Deleted
    }
    #endregion

    public class MedicalCase
    {
        public MedicalCase()
        {
            ResourceGroups = new Dictionary<string, ResourceGroupRef>(StringComparer.OrdinalIgnoreCase);
            SharedBy = new List<SharedBy>();
            SharedWith = new List<SharedWith>();
        }
        public MedicalCase(string userId, bool shared = false, IList<string> diseasesIds = null) : this()
        {
            Id = shared ? IDGenerator.GenerateID('s') : IDGenerator.GenerateID('c');
            UserId = userId;
            Status = shared ? CaseStatus.Shared.ToString() : CaseStatus.Private.ToString();
            CreatedOn = DateTimeOffset.UtcNow;
            UpdatedOn = CreatedOn;
            PatientInfo = new PatientInfo
            {
                DiseasesIds = diseasesIds ?? new List<string>()
            };
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }

        public PatientInfo PatientInfo { get; set; }

        public IDictionary<string, ResourceGroupRef> ResourceGroups { get; set; }

        public IList<SharedBy> SharedBy { get; set; }
        public IList<SharedWith> SharedWith { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }

        public override string ToString()
        {
            return $"{UserId}-{Id}";
        }
    }

    public class PatientInfo
    {
        public PatientInfo()
        {
            DiseasesIds = new List<string>();
        }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public IList<string> DiseasesIds { get; set; }
    }
}

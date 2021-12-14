using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    #region CaseRecordType
    public enum CaseRecordType
    {
        Generic,
        Patient,
        Share
    }
    #endregion

    public class CaseRecord
    {
        public CaseRecord()
        {
        }
        public CaseRecord(string userId, string recordId, CaseRecordType type, IDictionary<string, string> properties)
        {
            Id = recordId;
            UserId = userId;
            Type = type.ToString();
            Properties = properties;
        }

        public string Id { get; set; }

        public string UserId { get; set; }

        public string Type { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public string GetProperty(string key)
        {
            if (Properties.TryGetValue(key, out string value))
            {
                return value;
            }
            return null;
        }

        public Int32? GetPropertyInt32(string key)
        {
            var value = GetProperty(key);
            if (value != null)
            {
                if (Int32.TryParse(value, out Int32 res))
                {
                    return res;
                }
            }
            return null;
        }

        public Int64? GetPropertyInt64(string key)
        {
            var value = GetProperty(key);
            if (value != null)
            {
                if (Int64.TryParse(value, out Int64 res))
                {
                    return res;
                }
            }
            return null;
        }

        public DateTimeOffset? GetPropertyDateTime(string key)
        {
            var value = GetProperty(key);
            if (value != null)
            {
                if (DateTimeOffset.TryParse(value, out DateTimeOffset date))
                {
                    return date;
                }
            }
            return null;
        }
    }

    static public class CaseRecordExtensions
    {
        static public PatientInfo AsPatientInfo(this CaseRecord caseRecord)
        {
            if (caseRecord != null)
            {
                return new PatientInfo
                {
                    Name = caseRecord.GetProperty("name"),
                    BirthDate = caseRecord.GetPropertyDateTime("birthDate"),
                    Gender = caseRecord.GetProperty("gender"),
                    DiseasesIds = caseRecord.GetProperty("diseasesIds")?.Split(',') ?? new List<string>().ToArray()
                };
            }
            return null;
        }
    }
}

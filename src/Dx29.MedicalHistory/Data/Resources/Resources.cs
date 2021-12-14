using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public enum TermStatus
    {
        Undefined,
        Selected,
        Unselected
    }

    public class ResourceSymptom : Resource
    {
        public ResourceSymptom() { }
        public ResourceSymptom(string hpo, TermStatus status = TermStatus.Undefined) : base(hpo.ToLower(), hpo)
        {
            Status = status.ToString();
        }
    }

    public class ResourceReport : Resource
    {
        public ResourceReport() { }
        public ResourceReport(string id, string name, long size) : base(id, name)
        {
            Properties["Size"] = size.ToString();
        }
    }

    public class ResourceAnalysis : Resource
    {
        public ResourceAnalysis() { }
        public ResourceAnalysis(string id, IList<string> symptoms, IList<string> diseases, string genotypeId) : base(id, id)
        {
            Properties.Add("symptoms", String.Join(',', symptoms).ToLower());
            Properties.Add("diseases", String.Join(',', diseases).ToLower());
            Properties.Add("genotype", genotypeId);
        }

        public IList<string> GetSymptoms() => Properties["symptoms"].Split(',');
        public IList<string> GetDiseases() => Properties["diseases"].Split(',');
    }

    public class ResourceDiagnosis : Resource
    {
        public ResourceDiagnosis() { }
        public ResourceDiagnosis(string id, string name) : base(id, name)
        {
        }
    }

    public class ResourceNote : Resource
    {
        public ResourceNote() { }
        public ResourceNote(string id, string name) : base(id, name)
        {
        }
    }
}

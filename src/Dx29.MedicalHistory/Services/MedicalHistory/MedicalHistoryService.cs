using System;

using Dx29.Data;

namespace Dx29.Services
{
    public partial class MedicalHistoryService
    {
        public MedicalHistoryService(MedicalCaseService medicalCaseService, ResourceGroupService resourceGroupService)
        {
            MedicalCaseService = medicalCaseService;
            ResourceGroupService = resourceGroupService;
        }

        public MedicalCaseService MedicalCaseService { get; }
        public ResourceGroupService ResourceGroupService { get; }

        static public ResourceGroupType ParseResourceGroupType(string typeName)
        {
            return Enum.Parse<ResourceGroupType>(typeName);
        }

        static public ResourceGroupType? TryParseResourceGroupType(string typeName)
        {
            if (typeName != null)
            {
                return Enum.Parse<ResourceGroupType>(typeName);
            }
            return null;
        }
    }
}

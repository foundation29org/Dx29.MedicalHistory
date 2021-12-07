using System;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Services;

namespace Sample
{
    partial class UseCases
    {
        private static async Task<MedicalCase> CreateMedicalCaseAsync(MedicalHistoryService svc, string userId)
        {
            await Task.Delay(1000);
            var medicalCase = await svc.CreateMedicalCaseAsync(userId, new PatientInfo { Name = "Sample" });

            await Task.Delay(1000);

            await svc.CreateResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Reports, "Medical");
            await svc.CreateResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Reports, "Genetic");

            await svc.CreateResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Phenotype, "Manual");
            await svc.CreateResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Genotype, "Manual");
            await svc.CreateResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Notes, "Notes");

            await svc.CreateResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.TimeLine, "TimeLine");

            medicalCase = await svc.GetMedicalCaseAsync(medicalCase.UserId, medicalCase.Id);

            return medicalCase;
        }
    }
}

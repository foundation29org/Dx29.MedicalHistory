using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29;
using Dx29.Data;
using Dx29.Services;

namespace Sample
{
    static partial class UseCases
    {
        static public async Task RunAsync(MedicalHistoryService svc, string userId)
        {
            var medicalCase = await CreateMedicalCaseAsync(svc, userId);
            var resourceGroup = await AddManualSymptomsAsync(svc, medicalCase);

            await AddReportAsync(svc, medicalCase);
            await ProcessReportsAsync(svc, medicalCase);

            await UpdateSymptomsAsync(svc, resourceGroup);
            await DeleteSymptomsAsync(svc, resourceGroup);

            await AddTimeLinesAsync(svc, medicalCase);

            var caseSymptoms = await GetAllSymptomsAsync(svc, medicalCase);
            Console.WriteLine(caseSymptoms.Serialize());

            var caseSymptomGroups = await GetAllSymptomGroupssAsync(svc, medicalCase);
            Console.WriteLine(caseSymptomGroups.Serialize());

            var caseMedicalReports = await GetAllMedicalReportsAsync(svc, medicalCase);
            Console.WriteLine(caseMedicalReports.Serialize());

            var caseTimeLines = await GetAllTimeLinesAsync(svc, medicalCase);
            Console.WriteLine(caseTimeLines.Serialize());
        }

        private static Resource CreateResource((string, string, string) item)
        {
            return new Resource(item.Item1, item.Item2) { Status = item.Item3 };
        }

        private static IEnumerable<Resource> CreateResources(params (string, string, string)[] items)
        {
            foreach (var item in items)
            {
                yield return new Resource(item.Item1, item.Item2) { Status = item.Item3 };
            }
        }
    }
}

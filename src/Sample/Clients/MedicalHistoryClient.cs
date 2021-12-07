using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dx29.Web.Services
{
    public partial class MedicalHistoryClient
    {
        public MedicalHistoryClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync($"About/version");
        }
    }
}

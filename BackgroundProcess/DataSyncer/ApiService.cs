using DataSyncer.Entities;
using DataSyncer.Interfaces;
using System.Threading.Tasks;

namespace DataSyncer
{
    public class ApiService : IApiService
    {
        private IHttpService _httpService;
        public AuthenticationResponse UserToken { get; set; }
        public ApiService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<AuthenticationResponse> Login()
        {
            AuthenticationRequest request = new AuthenticationRequest();
            UserToken = await _httpService.Post<AuthenticationResponse>("/token", request);
            return UserToken;
        }

        public async Task SyncData(BillHeader billHeader)
        {
            await _httpService.Post("/invoices", billHeader);
        }
    }
}

using DataSyncer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSyncer.Interfaces
{
    public interface IApiService
    {
        AuthenticationResponse UserToken { get; }
        Task<AuthenticationResponse> Login();
        Task SyncData(BillHeader billHeader);
    }
}

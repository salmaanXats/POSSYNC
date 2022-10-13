using System.Configuration;

namespace DataSyncer.Entities
{
    public class AuthenticationRequest
    {
        public string grant_type { get; set; } = "client_credentials";
        public string tenant_id { get; set; } = ConfigurationManager.AppSettings["loginTenantId"];
        public string tenant_secret { get; set; } = ConfigurationManager.AppSettings["loginTenantSecret"];
    }
}

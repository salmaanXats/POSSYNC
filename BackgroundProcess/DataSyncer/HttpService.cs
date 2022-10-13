using DataSyncer.Helpers;
using DataSyncer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace DataSyncer
{
    public class HttpService : IHttpService
    {
        private static string _apiBaseUrl = "https://trms-api.azurewebsites.net/api";
        private HttpClient _client;

        public HttpService()
        {
            _client = new HttpClient();
        }

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format("{0}{1}", _apiBaseUrl, uri));
            return await SendRequest<T>(request);
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, string.Format("{0}{1}", _apiBaseUrl, uri), value);
            return await SendRequest<T>(request);
        }

        public async Task Post(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, string.Format("{0}{1}", _apiBaseUrl, uri), value);
            await SendRequest(request);
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string uri, object value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
                request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return request;
        }

        private async Task SendRequest(HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            // send request
            var response = await _client.SendAsync(request);

            
            await HandleErrors(response);
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            HttpResponseMessage response;

            try
            {
                response = await _client.SendAsync(request);

            }
            catch (Exception ex)
            {

                throw;
            }
            
            

            await HandleErrors(response);

            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new StringConverter());
            return await response.Content.ReadFromJsonAsync<T>(options);
        }

        private async Task AddJwtHeader(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization", $"Bearer {Program.AuthResponse?.access_token}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Program.AuthResponse?.access_token);
        }

        private async Task HandleErrors(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = response.ReasonPhrase;
                throw new Exception(error);
            }
        }
    }
}

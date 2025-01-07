using MachineTracking.Domain.Interfaces.Client;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MachineTracking.Client.Shared.Helpers
{
    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient _httpClient;
        public HttpClientProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> RequestPost(string url, object data)
        {
            try
            {
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var jsonData = JsonSerializer.Serialize(data); 
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {

                if (ex.StatusCode.HasValue)
                {
                    var errorResponse = new HttpResponseMessage(ex.StatusCode.Value)
                    {
                        ReasonPhrase = ex.Message
                    };
                    return errorResponse;
                }


                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = "An error occurred while processing the request."
                };
            }
        }

        public async Task<HttpResponseMessage> RequestGetByObject(string url, object data)
        {
            try
            {
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var properties = data.GetType().GetProperties();
                var query = string.Join("&", properties
                    .Where(p => p.GetValue(data) is not null && p.GetValue(data)?.ToString() != string.Empty)
                    .Select(p => $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.GetValue(data)?.ToString() ?? string.Empty)}")); 
                
                var fullUrl = $"{url}?{query}";

                var response = await _httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GET request: {ex.Message}");
            }
        }

        public async Task<HttpResponseMessage> RequestGet(string url)
        {
            try
            {
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> RequestGetById(string url, int id)
        {
            try
            {
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                url = $"{url}/?id={id}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> RequestGetByStringId(string url, string id)
        {
            try
            {
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                url = $"{url}/?id={id}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

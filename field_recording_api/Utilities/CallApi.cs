using Azure;
using field_recording_api.Models.HttpModel;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;

namespace field_recording_api.Utilities
{
    public static class CallApi
    {
        public static async Task<ResponseContext> get(string Endpoint, HttpOption httpOption = null)
        {
            var _resview = new ResponseContext();
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                var httpClient = new HttpClient(clientHandler);
                httpClient = setHttpOption(httpClient, httpOption);

                var response = await httpClient.GetAsync(Endpoint);
                if (!response.IsSuccessStatusCode) // Check the status
                {
                    _resview.statusCode = ((int)response.StatusCode).ToString();
                    _resview.message = response.StatusCode.ToString();
                    return _resview;
                }

                var apiResponse = await response.Content.ReadAsStringAsync(); 
                _resview.data = apiResponse;
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("CallApi.Get Endpoint {0} Error: {1}", Endpoint, ex.Message);
                throw new Exception(ex.Message);
            }

            return _resview;
        }

        public static async Task<ResponseContext> post(string Endpoint, object body, HttpOption httpOption = null)
        {
            var _resview = new ResponseContext();
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                var jsonObject = JsonSerializer.Serialize(body);
                StringContent httpContent = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json");
                var httpClient = new HttpClient(clientHandler);
                httpClient = setHttpOption(httpClient, httpOption);

                var response = await httpClient.PostAsync(Endpoint, httpContent);
                if (!response.IsSuccessStatusCode) // Check the status
                {
                    _resview.statusCode = ((int)response.StatusCode).ToString();
                    _resview.message = response.StatusCode.ToString();
                    return _resview;
                }

                var apiResponse = await response.Content.ReadAsStringAsync();
                _resview.data = apiResponse;
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("CallApi.Post Endpoint {0} Error: {1}", Endpoint, ex.Message);
                throw new Exception(ex.Message);
            }

            return _resview;
        }

        private static HttpClient setHttpOption(HttpClient httpClient, HttpOption httpOption = null) {
            if (httpOption != null && httpOption.token != null)
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpOption.token);

            if (httpOption != null && httpOption.timeouts != null) {
                TimeSpan ts = TimeSpan.FromSeconds(long.Parse(httpOption.timeouts) * 60);
                httpClient.Timeout = ts;
            }
                
            return httpClient;
        }
    }
}

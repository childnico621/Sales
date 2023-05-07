using Newtonsoft.Json;
using Sales.API.Intefaces;
using Sales.Shared.Responses;

namespace Sales.API.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Response<List<T>>> GetListAsync<T>(string servicePrefix, string controller)
        {
            HttpClient client = _httpClientFactory.CreateClient("CoutriesAPI");

            string url = $"{servicePrefix}{controller}";
            var response = await client.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new Response<List<T>>
                {
                    IsSuccess = false,
                    ErrorMessage = result,
                };
            }            List<T> list = JsonConvert.DeserializeObject<List<T>>(result)!;
            return new Response<List<T>>
            {
                IsSuccess = true,
                Result = list
            };
        }
    }
}

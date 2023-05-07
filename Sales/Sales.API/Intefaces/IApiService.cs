using Sales.Shared.Responses;

namespace Sales.API.Intefaces
{
    public interface IApiService
    {
        Task<Response<List<T>>> GetListAsync<T>(string servicePrefix, string controller);
    }
}
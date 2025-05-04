namespace Shared.Interfaces;

public interface IApiClient
{
    Task<ServiceResult<ContextOverview>> GetOverviewAsync();
    Task<ServiceResult<string>> CryptoAsync(CryptoRequest request);
    Task<ServiceResult<TResponse>> Get<TResponse>(string uriSuffix, params KeyValuePair<string, string>[] headers);

    Task<ServiceResult<TResponse>> Post<TResponse>(string uriSuffix, params KeyValuePair<string, string>[] headers);
    Task<ServiceResult> Post(string uriSuffix, params KeyValuePair<string, string>[] headers);
    Task<ServiceResult> Post<TRequest>(string uriSuffix, TRequest requestData, params KeyValuePair<string, string>[] headers);
    Task<ServiceResult<TResponse>> Post<TRequest, TResponse>(string uriSuffix, TRequest requestData, params KeyValuePair<string, string>[] headers);

    Task<ServiceResult<TResponse>> Put<TResponse>(string uriSuffix, params KeyValuePair<string, string>[] headers);
    Task<ServiceResult> Put(string uriSuffix, params KeyValuePair<string, string>[] headers);
    Task<ServiceResult> Put<TRequest>(string uriSuffix, TRequest requestData, params KeyValuePair<string, string>[] headers);
    Task<ServiceResult<TResponse>> Put<TRequest, TResponse>(string uriSuffix, TRequest requestData, params KeyValuePair<string, string>[] headers);
}

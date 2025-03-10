﻿using FrontBlazorFluentUI.Model;
using FrontBlazorFluentUI.Options;
using FrontBlazorFluentUI;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FrontBlazorFluentUI.Dto;

namespace FrontBlazorFluentUI.Services;

public sealed class ApiClient
{
    private readonly ILogger? _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ApiClientConfig _config;
    private readonly IStorageProvider _storage;

    public ApiClient(ILogger<ApiClient>? logger, HttpClient httpClient, ApiClientConfig config, JsonSerializerOptions jsonOptions, IStorageProvider storage)
    {
        _logger = logger;
        _httpClient = httpClient;
        _config = config;
        _jsonOptions = jsonOptions;
        _storage = storage;
    }
    public string BaseAddress => _config.BaseAddress;

    public async Task<ServiceResult<ContextOverview>> GetOverviewAsync()
    {
        var serviceResult = await Get<ContextOverview>("/info");
        serviceResult.Value?.SetReceivedTime();
        return serviceResult;
    }

    private async Task<HttpRequestMessage> PrepareRequest(HttpMethod method, string uriSuffix, params KeyValuePair<string, string>[] headers)
        => await PrepareRequest(method, uriSuffix, content: default, headers);

    private async Task<HttpRequestMessage> PrepareRequest(HttpMethod method, string uriSuffix, HttpContent? content, params KeyValuePair<string, string>[] headers)
    {
        var token = await _storage.GetAccessTokenAsync();
        var request = new HttpRequestMessage(method, $"{BaseAddress}{uriSuffix}");
        request.Content = content;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (headers.Length == 0)
            return request;

        foreach (var header in headers)
            request.Headers.Add(header.Key, header.Value);

        return request;
    }

    public async Task<ServiceResult<TResponse>> Get<TResponse>(string uriSuffix, params KeyValuePair<string, string>[] headers)
    {
        var serviceResult = new ServiceResult<TResponse>();
        try
        {
            var request = await PrepareRequest(HttpMethod.Get, uriSuffix, headers);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return await GenerateErrorResult(serviceResult, response);
            }

            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            if (result is null)
                return serviceResult.NoContent();

            return serviceResult.Success(result);

        }
        catch (Exception ex)
        {
            return GenerateErrorResult(serviceResult, ex);
        }
    }

    public async Task<ServiceResult<TResponse>> Post<TResponse>(string uriSuffix, params KeyValuePair<string, string>[] headers)
    {
        var serviceResult = new ServiceResult<TResponse>();
        try
        {
            var request = await PrepareRequest(HttpMethod.Post, uriSuffix, headers);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return await GenerateErrorResult(serviceResult, response);
            }

            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            if (result is null)
                return serviceResult.NoContent();

            return serviceResult.Success(result);

        }
        catch (Exception ex)
        {
            return GenerateErrorResult(serviceResult, ex);
        }
    }

    public async Task<ServiceResult> Post(string uriSuffix, params KeyValuePair<string, string>[] headers)
    {
        try
        {
            var request = await PrepareRequest(HttpMethod.Post, uriSuffix, headers);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return await GenerateErrorResult(response);
            }

            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            return GenerateErrorResult(ex);
        }
    }

    public async Task<ServiceResult> Post<TRequest>(string uriSuffix, TRequest requestData, params KeyValuePair<string, string>[] headers)
    {
        try
        {
            var content = JsonContent.Create(requestData, mediaType: null, _jsonOptions);
            var request = await PrepareRequest(HttpMethod.Post, uriSuffix, content, headers);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return await GenerateErrorResult(response);
            }

            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            return GenerateErrorResult(ex);
        }
    }

    public async Task<ServiceResult<TResponse>> Post<TRequest, TResponse>(string uriSuffix, TRequest requestData, params KeyValuePair<string, string>[] headers)
    {
        var serviceResult = new ServiceResult<TResponse>();
        try
        {
            var content = JsonContent.Create(requestData, mediaType: null, _jsonOptions);
            var request = await PrepareRequest(HttpMethod.Post, uriSuffix, content, headers);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return await GenerateErrorResult(serviceResult, response);
            }

            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            if (result is null)
                return serviceResult.NoContent();

            return serviceResult.Success(result);

        }
        catch (Exception ex)
        {
            return GenerateErrorResult(serviceResult, ex);
        }
    }



    private static async Task<string> GetErrorMessage(HttpResponseMessage response)
    {
        if (response.Content.Headers.ContentType?.MediaType == "application/json")
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (error.TrackingId != null)
                return $"{error.Message} [tracking: {error.TrackingId}]";

            return error.Message;
        }

        return await response.Content.ReadAsStringAsync();
    }

    private static async Task<ServiceResult<T>> GenerateErrorResult<T>(ServiceResult<T> serviceResult, HttpResponseMessage response)
    {
        return GenerateErrorResult(serviceResult, response.StatusCode, await GetErrorMessage(response));
    }
    private static async Task<ServiceResult> GenerateErrorResult(HttpResponseMessage response)
    {
        return GenerateErrorResult(response.StatusCode, await GetErrorMessage(response));
    }

    private static ServiceResult GenerateErrorResult(Exception ex)
    {
        if (ex is HttpRequestException httpRequestException)
        {
            return GenerateErrorResult(httpRequestException.StatusCode, httpRequestException.Message);
        }

        return ServiceResult.ServiceUnavailable(ex.Message);
    }

    private static ServiceResult<T> GenerateErrorResult<T>(ServiceResult<T> serviceResult, Exception ex)
    {
        if (ex is HttpRequestException httpRequestException)
        {
            return GenerateErrorResult(serviceResult, httpRequestException.StatusCode, httpRequestException.Message);
        }

        return serviceResult.ServiceUnavailable(ex.Message);
    }

    private static ServiceResult<T> GenerateErrorResult<T>(ServiceResult<T> serviceResult, HttpStatusCode? statusCode, string message)
    {
        if (statusCode is null)
            return serviceResult.ServiceUnavailable($"Server is not accesible ({message})");

        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
                return serviceResult.Unauthorized(message);
            case HttpStatusCode.NoContent:
                return serviceResult.NoContent(message);
            case HttpStatusCode.BadRequest:
                return serviceResult.BadRequest(message);
            case HttpStatusCode.Forbidden:
                return serviceResult.Forbidden(message);
            case HttpStatusCode.NotFound:
                return serviceResult.NotFound(message);
            case HttpStatusCode.NotImplemented:
                return serviceResult.NotImplemented(message);
            case HttpStatusCode.ServiceUnavailable:
            default:
                return serviceResult.ServiceUnavailable(message);
        }
    }

    private static ServiceResult GenerateErrorResult(HttpStatusCode? statusCode, string message)
    {
        if (statusCode is null)
            return ServiceResult.ServiceUnavailable(message);

        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
                return ServiceResult.Unauthorized(message);
            case HttpStatusCode.NoContent:
                return ServiceResult.NoContent(message);
            case HttpStatusCode.BadRequest:
                return ServiceResult.BadRequest(message);
            case HttpStatusCode.Forbidden:
                return ServiceResult.Forbidden(message);
            case HttpStatusCode.NotFound:
                return ServiceResult.NotFound(message);
            case HttpStatusCode.NotImplemented:
                return ServiceResult.NotImplemented(message);
            case HttpStatusCode.ServiceUnavailable:
            default:
                return ServiceResult.ServiceUnavailable(message);
        }
    }


}

//public class ApiService : ApiServiceBase
//{
//    public ApiService(ILogger<ApiService>? logger, HttpClient httpClient, ApiServiceConfig config, JsonSerializerOptions jsonOptions)
//        : base(logger, httpClient, config, jsonOptions) { }

//    internal async Task<ServiceResult> SignIn(SignInDto request)
//    {
//        try
//        {
//            // Call the login endpoint. The server will return a success status
//            // and set HttpOnly cookies with the tokens.
//            var response = await _httpClient.PostAsJsonAsync("/auth/sign-in", request);
//            if (response.IsSuccessStatusCode)
//                return ServiceResult.Success();

//            return ServiceResult.Unauthorized("Invalid credentials. Please try again.");
//        }
//        catch
//        {
//            return ServiceResult.InternalError(Guid.Empty, "Service is not available");
//        }
//    }

//    internal async Task<ServiceResult> SignOut()
//    {
//        try
//        {
//            var response = await _httpClient.PostAsync("/logout", null);
//            if (response.IsSuccessStatusCode)
//                return ServiceResult.Success();

//            return ServiceResult.Unauthorized("Invalid credentials. Please try again.");
//        }
//        catch
//        {
//            return ServiceResult.InternalError(Guid.Empty, "Service is not available");
//        }
//    }
//}

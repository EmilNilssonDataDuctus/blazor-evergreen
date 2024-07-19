using BlazorAppEvergreenOIDC;
using BlazorAppEvergreenOIDC.Data;
using BlazorAppEvergreenOIDC.Data.Interfaces;
using BlazorAppEvergreenOIDC.Models.ScimModels;
using BlazorAppEvergreenOIDC.Models.ScimModels.Responses;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class ScimUserService : IScimUserService
{
    private readonly HttpClient http;
    private readonly TokenProvider tokenProvider;
    private readonly FileLogger fileLogger;
    private readonly IConfiguration configuration;

    public ScimUserService(IHttpClientFactory clientFactory,
        TokenProvider tokenProvider, FileLogger fileLogger, IConfiguration configuration)
    {
        http = clientFactory.CreateClient();
        this.tokenProvider = tokenProvider;
        this.fileLogger = fileLogger;
        this.configuration = configuration;
    }

    public async Task<ResponseGetUsers> GetUsersAsync(int count, int startIndex)
    {
        var bearerToken = tokenProvider.AccessToken;
        var request = new HttpRequestMessage(HttpMethod.Get,
            this.configuration.GetValue<string>("OpenIDConnect:ScimEndpoint") +
            $"/Users" +
            $"?count={count}&startIndex={startIndex}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = JsonSerializer.Deserialize<ResponseGetUsers>(json, options);
        if (result == null || result.Resources == null)
        {
            throw new Exception("Failed to deserialize JSON response from API endpoint.");
        }
        fileLogger.SaveLogToFile(request.ToString() + Environment.NewLine + response.ToString());
        return result;
    }

    public async Task<ResponseGetUsers> GetUsersAsyncWithSearchQuery(int count, int startIndex, string searchQueryUserName)
    {
        var bearerToken = tokenProvider.AccessToken;
        var request = new HttpRequestMessage(HttpMethod.Get,
            this.configuration.GetValue<string>("OpenIDConnect:ScimEndpoint") +
            $"/Users" +
            $"?count={count}&startIndex={startIndex}" +
            $"&filter=" +
            $"userName co %22{searchQueryUserName}%22"
            //+ $"or name.GivenName co %22{searchQueryFirstName}%22"
            //+ $"or name.FamilyName co %22{searchQueryLastName}%22"
            );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ResponseGetUsers>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        if (result == null || result.Resources == null)
        {
            throw new Exception("Failed to deserialize JSON response from API endpoint.");
        }

        fileLogger.SaveLogToFile(request.ToString() + Environment.NewLine + response.ToString());
        return result;
    }

    public async Task<ResponseGetUsers> GetUserByUsernameAsync(string username)
    {
        if (!string.IsNullOrEmpty(username))
        {
            var bearerToken = tokenProvider.AccessToken;
            var request = new HttpRequestMessage(HttpMethod.Get,
                this.configuration.GetValue<string>("OpenIDConnect:ScimEndpoint") +
                "/Users" +
                "?filter=" +
                $"userName eq %22{username}%22");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResponseGetUsers>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (result == null || result.Resources == null)
            {
                throw new Exception("Failed to deserialize JSON response from API endpoint.");
            }

            fileLogger.SaveLogToFile(request.ToString() + Environment.NewLine + response.ToString());
            return result;
        }
        throw new Exception("Username not provided in IdToken during API call to GetUserByUsernameAsync.");
    }

    public async Task<ResponseGetUserById> GetUserByIdAsync(string userId)
    {
        var bearerToken = tokenProvider.AccessToken;
        var request = new HttpRequestMessage(HttpMethod.Get,
            this.configuration.GetValue<string>("OpenIDConnect:ScimEndpoint") +
            $"/Users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ResponseGetUserById>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        if (result == null)
        {
            throw new Exception("Failed to deserialize JSON response from API endpoint.");
        }

        fileLogger.SaveLogToFile(request.ToString() + Environment.NewLine + response.ToString());
        return result;
    }

    public async Task<ResponseGetDevices> GetDevicesAsync(int count, int startIndex)
    {
        var bearerToken = tokenProvider.AccessToken;
        var request = new HttpRequestMessage(HttpMethod.Get,
            this.configuration.GetValue<string>("OpenIDConnect:ScimEndpoint") +
            $"/Devices?count={count}&startIndex={startIndex}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ResponseGetDevices>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        if (result == null || result.Resources == null)
        {
            throw new Exception("Failed to deserialize JSON response from API endpoint.");
        }

        fileLogger.SaveLogToFile(request.ToString() + Environment.NewLine + response.ToString());
        return result;
    }

    public async Task DeleteDeviceAsync(string deviceId)
    {
        var token = tokenProvider.AccessToken;
        var request = new HttpRequestMessage(HttpMethod.Delete,
            this.configuration.GetValue<string>("OpenIDConnect:ScimEndpoint") + $"/Devices/{deviceId}");
        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        fileLogger.SaveLogToFile(request.ToString() + Environment.NewLine + response.ToString());
    }
}

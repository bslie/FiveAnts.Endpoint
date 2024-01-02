using Newtonsoft.Json;

namespace Endpoint.NET;

using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Exceptions;

public class Endpoint<T> : IDisposable
{
    public bool KeepAlive { get; }
    private readonly UriBuilder _uriBuilder;
    private readonly HttpClient _httpClient;
    private readonly SocketsHttpHandler _handler;

    public Endpoint(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));

        KeepAlive = false;

        try
        {
            _uriBuilder = new UriBuilder(baseUrl);
        }
        catch (UriFormatException)
        {
            throw new UriFormatException("Base URL is not a valid URL.");
        }

        _handler = new SocketsHttpHandler();

        if (KeepAlive) _handler.PooledConnectionLifetime = TimeSpan.FromMinutes(2);

        _httpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    public Endpoint(string baseUrl, string username, string password)
        : this(baseUrl)
    {
        var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
    }

    public Endpoint(string baseUrl, string bearerToken)
        : this(baseUrl)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
    }

    public Endpoint<T> AppendPathSegment<TSegment>(TSegment segment)
    {
        if (segment == null) throw new ArgumentNullException(nameof(segment), "Value cannot be null.");

        _uriBuilder.Path = _uriBuilder.Path.TrimEnd('/') + "/" + segment;
        return this;
    }

    public Endpoint<T> SetQueryParam<TName, TValue>(TName name, TValue value)
    {
        if (name == null) return this;
        if (value == null) return this;

        var queryParams = HttpUtility.ParseQueryString(_uriBuilder.Query);
        queryParams[name.ToString()] = value.ToString();
        _uriBuilder.Query = queryParams.ToString();
        return this;
    }

    public async Task<T> GetAsync()
    {
        var url = _uriBuilder.ToString();

        try
        {
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            throw new EndpointException("An error occurred while executing a GET request", ex);
        }
    }

    public async Task<T> PostAsync<TContent>(TContent content)
    {
        var url = _uriBuilder.ToString();
        var serializedContent =
            new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, serializedContent);
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            throw new EndpointException("An error occurred while executing a POST request", ex);
        }
    }

    public async Task<T> PutAsync<TContent>(TContent content)
    {
        var url = _uriBuilder.ToString();
        var serializedContent =
            new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PutAsync(url, serializedContent);
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            throw new EndpointException("An error occurred while executing a PUT request", ex);
        }
    }

    public async Task<T> DeleteAsync()
    {
        var url = _uriBuilder.ToString();

        try
        {
            var response = await _httpClient.DeleteAsync(url);
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            throw new EndpointException("An error occurred while executing a DELETE request", ex);
        }
    }

    public async Task<T> PatchAsync<TContent>(TContent content)
    {
        var url = _uriBuilder.ToString();
        var serializedContent =
            new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = serializedContent };

        try
        {
            var response = await _httpClient.SendAsync(request);
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            throw new EndpointException("An error occurred while executing a PATCH request", ex);
        }
    }


    private static async Task<T> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"The server returned a status code {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<T>(content);
        return result ?? throw new EndpointException("Failed to deserialize the response from the server",
            new InvalidOperationException());
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _handler.Dispose();
        GC.SuppressFinalize(this);
    }
}

using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WEB.Helpers
{
    public class Utils
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        private static readonly HttpClient HttpClient;

        static Utils()
        {
            HttpClientHandler handler = new()
            {
                UseDefaultCredentials = true
            };

            HttpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<T?> CallApiAsync<T>(
            string url,
            HttpMethod method,
            object? body = null,
            string contentType = "application/json",
            string? bearerToken = null,
            CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(method, url);

            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            if (body is not null)
            {
                request.Content = CreateHttpContent(body, contentType);
            }

            if (headers is not null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            using HttpResponseMessage response = await HttpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            string responseContent = response.Content is null
                ? string.Empty
                : await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"API call failed. " +
                    $"Status: {(int)response.StatusCode} ({response.StatusCode}). " +
                    $"URL: {url}. " +
                    $"Response: {responseContent}");
            }

            if (response.StatusCode == HttpStatusCode.NoContent || string.IsNullOrWhiteSpace(responseContent))
            {
                return default;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)responseContent;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(responseContent, JsonOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize API response to {typeof(T).Name}. Response: {responseContent}", ex);
            }
        }

        public static Task<T?> CallApiAsync<T>(
            string url,
            HttpMethod method,
            HttpContext? currentHttpContext,
            object? body = null,
            string contentType = "application/json",
            CancellationToken cancellationToken = default)
        {
            string? bearerToken =
                currentHttpContext?.Session.GetString("JwtToken")
                ?? currentHttpContext?.User.FindFirst("AccessToken")?.Value;

            return CallApiAsync<T>(
                url: url,
                method: method,
                body: body,
                contentType: contentType,
                bearerToken: bearerToken,
                cancellationToken: cancellationToken);
        }

        private static HttpContent CreateHttpContent(object body, string contentType)
        {
            if (body is HttpContent httpContent)
            {
                return httpContent;
            }

            if (body is string rawString)
            {
                return new StringContent(rawString, Encoding.UTF8, contentType);
            }

            string json = JsonSerializer.Serialize(body, JsonOptions);
            return new StringContent(json, Encoding.UTF8, contentType);
        }
    }
}
using Newtonsoft.Json;
using System.Diagnostics;

namespace iris_n2n_launcher.Utils;

internal class IrisServerApi
{
    private static readonly HttpClient _httpClient = new HttpClient()
    {
        BaseAddress = new Uri("https://iris.home56.top/")
    };

    public class ServerInfo
    {
        [JsonProperty("server")]
        public string? Server { get; set; }
        [JsonProperty("port")]
        public int? Port { get; set; }
    }

    public class AppVersionResponse
    {
        public string? Version { get; set; }
    }

    public class ChangelogResponse
    {
        public string? Changelog { get; set; }
    }

    public class NoticeResponse
    {
        public string? Notice { get; set; }
    }

    public static async Task<List<ServerInfo>?> GetServerListAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/servers");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ServerInfo>>(content);
        }
        catch
        {
            return null;
        }
    }
    public static List<ServerInfo>? GetServerList()
    {
        try
        {
            var response = _httpClient.GetAsync("api/servers")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (!response.IsSuccessStatusCode) return null;

            var content = response.Content.ReadAsStringAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return JsonConvert.DeserializeObject<List<ServerInfo>>(content);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<AppVersionResponse?> GetAppVersionAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/version");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AppVersionResponse>(content);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<ChangelogResponse?> GetChangelogAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/changelog");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChangelogResponse>(content);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<NoticeResponse?> GetNoticeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/notice");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<NoticeResponse>(content);
        }
        catch
        {
            return null;
        }
    }
}
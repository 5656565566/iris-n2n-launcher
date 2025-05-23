using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using static iris_n2n_launcher.Utils.FirewallHelper;

namespace iris_n2n_launcher.Utils;

public class MappingInfo
{
    public string listen_addr { get; set; } = string.Empty;
    public string forward_addr { get; set; } = string.Empty;
    public string mapping_type { get; set; } = string.Empty;
}


internal sealed class TcpUdpForw
{
    private static readonly Lazy<TcpUdpForw> _instance = new Lazy<TcpUdpForw>(() => new TcpUdpForw());
    public static TcpUdpForw Instance => _instance.Value;

    private HttpClient? _client;
    private string? _apiBaseUrl;
    private string? _authCode;
    private readonly ExeHelper _exeHelper = ExeHelper.Instance;
    private Process? _process;
    private readonly object _lock = new();

    static readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "N2N");
    static readonly string config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
    static readonly string configFile = Path.Combine(config, "tcpudp.yml");
    static readonly string EXE = Path.Combine(path, "orchid-tcpudp-forw.exe");

    private TcpUdpForw() { }

    public bool IsRun()
    {
        return (_process != null);
    }

    public async void Start()
    {
        await FirewallManager.AllowProgramAsync("n2n-tcpudp-forw", EXE);

        lock (_lock)
        {
            StopProcess();

            int apiPort = PortUtility.GetRandomUnusedPort(5000, 8000);
            _authCode = GenerateRandomString();
            _apiBaseUrl = $"http://127.0.0.1:{apiPort}/api/";
            _client = new HttpClient();

            _process = _exeHelper.CreateProcess(EXE, $"-p {apiPort} -code {_authCode} -config {configFile}");
        }
    }

    public async Task<bool> AddMappingAsync(string listenAddr, string forwardAddr, string mappingType, bool temp)
    {
        EnsureServiceRunning();

        var content = JsonConvert.SerializeObject(temp
            ? new { listenAddr, forwardAddr, mappingType, temp }
            : new { listenAddr, forwardAddr, mappingType });

        var request = new HttpRequestMessage(HttpMethod.Post, _apiBaseUrl + "add")
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", _authCode);

        var response = await _client!.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch {  return false; }
        return true;
    }

    public async Task<bool> DeleteMappingAsync(string listenAddr, string mappingType)
    {
        EnsureServiceRunning();

        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"{_apiBaseUrl}delete?listenAddr={listenAddr}&mappingType={mappingType}"
        );
        request.Headers.Add("Authorization", _authCode);

        var response = await _client!.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch { return false; }
        return true;
    }

    public async Task<List<MappingInfo>> QueryMappingsAsync()
    {
        EnsureServiceRunning();

        var request = new HttpRequestMessage(HttpMethod.Get, _apiBaseUrl + "query");
        request.Headers.Add("Authorization", _authCode);

        try
        {
            var response = await _client!.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json)) return [];

            var mappings = JsonConvert.DeserializeObject<List<MappingInfo>>(json);
            return mappings ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            StopProcess();
            _client?.Dispose();
            _client = null;
            _apiBaseUrl = null;
            _authCode = null;
        }
    }

    private void StopProcess()
    {
        try
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                _process.WaitForExit(1000);
                _process.Dispose();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Stop process failed: {ex.Message}");
        }
        finally
        {
            _process = null;
        }
    }

    private void EnsureServiceRunning()
    {
        if (_process == null || _process.HasExited || _client == null)
        {
            throw new InvalidOperationException("Service is not running. Call Start() first.");
        }
    }

    private static string GenerateRandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringBuilder = new StringBuilder(8);
        var random = new Random();

        for (int i = 0; i < 8; i++)
        {
            stringBuilder.Append(chars[random.Next(chars.Length)]);
        }
        return stringBuilder.ToString();
    }
}
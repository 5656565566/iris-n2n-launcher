using Newtonsoft.Json;
using System.Reflection;

namespace iris_n2n_launcher.Config;

public class Configuration
{
    /// <summary>
    /// 使用的配置文件
    /// </summary>
    public string ConfigName { set; get; } = "default";
    /// <summary>
    /// 广播修复
    /// </summary>
    public bool BroadcastRepair { set; get; } = true;
    /// <summary>
    /// 隔空投送
    /// </summary>
    public bool Airportal { set; get; } = false;
    /// <summary>
    /// 端口映射
    /// </summary>
    public bool TcpUdpForw { set; get; } = false;
    /// <summary>
    /// 开启时关闭防火墙
    /// </summary>
    public bool DisableFirewallOnRun { set; get; } = false;
    /// <summary>
    /// 我的世界房间转发
    /// </summary>
    public bool Minecraft { set; get; } = false;
    /// <summary>
    /// 关闭的时候开启防火墙
    /// </summary>
    public bool FirewallEnabledOnExit { set; get; } = false;
    /// <summary>
    /// 注册iris链接
    /// </summary>
    public bool UrlRegister { set; get; } = false;
    /// <summary>
    /// 更新检查
    /// </summary>
    public bool VersionUpdate { set; get; } = true;
    /// <summary>
    /// 用户自定义服务器列表。
    /// </summary>
    public List<string> UserServerList { get; set; } = [];
    /// <summary>
    /// 在线更新的服务器列表。
    /// </summary>
    public List<string> OnlineServerList { get; set; } = [];
}

public sealed class ConfigManager
{
    private static readonly Lazy<ConfigManager> _instance = new(() => new ConfigManager());

    private readonly string _path;
    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore
    };
    private ConfigManager()
    {
        _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");

        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
    }
    public static ConfigManager Instance => _instance.Value;

    public void SaveConfig<T>(string configName, T configObject)
    {
        if (string.IsNullOrWhiteSpace(configName))
            throw new ArgumentException("Config name cannot be empty", nameof(configName));

        var filePath = GetConfigFilePath(configName);
        string json = JsonConvert.SerializeObject(configObject, _jsonSettings);
        File.WriteAllText(filePath, json);
    }

    public T LoadConfig<T>(string configName) where T : new()
    {
        if (string.IsNullOrWhiteSpace(configName))
            throw new ArgumentException("Config name cannot be empty", nameof(configName));

        var filePath = GetConfigFilePath(configName);

        if (!File.Exists(filePath))
            return new T();

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(json) ?? new T();
    }

    public bool DeleteConfig(string configName)
    {
        if (string.IsNullOrWhiteSpace(configName))
            throw new ArgumentException("Config name cannot be empty", nameof(configName));

        var filePath = GetConfigFilePath(configName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取所有配置列表
    /// </summary>
    public IEnumerable<string?> ListConfigs()
    {
        if (!Directory.Exists(_path))
            return Enumerable.Empty<string>();

        return Directory.GetFiles(_path, "*.json")
                       .Select(Path.GetFileNameWithoutExtension);
    }

    /// <summary>
    /// 检查配置是否存在
    /// </summary>
    public bool ConfigExists(string configName)
    {
        if (string.IsNullOrWhiteSpace(configName))
            return false;

        var filePath = GetConfigFilePath(configName);
        return File.Exists(filePath);
    }

    /// <summary>
    /// 将对象序列化为JSON字符串
    /// </summary>
    public string Serialize<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, _jsonSettings);
    }

    /// <summary>
    /// 将JSON字符串反序列化为对象
    /// </summary>
    public static T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json)!;
    }

    private string GetConfigFilePath(string configName)
    {
        // 转换一下文件名称
        var safeName = string.Join("_", configName.Split(Path.GetInvalidFileNameChars()));
        return Path.Combine(_path, $"{safeName}.json");
    }

    /// <summary>
    /// 检查两个对象是否有差异
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="original">原始对象</param>
    /// <param name="modified">修改后的对象</param>
    /// <returns>是否有差异</returns>
    public bool HasChanges<T>(T original, T modified)
    {
        if (ReferenceEquals(original, null) || ReferenceEquals(modified, null))
            return !ReferenceEquals(original, modified);

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.GetIndexParameters().Length > 0)
                continue;

            var originalValue = property.GetValue(original);
            var modifiedValue = property.GetValue(modified);

            if (!Equals(originalValue, modifiedValue))
                return true;
        }

        return false;
    }
}
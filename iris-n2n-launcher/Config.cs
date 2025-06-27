using Newtonsoft.Json;
using System.Reflection;

namespace iris_n2n_launcher.Config;

public class Configuration
{
    /// <summary>
    /// ʹ�õ������ļ�
    /// </summary>
    public string ConfigName { set; get; } = "default";
    /// <summary>
    /// �㲥�޸�
    /// </summary>
    public bool BroadcastRepair { set; get; } = true;
    /// <summary>
    /// ����Ͷ��
    /// </summary>
    public bool Airportal { set; get; } = false;
    /// <summary>
    /// �˿�ӳ��
    /// </summary>
    public bool TcpUdpForw { set; get; } = false;
    /// <summary>
    /// ����ʱ�رշ���ǽ
    /// </summary>
    public bool DisableFirewallOnRun { set; get; } = false;
    /// <summary>
    /// �ҵ����緿��ת��
    /// </summary>
    public bool Minecraft { set; get; } = false;
    /// <summary>
    /// �رյ�ʱ��������ǽ
    /// </summary>
    public bool FirewallEnabledOnExit { set; get; } = false;
    /// <summary>
    /// ע��iris����
    /// </summary>
    public bool UrlRegister { set; get; } = false;
    /// <summary>
    /// ���¼��
    /// </summary>
    public bool VersionUpdate { set; get; } = true;
    /// <summary>
    /// �û��Զ���������б�
    /// </summary>
    public List<string> UserServerList { get; set; } = [];
    /// <summary>
    /// ���߸��µķ������б�
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
    /// ��ȡ���������б�
    /// </summary>
    public IEnumerable<string?> ListConfigs()
    {
        if (!Directory.Exists(_path))
            return Enumerable.Empty<string>();

        return Directory.GetFiles(_path, "*.json")
                       .Select(Path.GetFileNameWithoutExtension);
    }

    /// <summary>
    /// ��������Ƿ����
    /// </summary>
    public bool ConfigExists(string configName)
    {
        if (string.IsNullOrWhiteSpace(configName))
            return false;

        var filePath = GetConfigFilePath(configName);
        return File.Exists(filePath);
    }

    /// <summary>
    /// ���������л�ΪJSON�ַ���
    /// </summary>
    public string Serialize<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, _jsonSettings);
    }

    /// <summary>
    /// ��JSON�ַ��������л�Ϊ����
    /// </summary>
    public static T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json)!;
    }

    private string GetConfigFilePath(string configName)
    {
        // ת��һ���ļ�����
        var safeName = string.Join("_", configName.Split(Path.GetInvalidFileNameChars()));
        return Path.Combine(_path, $"{safeName}.json");
    }

    /// <summary>
    /// ������������Ƿ��в���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="original">ԭʼ����</param>
    /// <param name="modified">�޸ĺ�Ķ���</param>
    /// <returns>�Ƿ��в���</returns>
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
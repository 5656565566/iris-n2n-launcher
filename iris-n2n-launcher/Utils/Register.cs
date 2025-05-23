using Microsoft.Win32;
using System.Reflection;


namespace iris_n2n_launcher.Utils;

internal class Register
{
    public static void RegisterCustomProtocol()
    {
        string protocolName = "iris";
        string? applicationPath = Environment.ProcessPath;

        if (applicationPath == null)
        {
            return;
        }

        // 创建或打开HKEY_CLASSES_ROOT\{protocol}注册表项
        RegistryKey protocolKey = Registry.ClassesRoot.CreateSubKey(protocolName);
        if (protocolKey != null)
        {
            protocolKey.SetValue("", "URL: Custom Protocol");
            protocolKey.SetValue("URL Protocol", "");

            // 创建shell/open/command注册表项并设置默认值
            RegistryKey commandKey = protocolKey.CreateSubKey(@"shell\open\command");
            if (commandKey != null)
            {
                commandKey.SetValue("", $"\"{applicationPath}\" \"%1\"");
                commandKey.Close();
            }

            protocolKey.Close();
        }
    }

    public static void UnregisterCustomProtocol()
    {
        string protocolName = "iris";

        // 删除HKEY_CLASSES_ROOT\{protocol}注册表项
        Registry.ClassesRoot.DeleteSubKeyTree(protocolName, false);
    }
}

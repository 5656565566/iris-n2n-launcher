using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;

namespace iris_n2n_launcher.Utils;

/// <summary>
/// 机器码生成器
/// </summary>
class MachineCode
{
    public static string Generate()
    {
        try
        {
            string cpuInfo = GetCpuInfo();
            string macAddress = GetMacAddress();
            string diskId = GetDiskId();
            string windowsId = GetWindowsId();
            string machineName = Environment.MachineName;

            string combined = $"{cpuInfo}{macAddress}{diskId}{windowsId}{machineName}";

            using SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        catch
        {
            return Guid.NewGuid().ToString("N");
        }
    }

    private static string GetCpuInfo()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                return key?.GetValue("ProcessorNameString")?.ToString() ?? "";
            }
            return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "";
        }
        catch
        {
            return "";
        }
    }

    private static string GetMacAddress()
    {
        try
        {
            var firstInterface = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up &&
                                   !string.IsNullOrEmpty(n.GetPhysicalAddress().ToString()));

            return firstInterface?.GetPhysicalAddress().ToString() ?? "";
        }
        catch
        {
            return "";
        }
    }

    private static string GetDiskId()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                return key?.GetValue("InstallDate")?.ToString() ?? "";
            }
            return "";
        }
        catch
        {
            return "";
        }
    }

    private static string GetWindowsId()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                return key?.GetValue("ProductId")?.ToString() ?? "";
            }
            return "";
        }
        catch
        {
            return "";
        }
    }
}
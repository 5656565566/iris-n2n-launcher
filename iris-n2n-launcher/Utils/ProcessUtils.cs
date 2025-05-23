using System.Diagnostics;
using System.Net.NetworkInformation;

namespace iris_n2n_launcher.Utils
{
    internal class ProcessUtils
    {
        public static void ReleaseProcessByPid(int pid)
        {
            try
            {
                Process process = Process.GetProcessById(pid);
                process.Kill();
                Console.WriteLine($"进程 {pid} 已被释放。");
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"没有找到进程ID为 {pid} 的进程。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"释放进程时出现异常：{ex.Message}");
            }
        }

        public static int GetPidByPort(int port)
        {
            try
            {
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

                foreach (TcpConnectionInformation connection in tcpConnections)
                {
                    if (connection.LocalEndPoint.Port == port)
                    {
                        int pid = connection.LocalEndPoint.Port;
                        Console.WriteLine($"端口 {port} 对应的进程PID为 {pid}");
                        return pid;
                    }
                }

                Console.WriteLine($"没有找到占用端口 {port} 的进程。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取进程PID时出现异常：{ex.Message}");
            }

            return -1;
        }
        public static void ReleaseProcessByName(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                foreach (Process process in processes)
                {
                    process.Kill();
                    Console.WriteLine($"进程 {process.Id} ({process.ProcessName}) 已被释放。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"释放进程时出现异常：{ex.Message}");
            }
        }
    }
}

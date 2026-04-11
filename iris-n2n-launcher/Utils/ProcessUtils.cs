using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace iris_n2n_launcher.Utils
{
    internal class ProcessUtils
    {
        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(
            IntPtr pTcpTable,
            ref int dwOutBufLen,
            bool sort,
            int ipVersion,
            TCP_TABLE_CLASS tblClass,
            uint reserved = 0);

        private const int AfInet = 2;

        private enum TCP_TABLE_CLASS
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_TCPROW_OWNER_PID
        {
            public uint state;
            public uint localAddr;
            public uint localPort;
            public uint remoteAddr;
            public uint remotePort;
            public uint owningPid;
        }
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
            IntPtr tcpTablePointer = IntPtr.Zero;
            try
            {
                int bufferLength = 0;
                uint result = GetExtendedTcpTable(
                    IntPtr.Zero,
                    ref bufferLength,
                    true,
                    AfInet,
                    TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);

                tcpTablePointer = Marshal.AllocHGlobal(bufferLength);
                result = GetExtendedTcpTable(
                    tcpTablePointer,
                    ref bufferLength,
                    true,
                    AfInet,
                    TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);

                if (result != 0)
                {
                    Console.WriteLine($"获取 TCP 连接表失败，错误码：{result}");
                    return -1;
                }

                int rowCount = Marshal.ReadInt32(tcpTablePointer);
                IntPtr rowPointer = IntPtr.Add(tcpTablePointer, sizeof(int));
                int rowSize = Marshal.SizeOf<MIB_TCPROW_OWNER_PID>();

                for (int i = 0; i < rowCount; i++)
                {
                    var row = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(rowPointer);
                    int localPort = ConvertPortToHostOrder(row.localPort);
                    if (localPort == port)
                    {
                        int pid = unchecked((int)row.owningPid);
                        Console.WriteLine($"端口 {port} 对应的进程PID为 {pid}");
                        return pid;
                    }

                    rowPointer = IntPtr.Add(rowPointer, rowSize);
                }

                Console.WriteLine($"没有找到占用端口 {port} 的进程。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取进程PID时出现异常：{ex.Message}");
            }
            finally
            {
                if (tcpTablePointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(tcpTablePointer);
                }
            }

            return -1;
        }

        private static int ConvertPortToHostOrder(uint port)
        {
            return ((int)port >> 8 & 0xFF) | (((int)port & 0xFF) << 8);
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

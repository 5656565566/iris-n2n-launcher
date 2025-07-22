using iris_n2n_launcher.UI;
using iris_n2n_launcher.Utils;
using iris_n2n_launcher.Utils.FileTransfer;
using iris_n2n_launcher.Config;
using iris_n2n_launcher.N2N;
using static iris_n2n_launcher.Utils.FirewallHelper;
using AutoUpdaterDotNET;
using System.IO.Pipes;
using System.Text.RegularExpressions;

namespace iris_n2n_launcher;

internal static class Program
{
    private static readonly ExeHelper exeHelper = ExeHelper.Instance;
    private static readonly LogHelper logHelper = LogHelper.Instance;
    private static readonly Mutex mutex = new(true, "{3A3BC4F2-FA1E-4BB5-B01A-65C52D6A154C}");
    private static readonly ConfigManager configManager = ConfigManager.Instance;
    private static readonly TcpUdpForw tcpUdpForw = TcpUdpForw.Instance;
    private static FileTransferService? fileTransferService;
    private static bool shouldExit = false;
    private static MainForm? mainForm;
    private static readonly string pipeName = "IrisN2NPipe";
    private static readonly int pipeConnectionTimeout = 3000;
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD103:Call async methods when in an async method", Justification = "<����>")]
    static async Task Main(string[] args)
    {
        await FirewallManager.AllowProgramAsync("n2n", Environment.ProcessPath!);

        ApplicationConfiguration.Initialize();

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        
        Application.ThreadException += (s, e) => {
            string? stackTrace = e.Exception.StackTrace;
            string message = $"�쳣���� UI�߳��쳣: {e.Exception.Message}\n\n����ջ:\n{stackTrace}\n���ʹ�����־���԰�����λ����Ŷ";
            MessageBox.Show(message, "N2N ������һ���쳣", MessageBoxButtons.OK, MessageBoxIcon.Error);

            LogException(e.Exception);
        };

        AppDomain.CurrentDomain.UnhandledException += (s, e) => {
            var ex = e.ExceptionObject as Exception;
            string stackTrace = ex?.StackTrace ?? "�޵���ջ��Ϣ";
            string message = $"�쳣���� ��UI�߳��쳣: {ex?.Message ?? "δ֪����"}\n\n����ջ:\n{stackTrace}\n���ʹ�����־���԰�����λ����Ŷ";
            MessageBox.Show(message, "N2N ������һ���쳣", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (ex != null)
            {
                LogException(ex);
            }
        };

        if (args.Length < 1 && !mutex.WaitOne(TimeSpan.Zero, true))
        {
            MessageBox.Show("Ӧ�ó������������С�");
            return;
        }
        else if (args.Length > 0 && args[0].Contains("iris") && !mutex.WaitOne(TimeSpan.Zero, true))
        {
            TrySendToRunningInstance(args[0]);
            return;
        }
        else if (args.Length > 0 && args[0].Contains("--boot"))
        {
            mainForm = new MainForm(1);
        }
        else if (args.Length > 0 && args[0].Contains("iris"))
        {
            mainForm = new MainForm(2)
            {
                share = args[0]
            };
        }
        else
        {
            mainForm = new MainForm(0);
        }

        bool shouldStartApp = true;

        if (!configManager.ConfigExists("config"))
        {
            configManager.SaveConfig("config", new Configuration());
        }

        if (!configManager.ConfigExists("default"))
        {
            configManager.SaveConfig("default", new N2NConfiguration());
        }

        var config = configManager.LoadConfig<Configuration>("config");

        var server = IrisServerApi.GetServerList();

        if (server != null)
        {
            if (server.Count > 0)
            {
                config.OnlineServerList.Clear();
            }

            foreach (var item in server)
            {
                if (item.Port == 7654) { config.OnlineServerList.Add(item.Server!); }
                else { config.OnlineServerList.Add($"{item.Server}:{item.Port}"); }
            }
        }

        configManager.SaveConfig("config", config);

        AutoUpdater.CheckForUpdateEvent += args =>
        {
            if (args.Error != null)
            {
                MessageBox.Show($"������ʧ�ܣ�{args.Error.Message}", "���´���", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (args.IsUpdateAvailable)
            {
                ExeHelper.CloseProcess("WinIPBroadcast");
                ExeHelper.CloseProcess("orchid-tcpudp-forw");
                ExeHelper.CloseProcess("edge");

                bool isMandatory = args.Mandatory != null && args.Mandatory.Value;

                var message = isMandatory
                    ? $"���ֱ��밲װ���°汾 {args.InstalledVersion} -> {args.CurrentVersion}"
                    : $"�����°汾 {args.InstalledVersion}���Ƿ���µ� {args.CurrentVersion}��";

                var buttons = isMandatory ? MessageBoxButtons.OK : MessageBoxButtons.YesNo;

                var result = MessageBox.Show(
                    message,
                    "�������",
                    buttons,
                    isMandatory ? MessageBoxIcon.Warning : MessageBoxIcon.Question);

                if (isMandatory || result == DialogResult.Yes)
                {
                    shouldStartApp = false;
                    AutoUpdater.DownloadUpdate(args);
                }
            }
        };

        AutoUpdater.ShowSkipButton = true;
        AutoUpdater.ReportErrors = true;
        AutoUpdater.ShowRemindLaterButton = true;
        AutoUpdater.LetUserSelectRemindLater = true;
        AutoUpdater.TopMost = true;
        AutoUpdater.Synchronous = true;

        AutoUpdater.ClearAppDirectory = false; // ������Ŀ¼

        if (config.VersionUpdate)
        {
            AutoUpdater.Start("https://iris.home56.top/api/update-feed");
        }

        if (shouldStartApp)
        {
            StartPipeListener();

            ExeHelper.CloseProcess("WinIPBroadcast");
            ExeHelper.CloseProcess("orchid-tcpudp-forw");
            ExeHelper.CloseProcess("edge");

            if (config.TcpUdpForw)
            {
                tcpUdpForw.Start();
            }
            else
            {
                tcpUdpForw.Stop();
            }

            if (config.Airportal)
            {
                fileTransferService = FileTransferService.Instance;
                fileTransferService.Start();
            }

            Application.Run(mainForm);
        }
        else
        {
            return;
        }

        shouldExit = false;

        await FirewallManager.DeleteRuleAsync("n2n (Inbound)");
        await FirewallManager.DeleteRuleAsync("n2n (Outbound)");
        await FirewallManager.DeleteRuleAsync("n2n-tcpudp-forw (Inbound)");
        await FirewallManager.DeleteRuleAsync("n2n-tcpudp-forw (Outbound)");
        await FirewallManager.DeleteRuleAsync("n2n-edge (Inbound)");
        await FirewallManager.DeleteRuleAsync("n2n-edge (Outbound)");
        await FirewallManager.DeleteRuleAsync("n2n-WinIPBroadcast (Inbound)");
        await FirewallManager.DeleteRuleAsync("n2n-WinIPBroadcast (Outbound)");
        await FirewallManager.DeleteRuleAsync("n2n-ip");
        await FirewallManager.DeleteRuleAsync("n2n-ping");

        if (configManager.LoadConfig<Configuration>("config").FirewallEnabledOnExit)
        {
            FirewallOperateByObject(true, true, true);
        }

        using var wakeupPipe = new NamedPipeClientStream(".", pipeName);
        await wakeupPipe.ConnectAsync(pipeConnectionTimeout);

        exeHelper.Dispose();
        mutex.Dispose();
        fileTransferService?.Dispose();
    }

    private static void LogException(Exception ex)
    {
        try
        {
            string logContent = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                                $"�쳣����: {ex.GetType().FullName}\n" +
                                $"�쳣��Ϣ: {ex.Message}\n" +
                                $"����ջ:\n{ex.StackTrace}\n\n";

            logHelper.Error(logContent);
        }
        catch
        {
            // ������־��¼���������쳣
        }
    }
    private static bool TrySendToRunningInstance(string link)
    {
        try
        {
            using var clientPipe = new NamedPipeClientStream(
                ".",
                pipeName,
                PipeDirection.Out,
                PipeOptions.Asynchronous);

            clientPipe.Connect(pipeConnectionTimeout);

            using var writer = new StreamWriter(clientPipe);
            writer.AutoFlush = true;
            writer.WriteLine(link);
            return true;
        }
        catch (TimeoutException)
        {
            // û���ҵ������е�ʵ��
            return false;
        }
        catch
        {
            return false;
        }
    }

    private static void ProcessSharingLink(string link)
    {

        string pattern = @"iris:\/\/[^\s]+";
        Regex regex = new Regex(pattern);

        Match match = regex.Match(link.TrimEnd('/'));

        if (match.Success)
        {
            string irisLink = match.Value;
            string[] share = ShareForm.ReadUrl(irisLink.Replace("iris://", "")).Split('#');

            var config = configManager.LoadConfig<Configuration>("config");
            var n2nconfig = configManager.LoadConfig<N2NConfiguration>(config.ConfigName);

            n2nconfig.SuperNodeHostAndPort = share[0];
            n2nconfig.Community = share[1];

            mainForm?.Invoke((MethodInvoker)delegate
            {
                mainForm.UrlJoin(n2nconfig);
            });
        }
    }

    private static void StartPipeListener()
    {
        var thread = new Thread(() =>
        {
            while (!shouldExit)
            {
                try
                {
                    using var serverPipe = new NamedPipeServerStream(
                        pipeName,
                        PipeDirection.InOut,
                        1,
                        PipeTransmissionMode.Message, // ʹ����Ϣģʽ
                        PipeOptions.Asynchronous);

                    serverPipe.WaitForConnection();

                    using var reader = new StreamReader(serverPipe);
                    string? receivedMessage = reader.ReadLine();

                    if (!string.IsNullOrEmpty(receivedMessage))
                    {
                        ProcessSharingLink(receivedMessage);
                    }
                }
                catch
                {
                    if (!shouldExit) continue;
                }
            }
        })
        {
            IsBackground = true
        };
        thread.Start();
    }
}
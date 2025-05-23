using System.Collections.Concurrent;
using System.Diagnostics;


namespace iris_n2n_launcher.Utils;

/// <summary>
/// 进程执行帮助类，提供进程执行和管理功能
/// </summary>
public sealed class ExeHelper : IDisposable
{
    private static readonly Lazy<ExeHelper> _instance = new(() => new ExeHelper());

    /// <summary>
    /// 获取 ExeHelper 的单例实例
    /// </summary>
    public static ExeHelper Instance => _instance.Value;
    private readonly ConcurrentDictionary<int, Process> _runningProcesses = new();
    private bool _disposed;
    public record CommandResult(string StdOut, string StdErr, int ExitCode);

    private ExeHelper() { }

    #region 进程管理

    /// <summary>
    /// 创建并启动一个进程
    /// </summary>
    /// <param name="command">要执行的程序</param>
    /// <param name="arguments">需要传递的参数</param>
    /// <param name="workDirPath">工作目录</param>
    /// <returns>创建的进程</returns>
    public Process CreateProcess(string command, string arguments, string workDirPath = "", bool useOutputHandlers = true)
    {
        LogHelper logHelper = LogHelper.Instance;

        var processInfo = new ProcessStartInfo(command, arguments)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (!string.IsNullOrWhiteSpace(workDirPath))
        {
            processInfo.WorkingDirectory = workDirPath;
        }

        var process = new Process { StartInfo = processInfo };

        if (useOutputHandlers)
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    logHelper.Info(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    logHelper.Error(e.Data);
                }
            };
        }

        process.Start();

        if (useOutputHandlers)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        _runningProcesses.TryAdd(process.Id, process);
        process.Exited += (sender, e) =>
        {
            _runningProcesses.TryRemove(process.Id, out _);
            // logHelper.Warn($"Process {process.Id} has exited with code {process.ExitCode}");
        };

        return process;
    }


    /// <summary>
    /// 执行命令并检查输出是否包含指定字符串
    /// </summary>
    /// <param name="command">要执行的命令</param>
    /// <param name="arguments">参数</param>
    /// <param name="workDirPath">工作目录</param>
    /// <param name="expectedOutput">期望输出中包含的字符串</param>
    /// <returns>是否包含指定字符串</returns>
    public async Task<bool> RunCommandAsync(string command, string arguments, string workDirPath = "", string expectedOutput = "")
    {
        try
        {
            using var process = CreateProcess(command, arguments, workDirPath, useOutputHandlers: false);
            LogHelper.Instance.Info("Command started: {0} {1}", command, arguments);

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            LogHelper.Instance.Info("Command exit code: {0}", process.ExitCode);

            if (!string.IsNullOrWhiteSpace(output))
            {
                LogHelper.Instance.Info("Command output:\n{0}", output);
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                LogHelper.Instance.Info("Command error output:\n{0}", error);
            }

            return string.IsNullOrEmpty(expectedOutput) ||
                   output.Contains(expectedOutput, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception e)
        {
            LogHelper.Instance.Error(e, "Command execution failed");
            return false;
        }
    }
    /// <summary>
    /// 运行命令返回输出
    /// </summary>
    /// <param name="command">要执行的命令</param>
    /// <param name="arguments">参数</param>
    /// <param name="workDirPath">工作目录</param>
    /// <returns></returns>
    public async Task<CommandResult> RunCommandWithOutputAsync(string command, string arguments, string workDirPath = "")
    {
        try
        {
            using var process = CreateProcess(command, arguments, workDirPath, useOutputHandlers: false);
            LogHelper.Instance.Info("Command started: {0} {1}", command, arguments);

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var stdout = await outputTask;
            var stderr = await errorTask;

            LogHelper.Instance.Info("Command exit code: {0}", process.ExitCode);
            if (!string.IsNullOrWhiteSpace(stdout))
            {
                LogHelper.Instance.Info("Command output:\n{0}", stdout);
            }

            if (!string.IsNullOrWhiteSpace(stderr))
            {
                LogHelper.Instance.Info("Command error output:\n{0}", stderr);
            }

            return new CommandResult(stdout, stderr, process.ExitCode);
        }
        catch (Exception e)
        {
            LogHelper.Instance.Error(e, "Command execution failed");
            return new CommandResult(string.Empty, e.ToString(), -1);
        }
    }

    /// <summary>
    /// 关闭指定名称的进程
    /// </summary>
    /// <param name="processName">进程名</param>
    /// <param name="forceKill">是否强制终止</param>
    /// <returns>是否成功关闭</returns>
    public static bool CloseProcess(string processName, bool forceKill = false)
    {
        bool result = false;
        var processes = Process.GetProcessesByName(processName);

        foreach (var process in processes)
        {
            try
            {
                if (!process.CloseMainWindow() || forceKill)
                {
                    process.Kill();
                }
                result = true;
                LogHelper.Instance.Info("Process {0} (ID: {1}) was terminated", processName, process.Id);
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(e, $"Failed to terminate process {processName}");
            }
        }

        return result;
    }

    /// <summary>
    /// 关闭所有由该类创建的进程
    /// </summary>
    public void CloseAllManagedProcesses()
    {
        foreach (var process in _runningProcesses.Values)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    LogHelper.Instance.Info("Managed process {0} (ID: {1}) was terminated",
                        process.ProcessName, process.Id);
                }
            }
            catch
            {
                //LogHelper.Instance.Error(e, $"Failed to terminate managed process {process.ProcessName}");
            }
        }

        _runningProcesses.Clear();
    }

    #endregion

    #region 销毁

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            CloseAllManagedProcesses();
        }

        _disposed = true;
    }

    ~ExeHelper()
    {
        Dispose(false);
    }

    #endregion
}
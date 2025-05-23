using iris_n2n_launcher.Properties;
using Microsoft.VisualStudio.Threading;
using NLog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace iris_n2n_launcher.Utils.FileTransfer;

public sealed class FileTransferService : IDisposable
{
    private static readonly Lazy<FileTransferService> _instance =
        new(() => new FileTransferService());

    public static FileTransferService Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, FileTransferTask> _activeTasks = new();
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokens = new();
    private readonly ConcurrentDictionary<string, FileTransferTask> _endTasks = new();
    private TcpListener? _listener;
    private bool _disposed;

    public event EventHandler<FileTransferEventArgs>? OnReceiveSuccess;
    public event EventHandler<FileTransferEventArgs>? OnReceiveFailure;
    public event EventHandler<FileTransferEventArgs>? OnReceiveCancelled;
    public event EventHandler<FileTransferEventArgs>? OnSendSuccess;
    public event EventHandler<FileTransferEventArgs>? OnSendFailure;
    public event EventHandler<FileTransferEventArgs>? OnSendCancelled;
    public event EventHandler<FileRequestEventArgs>? OnReceiveRequest;

    private Task? _receivingTask;
    private CancellationTokenSource? _receivingCts;
    private NotifyIcon notifyIcon;

    public int ReceiverPort;

    private FileTransferService() { }

    public void SendFileToIp(string ipAddress, List<string> filePaths, int receiverPort)
    {
        foreach (var path in filePaths)
        {
            if (!File.Exists(path))
            {
                OnSendFailure?.Invoke(this, new FileTransferEventArgs(path, "找不到这个文件..."));
                continue;
            }

            var fileInfo = new FileInfo(path);
            var task = new FileTransferTask
            {
                Type = TransferType.Send,
                FileName = Path.GetFileName(path),
                FileSize = fileInfo.Length,
                RemoteIP = ipAddress,
                Status = TransferStatus.Pending
            };

            if (!_activeTasks.TryAdd(task.TaskId, task))
            {
                continue;
            }

            _ = SafeExecuteTaskAsync(
                () => ProcessSendTaskAsync(task, path, receiverPort),
                task.TaskId,
                isReceiveTask: false);
        }
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size = size / 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    private string? FileTransferOnReceiveRequest(string fileName, long size)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = $"接收文件：{fileName} ({FormatFileSize(size)})，请选择保存路径："
        };


        if (dialog.ShowDialog() == DialogResult.OK)
        {
            return Path.Combine(dialog.SelectedPath, fileName);
        }

        return null;
    }

    private void InitFileTransfer()
    {
        OnReceiveRequest += (sender, e) =>
        {
            var mre = new ManualResetEvent(false);
            string? selectedPath = null;
            var t = new Thread(() =>
            {
                selectedPath = FileTransferOnReceiveRequest(e.FileName, e.FileSize);
                mre.Set();
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            mre.WaitOne();

            if (!string.IsNullOrEmpty(selectedPath))
                e.SavePath = selectedPath;
            else
                e.SavePath = "";
        };

        notifyIcon = new NotifyIcon
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
            Visible = true,
            BalloonTipTitle = "文件传输通知",
            BalloonTipIcon = ToolTipIcon.Info
        };

        // 配置事件处理
        OnReceiveSuccess += (sender, e) =>
        {
            ShowBalloon($"接收成功：{e.FilePath}", e.Message, ToolTipIcon.Info);
        };

        OnReceiveFailure += (sender, e) =>
        {
            ShowBalloon($"接收失败：{e.FilePath}", e.Message, ToolTipIcon.Error);
        };

        OnReceiveCancelled += (sender, e) =>
        {
            ShowBalloon($"接收取消：{e.FilePath}", e.Message, ToolTipIcon.Warning);
        };

        OnSendSuccess += (sender, e) =>
        {
            ShowBalloon($"发送成功：{e.FilePath}", e.Message, ToolTipIcon.Info);
        };

        OnSendFailure += (sender, e) =>
        {
            ShowBalloon($"发送失败：{e.FilePath}", e.Message, ToolTipIcon.Error);
        };

        OnSendCancelled += (sender, e) =>
        {
            ShowBalloon($"发送取消：{e.FilePath}", e.Message, ToolTipIcon.Warning);
        };
    }
    private void ShowBalloon(string title, string message, ToolTipIcon icon)
    {
        notifyIcon.BalloonTipTitle = title;
        notifyIcon.BalloonTipText = message;
        notifyIcon.BalloonTipIcon = icon;
        notifyIcon.ShowBalloonTip(3000); // 显示3秒后自动消失
    }

    public List<FileTransferTask> GetTaskQueue() =>
        [.. _activeTasks.Values, .. _endTasks.Values];

    private async Task ProcessSendTaskAsync(FileTransferTask task, string filePath, int receiverPort)
    {
        using var cts = new CancellationTokenSource();
        if (!_cancellationTokens.TryAdd(task.TaskId, cts))
        {
            return;
        }

        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse(task.RemoteIP), receiverPort)
                .ConfigureAwait(false);

            using var stream = client.GetStream();
            using var fileStream = File.OpenRead(filePath);

            await SendFileMetadataAsync(stream, task.FileName, task.FileSize, cts.Token);

            task.Status = TransferStatus.Progressing;
            var sw = Stopwatch.StartNew();

            var buffer = new byte[8192];
            int bytesRead;
            long totalRead = 0;

            while ((bytesRead = await fileStream.ReadAsync(buffer, cts.Token)) > 0)
            {
                await stream.WriteAsync(buffer.AsMemory(0, bytesRead), cts.Token);
                totalRead += bytesRead;

                UpdateTaskProgress(task, totalRead, sw.Elapsed);
            }

            OnSendSuccess?.Invoke(this, new FileTransferEventArgs(filePath, "发送完成"));
            task.Status = TransferStatus.Completed;
        }
        catch (OperationCanceledException)
        {
            OnSendCancelled?.Invoke(this, new FileTransferEventArgs(filePath, "传输取消"));
            task.Status = TransferStatus.Canceled;
        }
        catch (Exception ex)
        {
            OnSendFailure?.Invoke(this, new FileTransferEventArgs(filePath, ex.Message));
            task.Status = TransferStatus.Failed;
        }
        finally
        {
            _activeTasks.TryRemove(task.TaskId, out _);
            task.TransferSpeed = 0;
            _endTasks.TryAdd(task.TaskId, task);
            _cancellationTokens.TryRemove(task.TaskId, out _);
            task.EndTime = DateTime.UtcNow;
        }
    }

    private async Task SendFileMetadataAsync(NetworkStream stream, string fileName, long fileSize, CancellationToken ct)
    {
        var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
        var fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
        await stream.WriteAsync(fileNameLengthBytes, 0, 4, ct);

        await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length, ct);

        var fileSizeBytes = BitConverter.GetBytes(fileSize);
        await stream.WriteAsync(fileSizeBytes, 0, 8, ct);
    }

    private static void UpdateTaskProgress(FileTransferTask task, long bytesTransferred, TimeSpan elapsed)
    {
        task.Progress = (double)bytesTransferred / task.FileSize * 100;
        task.ElapsedTime = elapsed;
        task.TransferSpeed = bytesTransferred / Math.Max(elapsed.TotalSeconds, 0.1);
    }

    public void CancelTransfer(string taskId)
    {
        if (_cancellationTokens.TryGetValue(taskId, out var cts))
        {
            cts.Cancel();
        }
    }
    public bool IsRun()
    {
        if (_listener == null) { return false; }
        return true;
    }

    public void Start()
    {
        if (_receivingTask != null && !_receivingTask.IsCompleted)
            return;

        _receivingCts = new CancellationTokenSource();
        _receivingTask = Task.Run(async () =>
        {
            try
            {
                if (OnReceiveCancelled == null) { InitFileTransfer(); }

                ReceiverPort = PortUtility.GetRandomUnusedPort(5000, 8000);
                _listener = new TcpListener(IPAddress.Any, ReceiverPort);
                _listener.Start();

                while (!_receivingCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var client = await _listener.AcceptTcpClientAsync()
                            .WithCancellation(_receivingCts.Token)
                            .ConfigureAwait(false);

                        _ = SafeExecuteTaskAsync(
                            () => HandleReceivingClientAsync(client),
                            Guid.NewGuid().ToString(),
                            isReceiveTask: true);
                    }
                    catch (OperationCanceledException)
                    {
                        // 正常停止
                        break;
                    }
                    catch { }
                }
            }
            finally
            {
                _listener?.Stop();
            }
        }, _receivingCts.Token);
    }
    public void Stop(int timeoutMs = 5000)
    {
        if (_receivingTask == null || _receivingTask.IsCompleted)
            return;

        try
        {
            _receivingCts?.Cancel();
            _listener?.Stop();
            _receivingTask?.Wait(timeoutMs);
        }
        catch
        {
            return;
        }
        finally
        {
            _listener = null;
            _receivingCts?.Dispose();
            _receivingCts = null;
            _receivingTask = null;
        }
    }

    private async Task HandleReceivingClientAsync(TcpClient client)
    {
        using (client)
        using (var stream = client.GetStream())
        {
            var (fileName, fileSize) = await ReadFileMetadataAsync(stream).ConfigureAwait(false);

            var args = new FileRequestEventArgs(fileName, fileSize);
            OnReceiveRequest?.Invoke(this, args);

            if (string.IsNullOrEmpty(args.SavePath))
            {
                await SendCancellationSignalAsync(stream);
                return;
            }

            var task = new FileTransferTask
            {
                Type = TransferType.Receive,
                FileName = fileName,
                FileSize = fileSize,
                Status = TransferStatus.Progressing
            };

            if (!_activeTasks.TryAdd(task.TaskId, task))
            {
                return;
            }

            try
            {
                using var fileStream = File.Create(args.SavePath);
                var sw = Stopwatch.StartNew();
                long totalRead = 0;
                var buffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
                    totalRead += bytesRead;
                    UpdateTaskProgress(task, totalRead, sw.Elapsed);
                }

                OnReceiveSuccess?.Invoke(this, new FileTransferEventArgs(args.SavePath, "接收完成"));
                task.Status = TransferStatus.Completed;
            }
            catch (OperationCanceledException)
            {
                OnReceiveCancelled?.Invoke(this, new FileTransferEventArgs(args.SavePath, "传输取消"));
                task.Status = TransferStatus.Canceled;
            }
            catch (Exception ex)
            {
                OnReceiveFailure?.Invoke(this, new FileTransferEventArgs(args.SavePath, ex.Message));
                task.Status = TransferStatus.Failed;
            }
            finally
            {
                _activeTasks.TryRemove(task.TaskId, out _);
                task.TransferSpeed = 0;
                _endTasks.TryAdd(task.TaskId, task);
                task.EndTime = DateTime.UtcNow;
            }
        }
    }

    private async Task<(string fileName, long fileSize)> ReadFileMetadataAsync(NetworkStream stream)
    {
        var fileNameLengthBytes = new byte[4];
        await stream.ReadExactlyAsync(fileNameLengthBytes, 0, 4);
        int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);

        var fileNameBytes = new byte[fileNameLength];
        await stream.ReadExactlyAsync(fileNameBytes, 0, fileNameLength);
        string fileName = Encoding.UTF8.GetString(fileNameBytes);

        var fileSizeBytes = new byte[8];
        await stream.ReadExactlyAsync(fileSizeBytes, 0, 8);
        long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

        return (fileName, fileSize);
    }

    private async Task SendCancellationSignalAsync(NetworkStream stream)
    {
        await stream.WriteAsync([0], 0, 1);
    }

    private async Task SafeExecuteTaskAsync(Func<Task> taskFunc, string taskId, bool isReceiveTask)
    {
        try
        {
            await taskFunc().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            HandleTaskException(ex, taskId, isReceiveTask);
        }
    }

    private void HandleTaskException(Exception ex, string taskId, bool isReceiveTask)
    {
        if (_activeTasks.TryGetValue(taskId, out var task))
        {
            var handler = isReceiveTask ? OnReceiveFailure : OnSendFailure;
            handler?.Invoke(this, new FileTransferEventArgs(task.FileName, ex.Message));
            task.Status = TransferStatus.Failed;
            task.EndTime = DateTime.UtcNow;
            _activeTasks.TryRemove(taskId, out _);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        if (_receivingTask != null && !_receivingTask.IsCompleted)
        {
            _receivingCts?.Cancel();
            _listener?.Stop();
            _receivingTask?.Wait(TimeSpan.FromSeconds(5));
        }

        foreach (var cts in _cancellationTokens.Values)
        {
            cts.Cancel();
            cts.Dispose();
        }
        _cancellationTokens.Clear();
        _receivingCts?.Dispose();
        _listener?.Stop();
        notifyIcon?.Dispose();
        GC.SuppressFinalize(this);
    }
}

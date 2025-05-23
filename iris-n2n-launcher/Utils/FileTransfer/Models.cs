namespace iris_n2n_launcher.Utils.FileTransfer;

public enum TransferType
{
    Send,
    Receive
}

public enum TransferStatus
{
    Pending,
    Progressing,
    Completed,
    Failed,
    Canceled
}

public class FileTransferTask
{
    public string TaskId { get; } = Guid.NewGuid().ToString();
    public TransferType Type { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string RemoteIP { get; set; }
    public double Progress { get; set; }
    public double TransferSpeed { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public TransferStatus Status { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
}

public class FileTransferEventArgs(string path, string msg) : EventArgs
{
    public string FilePath { get; } = path;
    public string Message { get; } = msg;
}

public class FileRequestEventArgs(string name, long size) : EventArgs
{
    public string FileName { get; } = name;
    public long FileSize { get; } = size;
    public string SavePath { get; set; }  // 由用户设置保存路径
}
using Microsoft.Win32.TaskScheduler;
using System.ComponentModel;
using System.IO;
using System.Security.Principal;

namespace iris_n2n_launcher.Utils;

internal class BootStart
{
    public static void CreateTask(string taskName, string exePath)
    {
        if (!IsAdministrator())
        {
            throw new UnauthorizedAccessException("需要管理员权限来创建计划任务");
        }

        if (!File.Exists(exePath))
        {
            throw new FileNotFoundException("目标程序路径不存在", exePath);
        }

        try
        {
            using TaskService taskService = new();

            if (TaskExists(taskName))
            {
                DeleteTask(taskName); // 先尝试删除旧任务
            }

            TaskDefinition taskDefinition = taskService.NewTask();

            taskDefinition.Principal.UserId = WindowsIdentity.GetCurrent().Name;
            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
            taskDefinition.Principal.LogonType = TaskLogonType.InteractiveToken;

            string exeDirectory = Path.GetDirectoryName(exePath)!;
            taskDefinition.Actions.Add(new ExecAction(
                path: $"\"{exePath}\"",
                arguments: "--boot",
                workingDirectory: $"\"{exeDirectory}\""));

            taskDefinition.Triggers.Add(new LogonTrigger { Delay = TimeSpan.FromSeconds(5) });

            taskDefinition.Settings.StopIfGoingOnBatteries = false;
            taskDefinition.Settings.DisallowStartIfOnBatteries = false;
            taskDefinition.Settings.AllowDemandStart = true;

            taskService.RootFolder.RegisterTaskDefinition(
                path: taskName,
                definition: taskDefinition,
                createType: TaskCreation.CreateOrUpdate,
                userId: null,
                password: null,
                logonType: TaskLogonType.InteractiveToken,
                sddl: null);
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == unchecked((int)0x80041319))
        {
            throw new Exception("任务创建失败：权限不足或任务已存在", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new Exception("访问被拒绝：请确保以管理员身份运行", ex);
        }
    }

    public static void DeleteTask(string taskName)
    {
        try
        {
            using TaskService taskService = new();
            taskService.RootFolder.DeleteTask(taskName, exceptionOnNotExists: false);
        }
        catch (Exception ex)
        {
            throw new Exception($"删除任务失败: {ex.Message}", ex);
        }
    }

    public static bool TaskExists(string taskName)
    {
        try
        {
            using TaskService taskService = new();
            return taskService.GetTask(taskName) != null;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
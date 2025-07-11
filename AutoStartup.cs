using Microsoft.Win32.TaskScheduler;
using System;
using System.Linq;
using System.Security.Principal;

namespace AutoPowerTimeOut;

public class StartupHelper
{
    private const string LogonTaskName = $"{Constants.AppName} Startup";
    private const string LogonTaskDesc = $"{Constants.AppName} Auto Startup";

    public static void CheckIsEnabled()
    {
        if (!CheckLogonTask())
        {
            ScheduleLogonTask();
        }
    }

    private static bool CheckLogonTask()
    {
        using var taskService = new TaskService();
        var task = taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == LogonTaskName);
        if (task != null)
        {
            try
            {
                // Check if the action is the same as the current executable path
                // If not, we need to unschedule and reschedule the task
                if (task.Definition.Actions.FirstOrDefault() is Microsoft.Win32.TaskScheduler.Action taskAction)
                {
                    var action = taskAction.ToString().Trim();
                    if (!action.Equals(Constants.ExecutablePath, StringComparison.OrdinalIgnoreCase))
                    {
                        UnscheduleLogonTask();
                        ScheduleLogonTask();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        return false;
    }

    private static bool ScheduleLogonTask()
    {
        using var td = TaskService.Instance.NewTask();
        td.RegistrationInfo.Description = LogonTaskDesc;
        td.Triggers.Add(new LogonTrigger { UserId = WindowsIdentity.GetCurrent().Name, Delay = TimeSpan.FromSeconds(2) });
        td.Actions.Add(Constants.ExecutablePath);

        if (IsCurrentUserIsAdmin())
        {
            td.Principal.RunLevel = TaskRunLevel.Highest;
        }

        td.Settings.StopIfGoingOnBatteries = false;
        td.Settings.DisallowStartIfOnBatteries = false;
        td.Settings.ExecutionTimeLimit = TimeSpan.Zero;

        try
        {
            TaskService.Instance.RootFolder.RegisterTaskDefinition(LogonTaskName, td);
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static bool UnscheduleLogonTask()
    {
        using var taskService = new TaskService();
        try
        {
            taskService.RootFolder.DeleteTask(LogonTaskName);
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static bool IsCurrentUserIsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}

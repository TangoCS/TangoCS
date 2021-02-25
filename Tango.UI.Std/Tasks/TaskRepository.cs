using System;
using System.Collections.Generic;
using Tango.Data;

namespace Tango.Tasks
{
    public interface ITaskRepository
    {
        IDatabase database { get; }

        IRepository<DTO_Task> GetTasks();
        int CreateTask(DTO_Task task);
        void UpdateTask(DTO_Task task);
        void DeleteTask(IEnumerable<int> ids);
        void DeactivationTasks(IEnumerable<int> ids);
        IEnumerable<DTO_TaskGroup> GetTaskGroups();
        IEnumerable<DTO_TaskStartType> GetTaskStartTypes();
        IEnumerable<TaskRunning> TasksRunning();
        IEnumerable<DTO_Task> TasksForExecute();
        IRepository<DTO_TaskParameter> GetTaskParameters();
        int CreateTaskParameter(DTO_TaskParameter taskparameter);
        void UpdateTaskParameter(DTO_TaskParameter taskparameter);
        void DeleteTaskParameter(IEnumerable<int> ids);
        IRepository<DTO_TaskExecution> GetTaskExecutions();
        void ClearTasksExecution(DateTime date);

        int CreateTaskExecution(DTO_TaskExecution execution);
        void UpdateTaskExecution(DTO_TaskExecution execution);
        void UpdateTaskExecutionError(DTO_TaskExecution execution, int errorid);
        void UpdateTaskExecutionTimeOut(TaskRunning task);
    }
}

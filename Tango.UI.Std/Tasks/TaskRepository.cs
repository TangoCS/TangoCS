using System;
using System.Collections.Generic;
using Tango.Data;

namespace Tango.Tasks
{
    public interface ITaskControllerRepository
    {
        IDatabase database { get; }

        DTO_Task GetTask(int id);
        IEnumerable<TaskRunning> TasksRunning();
        IEnumerable<DTO_Task> TasksForExecute();
        IEnumerable<DTO_TaskParameter> GetTaskParameters(int id);
        int CreateTaskExecution(DTO_TaskExecution execution);
        void UpdateTaskExecution(DTO_TaskExecution execution);
        void UpdateTaskExecutionError(DTO_TaskExecution execution, int errorid);
        void UpdateTaskExecutionTimeOut(TaskRunning task);
    }

    public interface ITaskRepository : IRepository<DTO_Task>
    {
        void Deactivation(IEnumerable<int> ids);
        IEnumerable<DTO_TaskGroup> GetGroups();
        IEnumerable<DTO_TaskStartType> GetStartTypes();
        bool IsExecuteTask(int id);
        IEnumerable<DTO_TaskParameter> GetParameters(int id);
        int CreateParameter(DTO_TaskParameter taskparameter);
        void DeleteParameter(int id);
    }

    public interface ITaskParameterRepository : IRepository<DTO_TaskParameter>
    {
        int MaximumSequenceNumber(int id);
    }

    public interface ITaskExecutionRepository : IRepository<DTO_TaskExecution>
    {
        void Clear(DateTime date);
    }
}

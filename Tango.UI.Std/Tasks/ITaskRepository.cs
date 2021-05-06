using System;
using System.Collections.Generic;
using Tango.Data;

namespace Tango.Tasks
{
    public interface ITaskControllerRepository
    {
        IDatabase database { get; }

        Task GetTask(int id);
        IEnumerable<TaskRunning> TasksRunning();
        IEnumerable<Task> TasksForExecute();
        IEnumerable<TaskParameter> GetTaskParameters(int id);
        int CreateTaskExecution(TaskExecution execution);
        void UpdateTaskExecution(TaskExecution execution);
        void UpdateTaskExecutionError(TaskExecution execution, int errorid);
        void UpdateTaskExecutionTimeOut(TaskRunning task);
    }

    public interface ITaskRepository : IRepository<Task>
    {
        void Deactivation(IEnumerable<int> ids);
        IEnumerable<TaskGroup> GetGroups();
        IEnumerable<TaskStartType> GetStartTypes();
        bool IsExecuteTask(int id);
        //bool IsEmptyGroup();
        IEnumerable<TaskParameter> GetParameters(int id);
        int CreateParameter(TaskParameter taskparameter);
        void DeleteParameter(int id);
    }

    public interface ITaskParameterRepository : IRepository<TaskParameter>
    {
        int MaximumSequenceNumber(int id);
    }

    public interface ITaskExecutionRepository : IRepository<TaskExecution>
    {
        void Clear(DateTime date);
    }
}

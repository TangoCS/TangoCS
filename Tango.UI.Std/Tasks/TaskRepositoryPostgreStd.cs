using System;
using System.Collections.Generic;
using Dapper;
using Tango.Data;
using Tango.Model;

namespace Tango.Tasks
{
	public class TaskControllerRepositoryPostgreStd : ITaskControllerRepository
    {
        public IDatabase database { get; }

        public TaskControllerRepositoryPostgreStd(IDatabase database)
        {
            this.database = database;
        }

        public Task GetTask(int id)
        {
            return database.Connection.QuerySingle<Task>(@"select t.*, tt.title as starttypetitle, tg.title as grouptitle, s.title as systemtitle
from tm_task t 
left outer join tm_taskgroup tg on t.taskgroupid = tg.taskgroupid
left outer join tm_taskstarttype tt on t.starttypeid = tt.taskstarttypeid
left outer join c_system s on t.systemid = s.systemid
where t.taskid = @id", new { id });
        }

        public IEnumerable<TaskParameter> GetTaskParameters(int id)
        {
            return database.Connection.Query<TaskParameter>(@"select tp.*, t.title as parenttitle, 
t.class as parentclass, t.method as parentmethod 
from tm_taskparameter tp join tm_task t on tp.parentid = t.taskid
where t.taskid = @id", new { id });
        }

        public IEnumerable<TaskRunning> TasksRunning()
        {
            return database.Connection.Query<TaskRunning>(@"select te.taskexecutionid, te.startdate, t.executiontimeout, t.taskid
from tm_task t join tm_taskexecution te on t.taskid = te.taskid 
where te.finishdate is null and t.startfromservice");
        }

        public IEnumerable<Task> TasksForExecute()
        {
            return database.Connection.Query<Task>(@"select t.* from tm_task t
where t.startfromservice and 
not exists (select 1 from tm_taskexecution te where te.taskid = t.taskid and te.finishdate is null)");
        }

        public int CreateTaskExecution(TaskExecution execution)
        {
            return execution.TaskExecutionID = database.Connection.QuerySingle<int>(@"
update tm_task set status = 1, laststartdate = now() where taskid = @TaskID;
insert into tm_taskexecution (lastmodifieddate, startdate, machinename, taskid, lastmodifieduserid, issuccessfull)
values (@LastModifiedDate, @StartDate, @MachineName, @TaskID, @LastModifiedUserID, @IsSuccessfull) 
returning taskexecutionid;", execution);
        }

        public void UpdateTaskExecution(TaskExecution execution)
        {
            database.Connection.ExecuteScalar(@"
update tm_task set status = 2, laststartdate = now() where taskid = @TaskID;
update tm_taskexecution set issuccessfull = @IsSuccessfull, finishdate = @FinishDate, lastmodifieddate = @LastModifiedDate, resultxml = @ResultXml 
where taskexecutionid = @TaskExecutionID;", execution);
        }

        public void UpdateTaskExecutionError(TaskExecution execution, int errorid)
        {
            if (errorid > 0)
                execution.ResultXml = $"<a href='/ic/ErrorLog/View?oid={errorid}' target='_blank'>Ошибка</a>";

            database.Connection.ExecuteScalar(@"
update tm_task set status = 3, laststartdate = now() where taskid = @TaskID;
update tm_taskexecution set issuccessfull = @IsSuccessfull, finishdate = @FinishDate, lastmodifieddate = @LastModifiedDate, resultxml = @ResultXml 
where taskexecutionid = @TaskExecutionID;", execution);
        }

        public void UpdateTaskExecutionTimeOut(TaskRunning task)
        {
            database.Connection.ExecuteScalar(@"
update tm_task set status = 3 where taskid = @TaskID;
update tm_taskexecution set issuccessfull = false, executionlog = 'Execution timed out', finishdate = now(), lastmodifieddate = now()
where taskexecutionid = @TaskExecutionID;", task);
        }
    }

    /// <summary>
    /// Postgre SQL стандартный репозиторий для задания
    /// </summary>
    public class TaskRepositoryPostgreStd : DapperRepository<Task>, ITaskRepository
    {
        public TaskRepositoryPostgreStd(IDatabase database, IServiceProvider provider) : base(database, provider)
        {
            AllObjectsQuery = @"select t.*, tt.title as starttypetitle, tg.title as grouptitle, s.title as systemtitle
from tm_task t 
left outer join tm_taskgroup tg on t.taskgroupid = tg.taskgroupid
left outer join tm_taskstarttype tt on t.starttypeid = tt.taskstarttypeid
left outer join c_system s on t.systemid = s.systemid";
        }

        public void Deactivation(IEnumerable<int> ids)
        {
            Database.Connection.Execute("update tm_task set isactive=false, lastmodifieddate = now() where taskid = any(@ids)", new { ids }, Database.Transaction);
        }

        public IEnumerable<TaskGroup> GetGroups()
        {
            return Database.Connection.Query<TaskGroup>(@"select * from tm_taskgroup");
        }

        public IEnumerable<TaskStartType> GetStartTypes()
        {
            return Database.Connection.Query<TaskStartType>(@"select * from tm_taskstarttype");
        }

        public bool IsExecuteTask(int id)
        {
            return Database.Connection.QuerySingleOrDefault<bool>(@"
select 1 from tm_task t where t.startfromservice and t.taskid = @id and
not exists (select 1 from tm_taskexecution te where te.taskid = t.taskid and te.finishdate is null)", new { id });
        }

        public IEnumerable<TaskParameter> GetParameters(int id)
        {
            return Database.Connection.Query<TaskParameter>(@"select tp.*, t.title as parenttitle, 
t.class as parentclass, t.method as parentmethod 
from tm_taskparameter tp join tm_task t on tp.parentid = t.taskid where t.taskid = @id", new { id });
        }

        public int CreateParameter(TaskParameter taskparameter)
        {
            return taskparameter.TaskParameterID = Database.Connection.QuerySingle<int>(@"insert into tm_taskparameter(title, sysname, value, parentid, seqno)
values (@title, @sysname, @value, @parentid, @seqno) returning taskparameterid", taskparameter, Database.Transaction);
        }

        public void DeleteParameter(int id)
        {
            Database.Connection.Execute(@"delete from tm_taskparameter where taskparameterid = @id", new { id }, Database.Transaction);
        }

		public IEnumerable<C_System> GetSystemName()
		{
            return Database.Connection.Query<C_System>(@"select * from c_system");
        }
	}

    /// <summary>
    /// Postgre SQL стандартный репозиторий для параметров задания
    /// </summary>
    public class TaskParameterRepositoryPostgreStd : DapperRepository<TaskParameter>, ITaskParameterRepository
    {
        public TaskParameterRepositoryPostgreStd(IDatabase database,IServiceProvider provider) : base(database, provider)
        {
            AllObjectsQuery = @"select tp.*, t.title as parenttitle, 
t.class as parentclass, t.method as parentmethod 
from tm_taskparameter tp join tm_task t on tp.parentid = t.taskid";
        }

        public int MaximumSequenceNumber(int id)
        {
            return Database.Connection.QuerySingle<int>(@"select max(seqno) from tm_taskparameter where parentid = @id", new { id });
        }
    }

    /// <summary>
    /// Postgre SQL стандартный репозиторий для журнала выполнения задания
    /// </summary>
    public class TaskExecutionRepositoryPostgreStd : DapperRepository<TaskExecution>, ITaskExecutionRepository
    {
        public TaskExecutionRepositoryPostgreStd(IDatabase database, IServiceProvider provider) : base(database, provider)
        {
            AllObjectsQuery = @"select te.*, t.title as taskname, u.title as username
from tm_taskexecution te 
join tm_task t on te.taskid = t.taskid
left outer join spm_subject u on te.lastmodifieduserid = u.subjectid";
        }

        public void Clear(DateTime date)
        {
            Database.Connection.Execute("delete from tm_taskexecution where lastmodifieddate < @date", new { date }, Database.Transaction);
        }
    }

    /// <summary>
    /// Postgre SQL стандартный репозиторий для типа задач
    /// </summary>
    public class TaskStartTypeRepositoryPostgreStd : DapperRepository<TaskStartType>
    {
        public TaskStartTypeRepositoryPostgreStd(IDatabase database, IServiceProvider provider) : base(database, provider)
        {
            AllObjectsQuery = "select * from tm_taskstarttype";
        }
    }

    /// <summary>
    /// Postgre SQL стандартный репозиторий для категории задач
    /// </summary>
    public class TaskGroupRepositoryPostgreStd : DapperRepository<TaskGroup>
    {
        public TaskGroupRepositoryPostgreStd(IDatabase database, IServiceProvider provider) : base(database, provider)
        {
            AllObjectsQuery = "select * from tm_taskgroup";
        }
    }
}

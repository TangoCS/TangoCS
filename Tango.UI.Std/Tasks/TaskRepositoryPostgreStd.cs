using System;
using System.Collections.Generic;
using Dapper;
using Tango.Data;

namespace Tango.Tasks
{
    public class TaskRepositoryPostgreStd : ITaskRepository
    {
        public IDatabase database { get; }

        public TaskRepositoryPostgreStd(IDatabase database)
        {
            this.database = database;
        }

        public IRepository<DTO_Task> GetTasks()
        {
            return database.Repository<DTO_Task>().WithAllObjectsQuery(@"select t.*, tt.title as starttypetitle, tg.title as grouptitle
from tm_task t 
left outer join tm_taskgroup tg on t.taskgroupid = tg.taskgroupid
left outer join tm_taskstarttype tt on t.starttypeid = tt.taskstarttypeid");
        }

        public int CreateTask(DTO_Task task)
        {
            return database.Connection.QuerySingle<int>(@"insert into 
tm_task(title, class, starttypeid, method, interval, status, isactive, startfromservice, executiontimeout, taskgroupid)
values (@title, @class, @starttypeid, @method, @interval, @status, @isactive, @startfromservice, @executiontimeout, @taskgroupid) 
returning taskid", task, database.Transaction);
        }

        public void UpdateTask(DTO_Task task)
        {
            database.Connection.ExecuteScalar(@"update tm_task set title=@title, class=@class, starttypeid=@starttypeid, method=@method, interval=@interval, 
isactive=@isactive, executiontimeout=@executiontimeout, taskgroupid=@taskgroupid
where taskid=@taskid", task, database.Transaction);
        }

        public void DeleteTask(IEnumerable<int> ids)
        {
            database.Connection.Execute(@"delete from tm_task where taskid = any(@ids)", new { ids }, database.Transaction);
        }

        public void DeactivationTasks(IEnumerable<int> ids)
        {
            database.Connection.Execute("update tm_task set isactive=false where taskid = any(@ids)", new { ids }, database.Transaction);
        }

        public IRepository<DTO_TaskParameter> GetTaskParameters()
        {
            return database.Repository<DTO_TaskParameter>().WithAllObjectsQuery(@"select tp.*, t.title as parenttitle, 
t.class as parentclass, t.method as parentmethod 
from tm_taskparameter tp join tm_task t on tp.parentid = t.taskid");
        }

        public int CreateTaskParameter(DTO_TaskParameter taskparameter)
        {
            return database.Connection.QuerySingle<int>(@"insert into tm_taskparameter(title, sysname, value, parentid, seqno)
values (@title, @sysname, @value, @parentid, @seqno) returning taskparameterid", taskparameter, database.Transaction);
        }

        public void UpdateTaskParameter(DTO_TaskParameter taskparameter)
        {
            database.Connection.ExecuteScalar(@"update tm_taskparameter set title=@title, sysname=@sysname, value=@value
where taskparameterid=@taskparameterid", taskparameter, database.Transaction);
        }

        public void DeleteTaskParameter(IEnumerable<int> ids)
        {
            database.Connection.Execute(@"delete from tm_taskparameter where taskparameterid = any(@ids)", new { ids }, database.Transaction);
        }

        public IRepository<DTO_TaskExecution> GetTaskExecutions()
        {
            return database.Repository<DTO_TaskExecution>().WithAllObjectsQuery(@"select te.*, t.title as taskname, u.title as username
from tm_taskexecution te 
join tm_task t on te.taskid = t.taskid
left outer join spm_subject u on te.lastmodifieduserid = u.subjectid");
        }

        public void ClearTasksExecution(DateTime date)
        {
            database.Connection.Execute("delete from tm_taskexecution where lastmodifieddate < @date", new { date }, database.Transaction);
        }

        public IEnumerable<DTO_TaskGroup> GetTaskGroups()
        {
            return database.Repository<DTO_TaskGroup>().WithAllObjectsQuery(@"select * from tm_taskgroup").List();
        }

        public IEnumerable<DTO_TaskStartType> GetTaskStartTypes()
        {
            return database.Repository<DTO_TaskStartType>().WithAllObjectsQuery(@"select * from tm_taskstarttype").List();
        }

        public IEnumerable<TaskRunning> TasksRunning()
        {
            return database.Connection.Query<TaskRunning>(@"select te.taskexecutionid, te.startdate, t.executiontimeout, t.taskid
from tm_task t join tm_taskexecution te on t.taskid = te.taskid 
where te.finishdate is null and t.startfromservice");
        }
        public IEnumerable<DTO_Task> TasksForExecute()
        {
            return database.Connection.Query<DTO_Task>(@"select t.* from tm_task t
where t.isactive and t.startfromservice 
and not exists (select 1 from tm_taskexecution te where te.taskid = t.taskid and te.finishdate is null)");
        }

        public int CreateTaskExecution(DTO_TaskExecution execution)
        {
            return database.Connection.QuerySingle<int>(@"
update tm_task set status = 1, laststartdate = now() where taskid = @TaskID;
insert into tm_taskexecution (lastmodifieddate, startdate, machinename, taskid, lastmodifieduserid, issuccessfull)
values (@LastModifiedDate, @StartDate, @MachineName, @TaskID, @LastModifiedUserID, @IsSuccessfull) 
returning taskexecutionid;", execution);
        }

        public void UpdateTaskExecution(DTO_TaskExecution execution)
        {
            database.Connection.ExecuteScalar(@"
update tm_task set status = 2, laststartdate = now() where taskid = @TaskID;
update tm_taskexecution set issuccessfull = @IsSuccessfull, finishdate = @FinishDate, lastmodifieddate = @LastModifiedDate, resultxml = @ResultXml 
where taskexecutionid = @TaskExecutionID;", execution);
        }

        public void UpdateTaskExecutionError(DTO_TaskExecution execution, int errorid)
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
}

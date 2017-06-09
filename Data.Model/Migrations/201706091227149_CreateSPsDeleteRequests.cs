namespace SimpleDispatcher.Data.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSPsDeleteRequests : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("DeleteRequestsNotProcessed",
                p => new
                {
                    RunDate = p.DateTime(),
                    Chunk = p.Int()
                },
                @"
while exists
(
	select top (1)
		Requests.ID
	from Requests with (nolock)
	inner join OperationSettings with (nolock)
	    on Requests.OperationSettings_ID = OperationSettings.ID
	where Requests.Status = 1 --NotProcessed
    and @RunDate > DateAdd(day, OperationSettings.KeepNotProcessedRequestsDuration, Requests.CreatedOn)
)
begin
    delete top(@Chunk) Requests
    from Requests with (rowlock)
    inner join OperationSettings with (nolock)
	    on Requests.OperationSettings_ID = OperationSettings.ID
    where Requests.Status = 1 --NotProcessed
    and @RunDate > DateAdd(day, OperationSettings.KeepNotProcessedRequestsDuration, Requests.CreatedOn)
end
");
            CreateStoredProcedure("DeleteRequestsSucceeded",
                p => new
                {
                    RunDate = p.DateTime(),
                    Chunk = p.Int()
                },
                @"
while exists
(
	select top (1)
		Requests.ID
	from Requests with (nolock)
	inner join OperationSettings with (nolock)
	    on Requests.OperationSettings_ID = OperationSettings.ID
	where Requests.Status = 2 --Succeeded
    and @RunDate > DateAdd(day, OperationSettings.KeepSucceededRequestsDuration, Requests.CreatedOn)
)
begin
    delete top(@Chunk) Requests
    from Requests with (rowlock)
    inner join OperationSettings with (nolock)
        on Requests.OperationSettings_ID = OperationSettings.ID
    where Requests.Status = 2 --Succeeded
    and @RunDate > DateAdd(day, OperationSettings.KeepSucceededRequestsDuration, Requests.CreatedOn)
end
");
            CreateStoredProcedure("DeleteRequestsFailed",
                p => new
                {
                    RunDate = p.DateTime(),
                    Chunk = p.Int()
                },
                @"
while exists
(
	select top (1)
		Requests.ID
	from Requests with (nolock)
	inner join OperationSettings with (nolock)
	    on Requests.OperationSettings_ID = OperationSettings.ID
	where Requests.Status = 3 --Failed
    and @RunDate > DateAdd(day, OperationSettings.KeepFailedRequestsDuration, Requests.CreatedOn)
)
begin
    delete top(@Chunk) Requests
    from Requests with (rowlock)
    inner join OperationSettings with (nolock)
        on Requests.OperationSettings_ID = OperationSettings.ID
    where Requests.Status = 3 --Failed
    and @RunDate > DateAdd(day, OperationSettings.KeepFailedRequestsDuration, Requests.CreatedOn)
end
");
            CreateStoredProcedure("DeleteRequestsRetrying",
                p => new
                {
                    RunDate = p.DateTime(),
                    Chunk = p.Int()
                },
                @"
while exists
(
	select top (1)
		Requests.ID
	from Requests with (nolock)
	inner join OperationSettings with (nolock)
	    on Requests.OperationSettings_ID = OperationSettings.ID
	where Requests.Status = 4 --Retrying
    and @RunDate > DateAdd(day, OperationSettings.KeepRetryingRequestsDuration, Requests.CreatedOn)
)
begin
    delete top(@Chunk) Requests
    from Requests with (rowlock)
    inner join OperationSettings with (nolock) 
        on Requests.OperationSettings_ID = OperationSettings.ID
    where Requests.Status = 4 --Retrying
    and @RunDate > DateAdd(day, OperationSettings.KeepRetryingRequestsDuration, Requests.CreatedOn)
end
");
            CreateStoredProcedure("DeleteRequestsSkipped",
                p => new
                {
                    RunDate = p.DateTime(),
                    Chunk = p.Int()
                },
                @"
while exists
(
	select top (1)
		Requests.ID
	from Requests with (nolock)
	inner join OperationSettings with (nolock)
	    on Requests.OperationSettings_ID = OperationSettings.ID
	where Requests.Status = 5 --Skipped
    and @RunDate > DateAdd(day, OperationSettings.KeepSkippedRequestsDuration, Requests.CreatedOn)
)
begin
    delete top(@Chunk) Requests
    from Requests with (rowlock)
    inner join OperationSettings with (nolock)
        on Requests.OperationSettings_ID = OperationSettings.ID
    where Requests.Status = 5 --Skipped
    and @RunDate > DateAdd(day, OperationSettings.KeepSkippedRequestsDuration, Requests.CreatedOn)
end
");
            CreateStoredProcedure("DeleteRequestsCancelled",
                p => new
                {
                    RunDate = p.DateTime(),
                    Chunk = p.Int()
                },
                @"
while exists
(
	select top (1)
		Requests.ID
	from Requests with (nolock)
	inner join OperationSettings with (nolock)
	    on Requests.OperationSettings_ID = OperationSettings.ID
	where Requests.Status = 6 --Cancelled
    and @RunDate > DateAdd(day, OperationSettings.KeepCancelledRequestsDuration, Requests.CreatedOn)
)
begin
    delete top(@Chunk) Requests
    from Requests with (rowlock)
    inner join OperationSettings with (nolock)
        on Requests.OperationSettings_ID = OperationSettings.ID
    where Requests.Status = 6 --Cancelled
    and @RunDate > DateAdd(day, OperationSettings.KeepCancelledRequestsDuration, Requests.CreatedOn)
end
");
        }

        public override void Down()
        {
            DropStoredProcedure("DeleteRequestsNotProcessed");
            DropStoredProcedure("DeleteRequestsSucceeded");
            DropStoredProcedure("DeleteRequestsFailed");
            DropStoredProcedure("DeleteRequestsRetrying");
            DropStoredProcedure("DeleteRequestsSkipped");
            DropStoredProcedure("DeleteRequestsCancelled");
        }
    }
}

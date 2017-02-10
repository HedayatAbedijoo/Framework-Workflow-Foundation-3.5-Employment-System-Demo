namespace WFTools.Services.Persistence.Ado
{
    /// <summary>
    /// All parameter names used for executing commands against the database.
    /// </summary>
    public enum ParameterNames
    {
        InstanceId, 
        ScopeId, 
        State, 
        Status,
        Unlock, 
        IsBlocked, 
        Info, 
        CurrentOwnerId, 
        OwnerId,
        OwnedUntil, 
        NextTimer, 
        Now, 
        Result, 
        WorkflowIds
    }
}
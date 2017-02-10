namespace WFTools.Services.Persistence.Ado
{
    /// <summary>
    /// All command names used for executing commands against 
    /// the persistence store.
    /// </summary>
    public enum CommandNames
    {
        InsertCompletedScope, 
        RetrieveCompletedScope, 
        InsertInstanceState, 
        RetrieveInstanceState,
        UnlockInstanceState, 
        RetrieveExpiredTimerIds,
        RetrieveNonBlockingInstanceIds
    }
}
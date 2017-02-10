using System;

namespace WFTools.Services.Persistence.Ado.Sql
{
    /// <summary>
    /// SQL Server specific implementation of <see cref="IAdoNameResolver" />.
    /// </summary>
    public class SqlNameResolver : IAdoNameResolver
    {
        /// <summary>
        /// Resolve <see cref="CommandNames" /> to their database-specific command text.
        /// </summary>
        /// <param name="commandName">
        /// A <see cref="CommandNames" /> indicating which command needs to be resolved.
        /// </param>
        public string ResolveCommandName(CommandNames commandName)
        {
            switch (commandName)
            {
                case CommandNames.InsertCompletedScope :
                    return "InsertCompletedScope";
                case CommandNames.InsertInstanceState :
                    return "InsertInstanceState";
                case CommandNames.RetrieveCompletedScope :
                    return "RetrieveCompletedScope";
                case CommandNames.RetrieveInstanceState :
                    return "RetrieveInstanceState";
                case CommandNames.RetrieveExpiredTimerIds :
                    return "RetrieveExpiredTimerIds";
                case CommandNames.RetrieveNonBlockingInstanceIds :
                    return "RetrieveNonBlockingInstanceStateIds";
                case CommandNames.UnlockInstanceState :
                    return "UnlockInstanceState";
            }

            throw new NotSupportedException("Specified CommandNames is not currently supported.");
        }

        /// <summary>
        /// Resolve <see cref="ParameterNames" /> to their database-specific parameter name.
        /// </summary>
        /// <param name="commandName">
        /// A <see cref="CommandNames" /> indicating which command the parameter
        /// name needs to be resolved for.
        /// </param>
        /// <param name="parameterName">
        /// A <see cref="ParameterNames" /> indicating which parameter needs to be resolved.
        /// </param>
        public string ResolveParameterName(CommandNames commandName, ParameterNames parameterName)
        {
            switch (parameterName)
            {
                case ParameterNames.InstanceId :
                    if (commandName == CommandNames.InsertCompletedScope)
                        return "InstanceID";
                    else
                        return "uidInstanceID";

                case ParameterNames.ScopeId :
                    return "completedScopeID";
                    
                case ParameterNames.State :
                    return "state";

                case ParameterNames.Status :
                    return "status";

                case ParameterNames.Unlock :
                    return "unlocked";

                case ParameterNames.IsBlocked :
                    return "blocked";

                case ParameterNames.Info :
                    return "info";

                case ParameterNames.CurrentOwnerId :
                    return "currentOwnerID";

                case ParameterNames.OwnerId :
                    return "ownerID";

                case ParameterNames.OwnedUntil :
                    return "ownedUntil";

                case ParameterNames.NextTimer :
                    return "nextTimer";

                case ParameterNames.Result :
                    return "result";

                case ParameterNames.Now :
                    return "now";

                case ParameterNames.WorkflowIds :
                    return "workflowIds";
            }

            throw new NotSupportedException("Specified CommandNames / ParameterNames combination is not currently supported.");
        }
    }
}

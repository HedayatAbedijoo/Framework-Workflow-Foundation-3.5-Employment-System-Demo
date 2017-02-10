using System;

namespace WFTools.Services.Persistence.Ado.Oracle
{
    /// <summary>
    /// Oracle specific implementation of <see cref="IAdoNameResolver" />.
    /// </summary>
    public class OracleNameResolver : IAdoNameResolver
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
                case CommandNames.InsertCompletedScope:
                    return "WORKFLOW_PKG.InsertCompletedScope";
                case CommandNames.InsertInstanceState:
                    return "WORKFLOW_PKG.InsertInstanceState";
                case CommandNames.RetrieveCompletedScope:
                    return "WORKFLOW_PKG.RetrieveCompletedScope";
                case CommandNames.RetrieveInstanceState:
                    return "WORKFLOW_PKG.RetrieveInstanceState";
                case CommandNames.RetrieveExpiredTimerIds:
                    return "WORKFLOW_PKG.RetrieveExpiredTimerIds";
                case CommandNames.RetrieveNonBlockingInstanceIds:
                    return "WORKFLOW_PKG.RetrieveNonBlockingInstanceIds";
                case CommandNames.UnlockInstanceState:
                    return "WORKFLOW_PKG.UnlockInstanceState";
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
        /// <returns>
        /// </returns>
        public string ResolveParameterName(CommandNames commandName, ParameterNames parameterName)
        {
            switch (parameterName)
            {
                case ParameterNames.InstanceId:
                    return "p_INSTANCE_ID";

                case ParameterNames.ScopeId:
                    return "p_COMPLETED_SCOPE_ID";

                case ParameterNames.State:
                    return "p_STATE";

                case ParameterNames.Status:
                    return "p_STATUS";

                case ParameterNames.Unlock:
                    return "p_UNLOCKED";

                case ParameterNames.IsBlocked:
                    return "p_BLOCKED";

                case ParameterNames.Info:
                    return "p_INFO";

                case ParameterNames.CurrentOwnerId:
                    return "p_CURRENT_OWNER_ID";

                case ParameterNames.OwnerId:
                    return "p_OWNER_ID";

                case ParameterNames.OwnedUntil:
                    return "p_OWNED_UNTIL";

                case ParameterNames.NextTimer:
                    return "p_NEXT_TIMER";

                case ParameterNames.Result:
                    return "p_RESULT";

                case ParameterNames.Now:
                    return "p_NOW";

                case ParameterNames.WorkflowIds :
                    return "p_WORKFLOW_IDS";
            }

            throw new NotSupportedException("Specified CommandNames / ParameterNames combination is not currently supported.");
        }
    }
}

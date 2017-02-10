using System;
using WFTools.Services.Persistence.Ado;

namespace WFTools.Services.MySql.Persistence
{
    /// <summary>
    /// MySQL specific implementation of <see cref="IAdoNameResolver" />.
    /// </summary>
    public class MySqlNameResolver : IAdoNameResolver
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
                    return "InsertCompletedScope";
                case CommandNames.InsertInstanceState:
                    return "InsertInstanceState";
                case CommandNames.RetrieveCompletedScope:
                    return "RetrieveCompletedScope";
                case CommandNames.RetrieveInstanceState:
                    return "RetrieveInstanceState";
                case CommandNames.RetrieveExpiredTimerIds:
                    return "RetrieveExpiredTimerIds";
                case CommandNames.RetrieveNonBlockingInstanceIds:
                    return "RetrieveNonBlockingInstanceIds";
                case CommandNames.UnlockInstanceState:
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

                case ParameterNames.WorkflowIds:
                    return "p_WORKFLOW_IDS";
            }

            throw new NotSupportedException("Specified CommandNames / ParameterNames combination is not currently supported.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Transactions;
using System.Workflow.Runtime;
using WFTools.Services.Common.Ado;

namespace WFTools.Services.Persistence.Ado
{
    /// <summary>
    /// Generic implementation of <see cref="IResourceAccessor" /> that utilises an 
    /// <see cref="IAdoResourceProvider" /> for obtaining the resources
    /// it needs to communicate with the persistence store.
    /// </summary>
    public class AdoResourceAccessor : IResourceAccessor, IDisposable
    {
        public AdoResourceAccessor(IAdoResourceProvider resourceProvider,
            IAdoNameResolver nameResolver, IAdoValueReader valueReader)
            : this(resourceProvider, nameResolver, valueReader, null) { }

        public AdoResourceAccessor(IAdoResourceProvider resourceProvider,
            IAdoNameResolver nameResolver, IAdoValueReader valueReader,
            Transaction transaction)
        {
            if (resourceProvider == null)
                throw new ArgumentNullException("resourceProvider");

            if (nameResolver == null)
                throw new ArgumentNullException("nameResolver");

            if (valueReader == null)
                throw new ArgumentNullException("valueReader");

            this.resourceProvider = resourceProvider;
            this.nameResolver = nameResolver;
            this.valueReader = valueReader;

            if (transaction == null)
            {
                this.dbConnection = resourceProvider.CreateConnection();
                this.dbConnection.Open();
            }
            else
                this.dbConnection = resourceProvider.CreateEnlistedConnection(transaction);
        }

        /// <summary>
        /// The resource provider used to create resources for connecting to
        /// and manipulating the persistence store.
        /// </summary>
        private IAdoResourceProvider resourceProvider;

        /// <summary>
        /// The <see cref="IAdoNameResolver" /> responsible for resolving names of 
        /// stored procedures and parameters for a particular persistence store.
        /// </summary>
        private IAdoNameResolver nameResolver;

        /// <summary>
        /// The <see cref="IAdoValueReader" /> responsible for reading values from
        /// <see cref="IDbCommand" /> and <see cref="IDataReader" /> implementations.
        /// </summary>
        private IAdoValueReader valueReader;

        /// <summary>
        /// The database connection used to connect to the persistence store.
        /// </summary>
        private DbConnection dbConnection;

        /// <summary>
        /// Insert a new completed scope into the persistence store.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="scopeId"></param>
        /// <param name="state"></param>
        public void InsertCompletedScope(Guid instanceId, Guid scopeId, byte[] state)
        {
            using (DbCommand dbCommand = resourceProvider.CreateCommand(dbConnection, nameResolver.ResolveCommandName(CommandNames.InsertCompletedScope), CommandType.StoredProcedure))
            {
                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertCompletedScope,
                        ParameterNames.InstanceId), instanceId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertCompletedScope,
                    ParameterNames.ScopeId), scopeId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertCompletedScope,
                    ParameterNames.State), state, AdoDbType.Binary);

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieve a completed scope from the persistence store.
        /// </summary>
        /// <param name="scopeId"></param>
        /// <returns></returns>
        public byte[] RetrieveCompletedScope(Guid scopeId)
        {
            byte[] completedScope = null;

            using (DbCommand dbCommand = resourceProvider.CreateCommand(this.dbConnection, nameResolver.ResolveCommandName(CommandNames.RetrieveCompletedScope), CommandType.StoredProcedure))
            {
                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveCompletedScope,
                        ParameterNames.ScopeId), scopeId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveCompletedScope,
                        ParameterNames.Result), AdoDbType.Int32, ParameterDirection.Output);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveCompletedScope,
                        ParameterNames.State), AdoDbType.Cursor, ParameterDirection.Output);

                int? result;
                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                        completedScope = (byte[])dataReader.GetValue(0);

                    result = valueReader.GetNullableInt32(dbCommand,
                        nameResolver.ResolveParameterName(
                            CommandNames.RetrieveInstanceState,
                            ParameterNames.Result));
                }

                if (completedScope == null && result > 0)
                {
                    // scope could not be found
                    throw new WorkflowPersistenceException(string.Format(
                        "Scope with identifier {0} could not be loaded.", scopeId));
                }
            }

            return completedScope;
        }

        /// <summary>
        /// Insert instance state into the persistence store.
        /// </summary>
        /// <param name="workItem"></param>
        /// <param name="ownerId"></param>
        /// <param name="ownedUntil"></param>
        public void InsertInstanceState(PendingWorkItem workItem, Guid ownerId, DateTime ownedUntil)
        {
            using (DbCommand dbCommand = resourceProvider.CreateCommand(dbConnection, nameResolver.ResolveCommandName(CommandNames.InsertInstanceState), CommandType.StoredProcedure))
            {
                string instanceParameter = nameResolver.ResolveParameterName(
                    CommandNames.InsertInstanceState, ParameterNames.InstanceId);

                string resultParameter = nameResolver.ResolveParameterName(
                    CommandNames.InsertInstanceState, ParameterNames.Result);

                resourceProvider.AddParameter(dbCommand,
                    instanceParameter, workItem.InstanceId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertInstanceState,
                        ParameterNames.State), workItem.SerialisedActivity, AdoDbType.Binary);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertInstanceState,
                        ParameterNames.Status), workItem.Status, AdoDbType.Int32);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertInstanceState,
                        ParameterNames.Unlock), workItem.Unlock, AdoDbType.Boolean);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertInstanceState,
                        ParameterNames.IsBlocked), workItem.IsBlocked, AdoDbType.Boolean);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertInstanceState,
                        ParameterNames.Info), workItem.Info, AdoDbType.String);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertInstanceState,
                        ParameterNames.OwnerId), ownerId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertInstanceState,
                        ParameterNames.OwnedUntil), ownedUntil, AdoDbType.DateTime);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertInstanceState,
                        ParameterNames.NextTimer), workItem.NextTimer, AdoDbType.DateTime);

                resourceProvider.AddParameter(dbCommand,
                    resultParameter, AdoDbType.Int32, ParameterDirection.Output);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.InsertInstanceState,
                        ParameterNames.CurrentOwnerId), AdoDbType.Guid, ParameterDirection.Output);

                dbCommand.ExecuteNonQuery();

                checkResult(dbCommand, resultParameter, instanceParameter);
            }
        }

        /// <summary>
        /// Retrieve instance state from the persistence store.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="ownerId"></param>
        /// <param name="ownedUntil"></param>
        /// <returns></returns>
        public byte[] RetrieveInstanceState(Guid instanceId, Guid ownerId, DateTime ownedUntil)
        {
            byte[] instanceState = null;

            using (DbCommand dbCommand = resourceProvider.CreateCommand(this.dbConnection, nameResolver.ResolveCommandName(CommandNames.RetrieveInstanceState), CommandType.StoredProcedure))
            {
                string instanceParameter = nameResolver.ResolveParameterName(
                    CommandNames.InsertInstanceState, ParameterNames.InstanceId);

                string resultParameter = nameResolver.ResolveParameterName(
                    CommandNames.InsertInstanceState, ParameterNames.Result);

                resourceProvider.AddParameter(dbCommand,
                    instanceParameter, instanceId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveInstanceState,
                    ParameterNames.OwnerId), ownerId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveInstanceState,
                    ParameterNames.OwnedUntil), ownedUntil, AdoDbType.DateTime);

                resourceProvider.AddParameter(dbCommand,
                    resultParameter, AdoDbType.Int32, ParameterDirection.Output);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveInstanceState,
                    ParameterNames.CurrentOwnerId), AdoDbType.Guid, ParameterDirection.Output);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveInstanceState,
                        ParameterNames.State), AdoDbType.Cursor, ParameterDirection.Output);

                int? result;
                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                        instanceState = (byte[])valueReader.GetValue(dataReader, 0);

                    result = valueReader.GetNullableInt32(dbCommand, resultParameter);
                }

                if (instanceState == null && result > 0)
                {
                    // workflow could not be found
                    WorkflowPersistenceException e =
                        new WorkflowPersistenceException(string.Format(
                        "Instance with identifier {0} could not be loaded.", instanceId));

                    e.Data["WorkflowNotFound"] = true;
                }
                else
                    checkResult(dbCommand, resultParameter, instanceParameter);
            }

            return instanceState;
        }

        /// <summary>
        /// Unlocks an instance in the persistence store.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="ownerId"></param>
        public void UnlockInstanceState(Guid instanceId, Guid ownerId)
        {
            using (DbCommand dbCommand = resourceProvider.CreateCommand(dbConnection, nameResolver.ResolveCommandName(CommandNames.UnlockInstanceState), CommandType.StoredProcedure))
            {
                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.UnlockInstanceState,
                        ParameterNames.InstanceId), instanceId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.UnlockInstanceState,
                        ParameterNames.OwnerId), ownerId, AdoDbType.Guid);

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieve a list of all expired workflow identifiers.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="ownedUntil"></param>
        /// <returns></returns>
        public IList<Guid> RetrieveExpiredTimerIds(Guid ownerId, DateTime ownedUntil)
        {
            List<Guid> expiredWorkflowIds = new List<Guid>();
            using (DbCommand dbCommand = resourceProvider.CreateCommand(dbConnection, nameResolver.ResolveCommandName(CommandNames.RetrieveExpiredTimerIds), CommandType.StoredProcedure))
            {
                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveExpiredTimerIds,
                        ParameterNames.OwnerId), ownerId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveExpiredTimerIds,
                        ParameterNames.OwnedUntil), ownedUntil, AdoDbType.DateTime);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveExpiredTimerIds,
                        ParameterNames.Now), DateTime.UtcNow, AdoDbType.DateTime);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveExpiredTimerIds,
                        ParameterNames.WorkflowIds), AdoDbType.Cursor, ParameterDirection.Output);

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                        expiredWorkflowIds.Add(valueReader.GetGuid(dataReader, 0));
                }
            }

            return expiredWorkflowIds;
        }

        /// <summary>
        /// Retrieve a list of all workflow identifiers whose ownership has expired.
        /// </summary>
        /// <param name="ownerId">
        /// <see cref="Guid" /> representing new owner's identifier.
        /// </param>
        /// <param name="ownedUntil">
        /// <see cref="DateTime" /> indicating when the new ownership expires.
        /// </param>
        /// <returns>
        /// List of all Guids matching the criteria.
        /// </returns>
        public IList<Guid> RetrieveNonBlockedInstanceIds(Guid ownerId, DateTime ownedUntil)
        {
            List<Guid> workflowIds = new List<Guid>();
            using (DbCommand dbCommand = resourceProvider.CreateCommand(dbConnection, nameResolver.ResolveCommandName(CommandNames.RetrieveNonBlockingInstanceIds), CommandType.StoredProcedure))
            {
                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveNonBlockingInstanceIds,
                        ParameterNames.OwnerId), ownerId, AdoDbType.Guid);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveNonBlockingInstanceIds,
                        ParameterNames.OwnedUntil), ownedUntil, AdoDbType.DateTime);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveNonBlockingInstanceIds,
                        ParameterNames.Now), DateTime.UtcNow, AdoDbType.DateTime);

                resourceProvider.AddParameter(dbCommand,
                    nameResolver.ResolveParameterName(CommandNames.RetrieveNonBlockingInstanceIds,
                        ParameterNames.WorkflowIds), AdoDbType.Cursor, ParameterDirection.Output);

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                        workflowIds.Add(valueReader.GetGuid(dataReader, 0));
                }
            }

            return workflowIds;
        }

        private const int ownershipError = -2;

        /// <summary>
        /// Check to see whether the result of a persistence operation
        /// was successful, raise appropriate exceptions if not.
        /// </summary>
        /// <param name="dbCommand">
        /// <see cref="DbCommand" /> to retrieve result information from.
        /// </param>
        /// <param name="resultParameter">
        /// Name of the parameter storing the result.
        /// </param>
        /// <param name="instanceParameter">
        /// Name of the parameter storing the instance identifier.
        /// </param>
        private void checkResult(DbCommand dbCommand, string resultParameter, string instanceParameter)
        {
            int? result = valueReader.GetNullableInt32(dbCommand, resultParameter);
            if (result == ownershipError)
            {
                Guid instanceId = valueReader.GetGuid(dbCommand, instanceParameter);

                throw new WorkflowOwnershipException(instanceId);
            }
        }

        ///<summary>
        /// Close down any database connection and perform associated clean-up.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.dbConnection != null)
                dbConnection.Dispose();
        }
    }
}
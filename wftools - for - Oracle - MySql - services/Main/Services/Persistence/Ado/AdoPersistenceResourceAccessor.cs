using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Transactions;
using System.Workflow.Runtime;
using WFTools.Services.Common.Ado;
using WFTools.Services.Persistence;

namespace WFTools.Services.Persistence.Ado
{
    /// <summary>
    /// Generic implementation of <see cref="IPersistenceResourceAccessor" /> that utilises an 
    /// <see cref="IAdoResourceProvider" /> for obtaining the resources
    /// it needs to communicate with the persistence store.
    /// </summary>
    public class AdoPersistenceResourceAccessor : IPersistenceResourceAccessor
    {
        /// <summary>
        /// Construct a new <see cref="AdoPersistenceResourceAccessor" /> with the
        /// specified <see cref="IAdoResourceProvider" />, 
        /// <see cref="IPersistenceNameResolver" /> and <see cref="IAdoValueReader" /> 
        /// </summary>
        /// <param name="resourceProvider">
        /// An <see cref="IAdoResourceProvider" /> used to provide resources for
        /// accessing the tracking store.
        /// </param>
        /// <param name="nameResolver">
        /// An <see cref="IPersistenceNameResolver" /> that resolves names
        /// of commands and parameters for the relevant tracking store.
        /// </param>
        /// <param name="valueReader">
        /// An <see cref="IAdoValueReader" /> that reads values from
        /// <see cref="IDbCommand" /> and <see cref="IDataReader" /> implementations.
        /// </param>
        public AdoPersistenceResourceAccessor(IAdoResourceProvider resourceProvider,
            IPersistenceNameResolver nameResolver, IAdoValueReader valueReader)
            : this(resourceProvider, nameResolver, valueReader, null) { }

        /// <summary>
        /// Construct a new <see cref="AdoPersistenceResourceAccessor" /> with the
        /// specified <see cref="IAdoResourceProvider" />, 
        /// <see cref="IPersistenceNameResolver" /> and <see cref="IAdoValueReader" /> 
        /// All work should be performed in the specified <see cref="Transaction" />.
        /// </summary>
        /// <param name="resourceProvider">
        /// An <see cref="IAdoResourceProvider" /> used to provide resources for
        /// accessing the tracking store.
        /// </param>
        /// <param name="nameResolver">
        /// An <see cref="IPersistenceNameResolver" /> that resolves names
        /// of commands and parameters for the relevant tracking store.
        /// </param>
        /// <param name="valueReader">
        /// An <see cref="IAdoValueReader" /> that reads values from
        /// <see cref="IDbCommand" /> and <see cref="IDataReader" /> implementations.
        /// </param>
        /// <param name="transaction">
        /// An <see cref="Transaction" /> in which to perform the work.
        /// </param>
        public AdoPersistenceResourceAccessor(IAdoResourceProvider resourceProvider,
            IPersistenceNameResolver nameResolver, IAdoValueReader valueReader,
            Transaction transaction)
        {
            if (resourceProvider == null)
                throw new ArgumentNullException("resourceProvider");

            if (nameResolver == null)
                throw new ArgumentNullException("nameResolver");

            if (valueReader == null)
                throw new ArgumentNullException("valueReader");

            _resourceProvider = resourceProvider;
            _nameResolver = nameResolver;
            _valueReader = valueReader;

            if (transaction == null)
            {
                _isConnectionOwner = true;
                _dbConnection = resourceProvider.CreateConnection();
                _dbConnection.Open();
            }
            else
                _dbConnection = resourceProvider.CreateEnlistedConnection(transaction, out _isConnectionOwner);
        }

        /// <summary>
        /// The resource provider used to create resources for connecting to
        /// and manipulating the persistence store.
        /// </summary>
        private readonly IAdoResourceProvider _resourceProvider;

        /// <summary>
        /// The <see cref="IPersistenceNameResolver" /> responsible for resolving names of 
        /// stored procedures and parameters for a particular persistence store.
        /// </summary>
        private readonly IPersistenceNameResolver _nameResolver;

        /// <summary>
        /// The <see cref="IAdoValueReader" /> responsible for reading values from
        /// <see cref="IDbCommand" /> and <see cref="IDataReader" /> implementations.
        /// </summary>
        private readonly IAdoValueReader _valueReader;

        /// <summary>
        /// The database connection used to connect to the persistence store.
        /// </summary>
        private readonly DbConnection _dbConnection;

        /// <summary>
        /// Indicates whether we own the database connection to the persistence store.
        /// </summary>
        private readonly Boolean _isConnectionOwner;

        /// <summary>
        /// Insert a new completed scope into the persistence store.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="scopeId"></param>
        /// <param name="state"></param>
        public void InsertCompletedScope(Guid instanceId, Guid scopeId, Byte[] state)
        {
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(PersistenceCommandName.InsertCompletedScope), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertCompletedScope, 
                    PersistenceParameterName.InstanceId), instanceId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertCompletedScope,
                    PersistenceParameterName.ScopeId), scopeId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertCompletedScope,
                    PersistenceParameterName.State), state, AdoDbType.Binary);

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieve a completed scope from the persistence store.
        /// </summary>
        /// <param name="scopeId"></param>
        /// <returns></returns>
        public Byte[] RetrieveCompletedScope(Guid scopeId)
        {
            Byte[] completedScope = null;

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(PersistenceCommandName.RetrieveCompletedScope), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveCompletedScope,
                    PersistenceParameterName.ScopeId), scopeId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveCompletedScope,
                    PersistenceParameterName.Result), AdoDbType.Int32,
                    ParameterDirection.Output);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveCompletedScope,
                    PersistenceParameterName.State), AdoDbType.Cursor,
                    ParameterDirection.Output);

                Int32? result;
                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                        completedScope = (Byte[])_valueReader.GetValue(dataReader, 0);

                    result = _valueReader.GetNullableInt32(dbCommand,
                        _nameResolver.ResolveParameterName(
                            PersistenceCommandName.RetrieveInstanceState,
                            PersistenceParameterName.Result));
                }

                if (completedScope == null && result > 0)
                {
                    // scope could not be found
                    throw new PersistenceException(RM.Get_Error_ScopeCouldNotBeLoaded(scopeId));
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
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(PersistenceCommandName.InsertInstanceState), CommandType.StoredProcedure))
            {
                String instanceParameter = _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.InstanceId);

                String resultParameter = _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.Result);

                AddParameter(dbCommand, instanceParameter, 
                    workItem.InstanceId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.State), workItem.SerialisedActivity,
                    AdoDbType.Binary);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.Status), workItem.Status, AdoDbType.Int32);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.Unlock), workItem.Unlock, AdoDbType.Boolean);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.IsBlocked), workItem.IsBlocked,
                    AdoDbType.Boolean);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.Info), workItem.Info, AdoDbType.Text);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.OwnerId), ownerId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.OwnedUntil), ownedUntil.ToUniversalTime(), 
                    AdoDbType.DateTime);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.NextTimer), workItem.NextTimer,
                    AdoDbType.DateTime);

                AddParameter(dbCommand, resultParameter, 
                    AdoDbType.Int32, ParameterDirection.Output);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState,
                    PersistenceParameterName.CurrentOwnerId), AdoDbType.Guid,
                    ParameterDirection.Output);

                dbCommand.ExecuteNonQuery();

                CheckResult(dbCommand, resultParameter, instanceParameter);
            }
        }

        /// <summary>
        /// Retrieve instance state from the persistence store.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="ownerId"></param>
        /// <param name="ownedUntil"></param>
        /// <returns></returns>
        public Byte[] RetrieveInstanceState(Guid instanceId, Guid ownerId, DateTime ownedUntil)
        {
            Byte[] instanceState = null;

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(PersistenceCommandName.RetrieveInstanceState), CommandType.StoredProcedure))
            {
                String instanceParameter = _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState, 
                    PersistenceParameterName.InstanceId);

                String resultParameter = _nameResolver.ResolveParameterName(
                    PersistenceCommandName.InsertInstanceState, 
                    PersistenceParameterName.Result);

                AddParameter(dbCommand, instanceParameter, instanceId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveInstanceState, 
                    PersistenceParameterName.OwnerId), ownerId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveInstanceState,
                    PersistenceParameterName.OwnedUntil), ownedUntil, AdoDbType.DateTime);

                AddParameter(dbCommand, resultParameter, 
                    AdoDbType.Int32, ParameterDirection.Output);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveInstanceState,
                    PersistenceParameterName.CurrentOwnerId), AdoDbType.Guid,
                    ParameterDirection.Output);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveInstanceState,
                    PersistenceParameterName.State), AdoDbType.Cursor,
                    ParameterDirection.Output);

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                        instanceState = (Byte[])_valueReader.GetValue(dataReader, 0);
                }

                CheckResult(dbCommand, resultParameter, instanceParameter);
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
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(PersistenceCommandName.UnlockInstanceState), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.UnlockInstanceState,
                    PersistenceParameterName.InstanceId), instanceId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.UnlockInstanceState,
                    PersistenceParameterName.OwnerId), ownerId, AdoDbType.Guid);

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
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(PersistenceCommandName.RetrieveExpiredTimerIds), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveExpiredTimerIds,
                    PersistenceParameterName.OwnerId), ownerId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveExpiredTimerIds,
                    PersistenceParameterName.OwnedUntil), ownedUntil,
                    AdoDbType.DateTime);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveExpiredTimerIds,
                    PersistenceParameterName.Now), DateTime.UtcNow,
                    AdoDbType.DateTime);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveExpiredTimerIds,
                    PersistenceParameterName.WorkflowIds), AdoDbType.Cursor,
                    ParameterDirection.Output);

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                        expiredWorkflowIds.Add(_valueReader.GetGuid(dataReader, 0));
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
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(PersistenceCommandName.RetrieveNonBlockingInstanceIds), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveNonBlockingInstanceIds,
                    PersistenceParameterName.OwnerId), ownerId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveNonBlockingInstanceIds, 
                    PersistenceParameterName.OwnedUntil), ownedUntil, AdoDbType.DateTime);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveNonBlockingInstanceIds,
                    PersistenceParameterName.Now), DateTime.UtcNow, AdoDbType.DateTime);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    PersistenceCommandName.RetrieveNonBlockingInstanceIds,
                    PersistenceParameterName.WorkflowIds), AdoDbType.Cursor, ParameterDirection.Output);

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                        workflowIds.Add(_valueReader.GetGuid(dataReader, 0));
                }
            }

            return workflowIds;
        }

        private const Int32 _ownershipError = -2;
        private const Int32 _instanceNotFoundError = -1;

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
        private void CheckResult(DbCommand dbCommand, String resultParameter, String instanceParameter)
        {
            Int32? result = _valueReader.GetNullableInt32(dbCommand, resultParameter);
            Guid instanceId = _valueReader.GetGuid(dbCommand, instanceParameter);
            switch (result)
            {
                case _ownershipError:
                    // ownership error
                    throw new WorkflowOwnershipException(instanceId);
                case _instanceNotFoundError:
                    // workflow could not be found
                    PersistenceException e = new PersistenceException(RM.Get_Error_InstanceCouldNotBeLoaded(instanceId));
                    e.Data["WorkflowNotFound"] = true;
                    throw e;
            }
        }

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.CreateCommand(DbConnection,string)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbCommand CreateCommand(String commandText)
        {
            return _resourceProvider.CreateCommand(_dbConnection, commandText);
        }

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.CreateCommand(DbConnection,string,CommandType)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbCommand CreateCommand(String commandText, CommandType commandType)
        {
            return _resourceProvider.CreateCommand(_dbConnection, commandText, commandType);
        }

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.AddParameter(DbCommand,string,object,AdoDbType)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbParameter AddParameter(DbCommand dbCommand, String name, object value, AdoDbType type)
        {
            return _resourceProvider.AddParameter(dbCommand, name, value, type);
        }

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.AddParameter(DbCommand,string,object,AdoDbType,ParameterDirection)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbParameter AddParameter(DbCommand dbCommand, String name, object value, AdoDbType type, ParameterDirection direction)
        {
            return _resourceProvider.AddParameter(dbCommand, name, value, type, direction);
        }

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.AddParameter(DbCommand,string,AdoDbType,ParameterDirection)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbParameter AddParameter(DbCommand dbCommand, String name, AdoDbType type, ParameterDirection direction)
        {
            return _resourceProvider.AddParameter(dbCommand, name, type, direction);
        }

        ///<summary>
        /// Close down any database connection and perform associated clean-up.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ///<summary>
        /// Close down any database connection and perform associated clean-up.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (_isConnectionOwner && _dbConnection != null)
                    _dbConnection.Dispose();
            }
        }
    }
}
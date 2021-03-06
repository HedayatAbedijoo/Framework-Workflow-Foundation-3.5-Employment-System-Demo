using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Transactions;
using WFTools.Services.Common.Ado;
using WFTools.Utilities.Diagnostics;

namespace WFTools.Services.Batching.Ado
{
    /// <summary>
    /// Implementation of <see cref="GenericWorkBatchService" /> that provides
    /// shared connection support for other ADO-based services. This prevents
    /// MSDTC being used in situations where two services share the same connection
    /// string.
    /// 
    /// In addition it also supports using local transactions in databases
    /// that do not currently support promotable single phase enlistment. This
    /// prevents MSDTC becoming involved when only a single connection is in use.
    /// <remarks>
    /// Although this service allows the sharing of connections where two or more
    /// services share a connection string, it does not prevent services from
    /// registering with different connection strings. By doing so we can support
    /// services that access many databases and still allow them to share existing
    /// connections where possible.
    /// </remarks>
    /// </summary>
    public class AdoWorkBatchService : GenericWorkBatchService
    {
        /// <summary>
        /// Create a new <see cref="AdoWorkBatchService" />.
        /// </summary>
        /// <remarks>
        /// Use this overload if your database supports promotable single 
        /// phase enlistments (PSPE) or if you intend to use multiple 
        /// connections in your workflow services. This overload will use
        /// distributed transactions where necessary.
        /// </remarks>
        public AdoWorkBatchService() : this(false) { }

        /// <summary>
        /// Create a new <see cref="AdoWorkBatchService" /> and register
        /// an <see cref="IPromotableSinglePhaseNotification" /> to prevent 
        /// promotion to a distributed transaction.
        /// </summary>
        /// <param name="useLocalTransactions">
        /// Indicates whether to use local transactions, preventing
        /// promotion to a distributed transaction.
        /// </param>
        /// <remarks>
        /// This overload should only be used when one connection 
        /// (i.e. same connection string) is shared by multiple services 
        /// *AND* the ADO provider does not support the use of promotable 
        /// single phase enlistments. Examples include Oracle and SQL 
        /// Server 2000.
        /// </remarks>
        public AdoWorkBatchService(Boolean useLocalTransactions)
        {
            _useLocalTransactions = useLocalTransactions;
        }

        /// <summary>
        /// Called just after a transaction is created for this workbatch.
        /// </summary>
        /// <param name="transaction">
        /// The transaction that was created.
        /// </param>
        protected override void TransactionCreated(Transaction transaction)
        {
            TraceHelper.Trace();

            if (!_connectionsByTransaction.ContainsKey(transaction))
                _connectionsByTransaction.Add(transaction, new Dictionary<String, DbConnection>());
        }

        /// <summary>
        /// Called after the workbatch is committed.
        /// </summary>
        protected override void WorkBatchCommitted(Transaction transaction)
        {
            TraceHelper.Trace();

            CleanupTransaction(transaction);
        }

        /// <summary>
        /// Called after the workbatch has rolled back.
        /// </summary>
        protected override void WorkBatchRolledback(Transaction transaction)
        {
            TraceHelper.Trace();

            CleanupTransaction(transaction);
        }

        /// <summary>
        /// Clean-up after a transaction has been committed or rolledback. Disposes of all
        /// connections used by the transaction during its execution.
        /// </summary>
        /// <param name="transaction">
        /// <see cref="Transaction" /> to clean-up after.
        /// </param>
        private void CleanupTransaction(Transaction transaction)
        {
            if (_connectionsByTransaction.ContainsKey(transaction))
            {
                Dictionary<String, DbConnection> connectionsByConnectionString = _connectionsByTransaction[transaction];
                foreach (String connectionString in connectionsByConnectionString.Keys)
                    connectionsByConnectionString[connectionString].Dispose();

                _connectionsByTransaction.Remove(transaction);
            }            
        }

        /// <summary>
        /// Indicates that we need to enlist a 
        /// <see cref="IPromotableSinglePhaseNotification" /> to prevent 
        /// <see cref="TransactionScope" /> from promoting a transaction.
        /// </summary>
        private readonly Boolean _useLocalTransactions;

        /// <summary>
        /// The <see cref="SinglePhaseOnlyTransactionAdapter"/> that
        /// facilitates the use of local transactions.
        /// </summary>
        private SinglePhaseOnlyTransactionAdapter _transactionAdapter;

        /// <summary>
        /// Dictionary containing connections, indexed by connection string
        /// and then by transaction so that we can efficiently implement
        /// connection sharing.
        /// </summary>
        private readonly Dictionary<Transaction, Dictionary<String, DbConnection>> _connectionsByTransaction = new Dictionary<Transaction, Dictionary<String, DbConnection>>();

        /// <summary>
        /// Lock used to allow thread-safe creation of resource providers.
        /// </summary>
        private readonly Object _resourceProviderLock = new Object();

        /// <summary>
        /// Dictionary containing resource providers indexed by connection String.
        /// </summary>
        private readonly Dictionary<String, IAdoResourceProvider> _resourceProvidersByConnectionString = new Dictionary<String, IAdoResourceProvider>();

        /// <summary>
        /// Creates an <see cref="IAdoResourceProvider" /> that deals with connection
        /// sharing where a number of workflow services share the same connection
        /// String.
        /// </summary>
        /// <param name="connectionStringSettings">
        /// <see cref="ConnectionStringSettings" /> containing the connection string settings.
        /// </param>
        /// <returns>
        /// An <see cref="IAdoResourceProvider" /> implementation.
        /// </returns>
        public IAdoResourceProvider CreateResourceProvider(ConnectionStringSettings connectionStringSettings)
        {
            String connectionStringKey = String.Format("{0}_{1}",
                connectionStringSettings.ProviderName,
                connectionStringSettings.ConnectionString);

            IAdoResourceProvider resourceProvider;
            if (!_resourceProvidersByConnectionString.TryGetValue(connectionStringKey, out resourceProvider))
            {
                lock (_resourceProviderLock)
                {
                    if (!_resourceProvidersByConnectionString.TryGetValue(connectionStringKey, out resourceProvider))
                    {
                        resourceProvider = new AdoResourceProviderWrapper(this, connectionStringSettings);
                        _resourceProvidersByConnectionString.Add(connectionStringKey, resourceProvider);
                    }
                }
            }

            return resourceProvider;
        }

        /// <summary>
        /// Implementation of <see cref="IPromotableSinglePhaseNotification" /> that
        /// prevents a <see cref="TransactionScope" /> from promoting a local 
        /// transaction into a distributed transaction.
        /// 
        /// This allows us to avoid the use of distributed transactions in situtations
        /// where we're connecting to databases that do not support promotable
        /// single phase enlistment (PSPE) such as Oracle and SQL 2000.
        /// </summary>
        /// <remarks>
        /// Based on the code provided by Jim Johnson @ http://www.pluralsight.com/blogs/jimjohn/archive/2005/09/13/14795.aspx.
        /// </remarks>
        private class SinglePhaseOnlyTransactionAdapter : IPromotableSinglePhaseNotification
        {
            /// <summary>
            /// Construct a new <see cref="SinglePhaseOnlyTransactionAdapter" />
            /// using the specified connection to create a local transaction.
            /// </summary>
            /// <param name="dbConnection">
            /// A <see cref="DbConnection" /> that we can use
            /// to create a local transaction.
            /// </param>
            public SinglePhaseOnlyTransactionAdapter(DbConnection dbConnection)
            {
                if (dbConnection == null)
                    throw new ArgumentNullException("dbConnection");

                _dbConnection = dbConnection;
            }

            /// <summary>
            /// Connection that we can retrieve transactions from.
            /// </summary>
            private readonly DbConnection _dbConnection;

            private DbTransaction _dbTransaction;
            /// <summary>
            /// Local transaction created from the connection.
            /// </summary>
            public DbTransaction DbTransaction
            {
                get { return _dbTransaction; }
            }

            /// <summary>
            /// Transaction in which we are enlisted.
            /// </summary>
            private Transaction _enlistedTransaction;

            ///<summary>
            /// Called when we have successfully enlisted in the transaction.
            ///</summary>
            /// <remarks>
            /// Here we start our database transaction and make a record of 
            /// the transaction in which we have been enlisted.
            /// </remarks>
            void IPromotableSinglePhaseNotification.Initialize()
            {
                _enlistedTransaction = Transaction.Current;
                _dbTransaction = _dbConnection.BeginTransaction();
            }

            ///<summary>
            /// Attempt to commit the internal transaction.
            ///</summary>
            void IPromotableSinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
            {
                try
                {
                    // attempt to commit the enlistment
                    _dbTransaction.Commit();
                    singlePhaseEnlistment.Committed();
                }
                catch (Exception e)
                {
                    // something went wrong, make sure the transaction aborts
                    singlePhaseEnlistment.Aborted(e);
                    throw;
                }
                finally
                {
                    // clean-up our resources
                    Dispose();
                }
            }

            ///<summary>
            /// Attempt to rollback the internal transaction.
            ///</summary>
            void IPromotableSinglePhaseNotification.Rollback(SinglePhaseEnlistment singlePhaseEnlistment)
            {
                try
                {
                    // attempt to rollback the transaction
                    _dbTransaction.Rollback();
                    singlePhaseEnlistment.Done();
                }
                finally
                {
                    // clean-up our resources
                    Dispose();
                }
            }

            ///<summary>
            /// We don't support promotion, always throws a NotSupportedException.
            ///</summary>
            byte[] ITransactionPromoter.Promote()
            {
                throw new NotSupportedException(RM.Get_Error_PromotionNotSupported());
            }

            /// <summary>
            /// Enlist ourselves into the ambient transaction.
            /// </summary>
            public void Begin()
            {
                if (_enlistedTransaction == null)
                {
                    if (Transaction.Current == null)
                    {
                        // no ambient transaction to enlist in
                        throw new InvalidOperationException(RM.Get_Error_CannotEnlistInNonTransactionalContext());
                    }

                    if (!Transaction.Current.EnlistPromotableSinglePhase(this))
                    {
                        // cannot enlist the adapter in a distributed transaction
                        throw new InvalidOperationException(RM.Get_Error_CannotEnlistInDistributedTransaction());
                    }
                }
                else if (Transaction.Current != _enlistedTransaction)
                {
                    // cannot enlist an adapter in more than one transaction
                    throw new InvalidOperationException(RM.Get_Error_AdapterAlreadyEnlisted());
                }
            }

            public void Dispose()
            {
                // clean-up our transaction
                if (_dbTransaction != null)
                {
                    _dbTransaction.Dispose();
                    _dbTransaction = null;
                }
            }
        }

        /// <summary>
        /// Implementation of <see cref="IAdoResourceProvider" /> that creates
        /// an inner <see cref="IAdoResourceProvider" />. When enlisting a connection
        /// in a transaction this resource provider first checks to see whether the
        /// parent <see cref="AdoWorkBatchService" /> has an existing connection
        /// associated with the transaction. If that's the case then it will re-use
        /// the connection. All other operations delegate to the inner resource provider.
        /// </summary>
        private class AdoResourceProviderWrapper : IAdoResourceProvider
        {
            public AdoResourceProviderWrapper(AdoWorkBatchService workBatchService, ConnectionStringSettings connectionStringSettings)
            {
                _workBatchService = workBatchService;
                _innerResourceProvider = AdoResourceProviderFactory.Create(connectionStringSettings);
            }

            /// <summary>
            /// The parent <see cref="AdoWorkBatchService" /> that created the instance.
            /// </summary>
            private readonly AdoWorkBatchService _workBatchService;

            /// <summary>
            /// Inner <see cref="IAdoResourceProvider" /> that is used to perform
            /// the majority of operations exposed by the interface.
            /// </summary>
            private readonly IAdoResourceProvider _innerResourceProvider;

            /// <summary>
            /// Initialise the resource provider with connection String details.
            /// </summary>
            /// <param name="connectionStringSettings">
            /// The connection string settings to initialise with.
            /// </param>
            public void Initialise(ConnectionStringSettings connectionStringSettings)
            {
                // do nothing - provided by our inner resource provider
            }

            /// <summary>
            /// The connection String used to connect to the underlhing database.
            /// </summary>
            public String ConnectionString
            {
                get
                {
                    if (_innerResourceProvider == null)
                        throw new NotSupportedException(RM.Get_Error_ResourceProviderNotInitialised());

                    return _innerResourceProvider.ConnectionString;
                }
            }

            /// <summary>
            /// The name of the ADO.NET provider for which we provide resources for.
            /// </summary>
            public String ProviderName
            {
                get
                {
                    if (_innerResourceProvider == null)
                        throw new NotSupportedException(RM.Get_Error_ResourceProviderNotInitialised());

                    return _innerResourceProvider.ProviderName;
                }
            }

            /// <summary>
            /// Creates an ADO connection and enlists it in the specified transaction.
            /// </summary>
            /// <param name="transaction">
            /// The <see cref="Transaction" /> in which to enlist a new connection in.
            /// </param>
            /// <param name="shouldClose">
            /// Indicates whether the caller should close the connection or 
            /// whether it is managed by an external entity.
            /// </param>
            /// <returns>
            /// A <see cref="DbConnection" /> that is enlisted in the specified
            /// <see cref="Transaction" />.
            /// </returns>
            public DbConnection CreateEnlistedConnection(Transaction transaction, out Boolean shouldClose)
            {
                if (transaction == null)
                    throw new ArgumentNullException("transaction");

                if (_innerResourceProvider == null)
                    throw new NotSupportedException(RM.Get_Error_ResourceProviderNotInitialised());

                if (!_workBatchService._connectionsByTransaction.ContainsKey(transaction))
                    throw new ArgumentNullException(RM.Get_Error_InvalidTransactionSpecified());

                DbConnection dbConnection;
                Dictionary<String, DbConnection> connectionsByConnectionString = _workBatchService._connectionsByTransaction[transaction];
                if (!connectionsByConnectionString.TryGetValue(_innerResourceProvider.ConnectionString, out dbConnection))
                {
                    if (_workBatchService._useLocalTransactions)
                    {
                        // we're using local transactions - create a new
                        // SinglePhaseOnlyTransactionAdapter to create a local
                        // transaction and prevent it from being promoted to
                        // a distributed transaction
                        try
                        {
                            dbConnection = _innerResourceProvider.CreateConnection();
                            dbConnection.Open();

                            // can only do this once (additional enlistments will fail)
                            _workBatchService._transactionAdapter = new SinglePhaseOnlyTransactionAdapter(dbConnection);
                            _workBatchService._transactionAdapter.Begin();
                        }
                        catch
                        {
                            if (dbConnection != null)
                                dbConnection.Dispose();

                            throw;
                        }
                    }
                    else
                        dbConnection = _innerResourceProvider.CreateEnlistedConnection(transaction, out shouldClose);

                    connectionsByConnectionString.Add(_innerResourceProvider.ConnectionString, dbConnection);
                }

                shouldClose = false;

                return dbConnection;
            }

            /// <summary>
            /// Creates an ADO connection initialised using the specified
            /// </summary>
            public DbConnection CreateConnection()
            {
                if (_innerResourceProvider == null)
                    throw new NotSupportedException(RM.Get_Error_ResourceProviderNotInitialised());

                return _innerResourceProvider.CreateConnection();
            }

            /// <summary>
            /// Creates an ADO command initialised with the specified connection
            /// and command text. This uses the default command type.
            /// </summary>
            public DbCommand CreateCommand(DbConnection dbConnection, String commandText)
            {
                if (_innerResourceProvider == null)
                    throw new NotSupportedException(RM.Get_Error_ResourceProviderNotInitialised());

                return AssociateCommandWithTransaction(
                    _innerResourceProvider.CreateCommand(dbConnection,
                        commandText));
            }

            /// <summary>
            /// Creates an ADO command initialised with the specified connection, 
            /// command text and type.
            /// </summary>
            public DbCommand CreateCommand(DbConnection dbConnection, String commandText, CommandType commandType)
            {
                if (_innerResourceProvider == null)
                    throw new NotSupportedException(RM.Get_Error_ResourceProviderNotInitialised());

                return AssociateCommandWithTransaction(
                    _innerResourceProvider.CreateCommand(dbConnection,
                        commandText, commandType));
            }

            /// <summary>
            /// Helper method that associates a command with a transaction if
            /// local transactions are enabled and there is an active transaction
            /// adapter for the current connection.
            /// </summary>
            private DbCommand AssociateCommandWithTransaction(DbCommand dbCommand)
            {
                if (_workBatchService._useLocalTransactions &&
                    _workBatchService._transactionAdapter != null &&
                    _workBatchService._transactionAdapter.DbTransaction != null &&
                    _workBatchService._transactionAdapter.DbTransaction.Connection == dbCommand.Connection)
                {
                    dbCommand.Transaction = _workBatchService._transactionAdapter.DbTransaction;
                }

                return dbCommand;
            }

            /// <summary>
            /// Add an input parameter initialised with the value and type to the 
            /// specified <see cref="DbCommand" />.
            /// </summary>
            public DbParameter AddParameter(DbCommand dbCommand, String name, Object value, AdoDbType type)
            {
                if (_innerResourceProvider == null)
                    throw new NotSupportedException(RM.Get_Error_ResourceProviderNotInitialised());

                return _innerResourceProvider.AddParameter(dbCommand, name, value, type);
            }

            /// <summary>
            /// Add a parameter initialised with the value, type and direction to 
            /// the specified <see cref="DbCommand" />.
            /// </summary>
            public DbParameter AddParameter(DbCommand dbCommand, String name, Object value, AdoDbType type, ParameterDirection direction)
            {
                if (_innerResourceProvider == null)
                    throw new NotSupportedException(RM.Get_Error_ResourceProviderNotInitialised());

                return _innerResourceProvider.AddParameter(dbCommand, name, value, type, direction);
            }

            /// <summary>
            /// Add a parameter initialised with the type and direction to the
            /// specified <see cref="DbCommand" />.
            /// </summary>
            public DbParameter AddParameter(DbCommand dbCommand, String name, AdoDbType type, ParameterDirection direction)
            {
                if (_innerResourceProvider == null)
                    throw new NotSupportedException(RM.Get_Error_ResourceProviderNotInitialised());

                return _innerResourceProvider.AddParameter(dbCommand, name, type, direction);
            }
        }
    }
}
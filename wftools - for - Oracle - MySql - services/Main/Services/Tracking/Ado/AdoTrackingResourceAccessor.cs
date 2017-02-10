using System;
using System.Data;
using System.Data.Common;
using System.Transactions;
using WFTools.Services.Common.Ado;
using WFTools.Services.Common.State;

namespace WFTools.Services.Tracking.Ado
{
    /// <summary>
    /// Default implementation of all the resource accessors used for retrieving
    /// and manipulating data from the tracking store. Other implementations may 
    /// provide custom batching functionality for their particular providers.
    /// </summary>
    /// <remarks>
    /// This class is implemented as a number of partial classes to increase readability.
    /// </remarks>
    public partial class AdoTrackingResourceAccessor : ITrackingServiceResourceAccessor, 
        ITrackingChannelResourceAccessor, ITrackingProfileResourceAccessor, 
        ITrackingQueryResourceAccessor
    {
        /// <summary>
        /// Construct a new <see cref="AdoTrackingResourceAccessor" /> with the
        /// specified <see cref="IAdoResourceProvider" />, 
        /// <see cref="ITrackingNameResolver" /> and <see cref="IAdoValueReader" />.
        /// </summary>
        /// <param name="resourceProvider">
        /// An <see cref="IAdoResourceProvider" /> used to provide resources for
        /// accessing the tracking store.
        /// </param>
        /// <param name="nameResolver">
        /// An <see cref="ITrackingNameResolver" /> that resolves names
        /// of commands and parameters for the relevant tracking store.
        /// </param>
        /// <param name="valueReader">
        /// An <see cref="IAdoValueReader" /> that reads values from
        /// <see cref="IDbCommand" /> and <see cref="IDataReader" /> implementations.
        /// </param>
        public AdoTrackingResourceAccessor(IAdoResourceProvider resourceProvider,
            ITrackingNameResolver nameResolver, IAdoValueReader valueReader) : 
            this(resourceProvider, nameResolver, valueReader, null, null) { }

        /// <summary>
        /// Construct a new <see cref="AdoTrackingResourceAccessor" /> with the
        /// specified <see cref="IAdoResourceProvider" />, 
        /// <see cref="ITrackingNameResolver" />, <see cref="IAdoValueReader" /> 
        /// and <see cref="IStateProvider" />. All work should be performed in
        /// the specified <see cref="Transaction" />.
        /// </summary>
        /// <param name="resourceProvider">
        /// An <see cref="IAdoResourceProvider" /> used to provide resources for
        /// accessing the tracking store.
        /// </param>
        /// <param name="nameResolver">
        /// An <see cref="ITrackingNameResolver" /> that resolves names
        /// of commands and parameters for the relevant tracking store.
        /// </param>
        /// <param name="valueReader">
        /// An <see cref="IAdoValueReader" /> that reads values from
        /// <see cref="IDbCommand" /> and <see cref="IDataReader" /> implementations.
        /// </param>
        /// <param name="transaction">
        /// An <see cref="Transaction" /> in which to perform the work.
        /// </param>
        /// <param name="stateProvider">
        /// An <see cref="IStateProvider" /> that can be used to store state.
        /// </param>
        public AdoTrackingResourceAccessor(IAdoResourceProvider resourceProvider,
            ITrackingNameResolver nameResolver, IAdoValueReader valueReader,
            Transaction transaction, IStateProvider stateProvider)
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
            _stateProvider = stateProvider;

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
        /// The <see cref="ITrackingNameResolver" /> responsible for resolving names of 
        /// stored procedures and parameters for a particular persistence store.
        /// </summary>
        private readonly ITrackingNameResolver _nameResolver;

        private readonly IAdoValueReader _valueReader;
        /// <summary>
        /// The <see cref="IAdoValueReader" /> responsible for reading values from
        /// <see cref="IDbCommand" /> and <see cref="IDataReader" /> implementations.
        /// </summary>
        protected IAdoValueReader ValueReader
        {
            get { return _valueReader; }
        }

        /// <summary>
        /// The database connection used to connect to the tracking store.
        /// </summary>
        private readonly DbConnection _dbConnection;

        /// <summary>
        /// Indicates whether we own the database connection to the tracking store.
        /// </summary>
        private readonly Boolean _isConnectionOwner;

        /// <summary>
        /// The <see cref="IStateProvider" /> used for storing the state of the accessor.
        /// </summary>
        private readonly IStateProvider _stateProvider;

        /// <summary>
        /// Indicates an 'empty' version.
        /// </summary>
        private static readonly Version emptyVersion = new Version(0, 0, 0, 0);

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

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.CreateCommand(DbConnection,String)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbCommand CreateCommand(String commandText)
        {
            return _resourceProvider.CreateCommand(_dbConnection, commandText);
        }

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.CreateCommand(DbConnection,String,CommandType)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbCommand CreateCommand(String commandText, CommandType commandType)
        {
            return _resourceProvider.CreateCommand(_dbConnection, commandText, commandType);
        }

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.AddParameter(DbCommand,String,object,AdoDbType)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbParameter AddParameter(DbCommand dbCommand, String name, object value, AdoDbType type)
        {
            return _resourceProvider.AddParameter(dbCommand, name, value, type);
        }

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.AddParameter(DbCommand,String,object,AdoDbType,ParameterDirection)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbParameter AddParameter(DbCommand dbCommand, String name, object value, AdoDbType type, ParameterDirection direction)
        {
            return _resourceProvider.AddParameter(dbCommand, name, value, type, direction);
        }

        /// <summary>
        /// Thin wrapper around
        /// <see cref="IAdoResourceProvider.AddParameter(DbCommand,String,AdoDbType,ParameterDirection)" />
        /// that provides the connection to use for derived implementations.
        /// </summary>
        protected DbParameter AddParameter(DbCommand dbCommand, String name, AdoDbType type, ParameterDirection direction)
        {
            return _resourceProvider.AddParameter(dbCommand, name, type, direction);
        }
    }
}

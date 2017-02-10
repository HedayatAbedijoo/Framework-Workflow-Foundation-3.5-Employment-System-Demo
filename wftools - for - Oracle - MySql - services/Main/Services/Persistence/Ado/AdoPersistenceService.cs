using System;
using System.Configuration;
using System.Data;
using System.Transactions;
using WFTools.Services.Batching.Ado;
using WFTools.Services.Common.Ado;

namespace WFTools.Services.Persistence.Ado
{
    /// <summary>
    /// Concrete implementation of <see cref="GenericPersistenceService" />
    /// that uses the ADO.NET provider architecture for persisting workflows to
    /// any database supported by ADO.NET.
    /// </summary>
    public class AdoPersistenceService : GenericPersistenceService
    {
        /// <summary>
        /// Construct a new <see cref="AdoPersistenceService" />
        /// with the specified connection string settings.
        /// <param name="connectionStringSettings">
        /// Connection string settings for the ADO provider.
        /// </param>
        /// </summary>
        public AdoPersistenceService(ConnectionStringSettings connectionStringSettings) : 
            this(connectionStringSettings, false, TimeSpan.Zero, TimeSpan.Zero) { }

        /// <summary>
        /// Construct a new <see cref="AdoPersistenceService" />
        /// with the specified connection string settings and the 
        /// specified configuration parameters.
        /// </summary>
        /// <param name="connectionStringSettings">
        /// Connection string settings for the ADO provider.
        /// </param>
        /// <param name="unloadOnIdle">
        /// Indicates whether to unload idle workflow instances.
        /// </param>
        /// <param name="instanceOwnershipDuration">
        /// The timeout before the ownership of a workflow instance is invalidated.
        /// </param>
        /// <param name="loadInterval">
        /// The interval to check for workflow instances with expired timers.
        /// </param>
        /// </summary>
        public AdoPersistenceService(ConnectionStringSettings connectionStringSettings, 
            Boolean unloadOnIdle, TimeSpan instanceOwnershipDuration, TimeSpan loadInterval) : 
            base(unloadOnIdle, instanceOwnershipDuration, loadInterval)
        {
            if (connectionStringSettings == null)
                throw new ArgumentNullException("connectionStringSettings");

            _connectionStringSettings = connectionStringSettings;
        }

        /// <summary>
        /// Perform startup duties associated with this persistence service.
        /// <remarks>
        /// This implementation calls a virtual method to create a single 
        /// <see cref="IPersistenceNameResolver" /> and <see cref="IAdoValueReader" />
        /// for this persistence service.
        /// </remarks>
        /// </summary>
        protected override void Start()
        {
            _nameResolver = CreateNameResolver();
            _valueReader = CreateValueReader();
            _workBatchService = Runtime.GetService<AdoWorkBatchService>();

            base.Start();
        }

        /// <summary>
        /// The connection string settings used to initialise the resource provider.
        /// </summary>
        private readonly ConnectionStringSettings _connectionStringSettings;

        /// <summary>
        /// The active <see cref="IPersistenceNameResolver" /> that resolves
        /// names necessary for manipulating the underlying persistence store.
        /// </summary>
        private IPersistenceNameResolver _nameResolver;

        /// <summary>
        /// The active <see cref="IAdoValueReader" /> that reads values
        /// from <see cref="IDbCommand" /> and <see cref="IDataReader" />
        /// objects.
        /// </summary>
        private IAdoValueReader _valueReader;

        /// <summary>
        /// An instance of the <see cref="AdoWorkBatchService" /> or <c>null</c>
        /// if one isn't present in the runtime.
        /// </summary>
        private AdoWorkBatchService _workBatchService;

        /// <summary>
        /// Create an <see cref="IAdoResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying ADO persistence store.
        /// </summary>
        protected override IResourceProvider CreateResourceProvider()
        {
            if (_workBatchService != null)
                return _workBatchService.CreateResourceProvider(_connectionStringSettings);

            return AdoResourceProviderFactory.Create(_connectionStringSettings);
        }

        /// <summary>
        /// Create an <see cref="IPersistenceNameResolver" /> that resolves names
        /// of commands and parameters for the relevant persistence store.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="IPersistenceNameResolver" /> appropriate for the 
        /// persistence store.
        /// </returns>
        protected virtual IPersistenceNameResolver CreateNameResolver()
        {
            return PersistenceNameResolverFactory.Create(_connectionStringSettings.ProviderName);
        }

        /// <summary>
        /// Create an <see cref="IAdoValueReader" /> that reads values from
        /// <see cref="IDbCommand" /> and <see cref="IDataReader" /> implementations.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="IAdoValueReader" /> appropriate for the 
        /// persistence store.
        /// </returns>
        protected virtual IAdoValueReader CreateValueReader()
        {
            return AdoValueReaderFactory.Create(_connectionStringSettings.ProviderName);
        }

        /// <summary>
        /// Create an <see cref="IPersistenceResourceAccessor" /> that is responsible
        /// for manipulating the underlying persistence store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        protected override IPersistenceResourceAccessor CreateAccessor(IResourceProvider resourceProvider)
        {
            return CreateAccessor(resourceProvider, null);
        }

        /// <summary>
        /// Create an <see cref="IPersistenceResourceAccessor" /> that is responsible
        /// for manipulating the underlying persistence store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        /// <param name="transaction">
        /// Transaction to perform operations within.
        /// </param>
        protected override IPersistenceResourceAccessor CreateAccessor(IResourceProvider resourceProvider, Transaction transaction)
        {
            IAdoResourceProvider adoResourceProvider = resourceProvider as IAdoResourceProvider;
            if (adoResourceProvider == null)
                throw new ArgumentException(RM.Get_Error_NotIAdoResourceProvider());

            return new AdoPersistenceResourceAccessor(
                adoResourceProvider, _nameResolver, _valueReader,
                transaction);
        }
    }
}
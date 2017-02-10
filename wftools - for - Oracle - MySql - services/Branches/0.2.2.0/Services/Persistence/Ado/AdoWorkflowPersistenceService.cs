using System;
using System.Configuration;
using System.Data;
using System.Transactions;
using WFTools.Services.Common.Ado;

namespace WFTools.Services.Persistence.Ado
{
    /// <summary>
    /// Concrete implementation of <see cref="GenericWorkflowPersistenceService" />
    /// that uses the ADO.NET provider architecture for persisting workflows to
    /// any database supported by ADO.NET.
    /// </summary>
    public class AdoWorkflowPersistenceService : GenericWorkflowPersistenceService
    {
        /// <summary>
        /// Construct a new <see cref="AdoWorkflowPersistenceService" />
        /// with the specified connection string settings.
        /// </summary>
        /// <param name="connectionStringSettings">
        /// Connection string settings for the ADO provider.
        /// </param>
        public AdoWorkflowPersistenceService(ConnectionStringSettings connectionStringSettings) : 
            this(connectionStringSettings, false, TimeSpan.Zero, TimeSpan.Zero) { }

        /// <summary>
        /// Construct a new <see cref="AdoWorkflowPersistenceService" />
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
        public AdoWorkflowPersistenceService(ConnectionStringSettings connectionStringSettings, 
            bool unloadOnIdle, TimeSpan instanceOwnershipDuration, TimeSpan loadInterval) : 
            base(unloadOnIdle, instanceOwnershipDuration, loadInterval)
        {
            if (connectionStringSettings == null)
                throw new ArgumentNullException("connectionStringSettings");

            this.connectionStringSettings = connectionStringSettings;
        }

        /// <summary>
        /// Perform startup duties associated with this persistence service.
        /// <remarks>
        /// This implementation calls a virtual method to create a single 
        /// <see cref="IAdoNameResolver" /> and <see cref="IAdoValueReader" />
        /// for this persistence service.
        /// </remarks>
        /// </summary>
        protected override void Start()
        {
            this.nameResolver = CreateNameResolver();
            this.valueReader = CreateValueReader();

            base.Start();
        }

        internal const string ConfigurationSectionName = "wftools.services.persistence.ado";

        /// <summary>
        /// The connection string settings used to initialise the resource provider.
        /// </summary>
        private ConnectionStringSettings connectionStringSettings;

        /// <summary>
        /// The active <see cref="IAdoNameResolver" /> that resolves
        /// names necessary for manipulating the underlying persistence store.
        /// </summary>
        private IAdoNameResolver nameResolver;

        /// <summary>
        /// The active <see cref="IAdoValueReader" /> that reads values
        /// from <see cref="IDbCommand" /> and <see cref="IDataReader" />
        /// objects.
        /// </summary>
        private IAdoValueReader valueReader;

        /// <summary>
        /// Create an <see cref="IAdoResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying ADO persistence store.
        /// </summary>
        protected override IResourceProvider CreateResourceProvider()
        {
            string providerName = connectionStringSettings.ProviderName;

            // grab the resource provider from the factory
            IAdoResourceProvider resourceProvider =
                AdoResourceProviderFactory.Create(ConfigurationSectionName,
                    providerName);

            if (resourceProvider != null)
            {
                // initialise it with our connection string settings
                resourceProvider.Initialise(connectionStringSettings);
            }

            return resourceProvider;
        }

        /// <summary>
        /// Create an <see cref="IAdoNameResolver" /> that resolves names
        /// of commands and parameters for the relevant persistence store.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="IAdoNameResolver" /> appropriate for the 
        /// persistence store.
        /// </returns>
        protected virtual IAdoNameResolver CreateNameResolver()
        {
            return AdoNameResolverFactory.Create(connectionStringSettings.ProviderName);
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
            return AdoValueReaderFactory.Create(ConfigurationSectionName,
                connectionStringSettings.ProviderName);
        }

        /// <summary>
        /// Create an <see cref="IResourceAccessor" /> that is responsible
        /// for manipulating the underlying persistence store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        protected override IResourceAccessor CreateAccessor(IResourceProvider resourceProvider)
        {
            return CreateAccessor(resourceProvider, null);
        }

        /// <summary>
        /// Create an <see cref="IResourceAccessor" /> that is responsible
        /// for manipulating the underlying persistence store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        /// <param name="transaction">
        /// Transaction to perform operations within.
        /// </param>
        protected override IResourceAccessor CreateAccessor(IResourceProvider resourceProvider, Transaction transaction)
        {
            if (!(resourceProvider is IAdoResourceProvider))
                throw new ArgumentException("Resource provider is not an IAdoResourceProvider");

            return new AdoResourceAccessor(
                (IAdoResourceProvider) resourceProvider,
                nameResolver, valueReader, transaction);
        }
    }
}
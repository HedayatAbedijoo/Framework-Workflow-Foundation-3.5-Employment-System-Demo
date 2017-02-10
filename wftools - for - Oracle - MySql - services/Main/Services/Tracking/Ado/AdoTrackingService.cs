using System;
using System.Configuration;
using System.Data;
using System.Workflow.Runtime.Tracking;
using WFTools.Services.Batching.Ado;
using WFTools.Services.Common.Ado;

namespace WFTools.Services.Tracking.Ado
{
    /// <summary>
    /// Concrete implementation of <see cref="GenericTrackingService" />
    /// that uses the ADO.NET provider architecture for tracking workflow events
    /// using any database supported by ADO.NET.
    /// </summary>
    public class AdoTrackingService : GenericTrackingService
    {
        /// <summary>
        /// Construct a new <see cref="AdoTrackingService" /> with the 
        /// specified connection string settings.
        /// </summary>
        /// <param name="connectionStringSettings">
        /// <see cref="ConnectionStringSettings" /> for the ADO provider.
        /// </param>
        public AdoTrackingService(ConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings == null)
                throw new ArgumentNullException("connectionStringSettings");

            _connectionStringSettings = connectionStringSettings;
        }

        /// <summary>
        /// Construct a new <see cref="AdoTrackingService" /> with the 
        /// specified connection string settings and the interval in which 
        /// to poll for profile changes.
        /// </summary>
        /// <param name="connectionStringSettings">
        /// <see cref="ConnectionStringSettings" /> for the ADO provider.
        /// </param>
        /// <param name="profileChangeInterval">
        /// <see cref="TimeSpan" /> representing the interval to poll for profile
        /// changes.
        /// </param>
        public AdoTrackingService(ConnectionStringSettings connectionStringSettings, TimeSpan profileChangeInterval) : base(profileChangeInterval)
        {
            if (connectionStringSettings == null)
                throw new ArgumentNullException("connectionStringSettings");

            _connectionStringSettings = connectionStringSettings;
        }

        /// <summary>
        /// The connection string settings used to initialise an 
        /// <see cref="IAdoResourceProvider" />.
        /// </summary>
        private readonly ConnectionStringSettings _connectionStringSettings;

        /// <summary>
        /// The active <see cref="ITrackingNameResolver" /> that resolves
        /// names necessary for manipulating the underlying tracking store.
        /// </summary>
        private ITrackingNameResolver _nameResolver;

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
        /// Perform startup duties associated with this persistence service.
        /// <remarks>
        /// This implementation calls a virtual method to create a single 
        /// <see cref="ITrackingNameResolver" /> and <see cref="IAdoValueReader" />
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
        /// Create an <see cref="IResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying tracking store.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="IAdoResourceProvider" /> appropriate 
        /// for the <see cref="AdoTrackingService" /> tracking store.
        /// </returns>
        /// <remarks>
        /// The resource provider is created once upon Start of the 
        /// tracking service.
        /// </remarks>
        protected override IResourceProvider CreateResourceProvider()
        {
            if (_workBatchService != null)
                return _workBatchService.CreateResourceProvider(_connectionStringSettings);

            return AdoResourceProviderFactory.Create(_connectionStringSettings);
        }

        /// <summary>
        /// Create an <see cref="ITrackingNameResolver" /> that resolves names
        /// of commands and parameters for the relevant <see cref="AdoTrackingService" />
        /// persistence store.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="ITrackingNameResolver" /> appropriate 
        /// for the <see cref="AdoTrackingService" /> tracking store.
        /// </returns>
        protected virtual ITrackingNameResolver CreateNameResolver()
        {
            return TrackingNameResolverFactory.Create(_connectionStringSettings.ProviderName);
        }

        /// <summary>
        /// Create an <see cref="IAdoValueReader" /> that reads values from
        /// <see cref="IDbCommand" /> and <see cref="IDataReader" /> implementations.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="IAdoValueReader" /> appropriate for the 
        /// tracking service's persistence store.
        /// </returns>
        protected virtual IAdoValueReader CreateValueReader()
        {
            return AdoValueReaderFactory.Create(_connectionStringSettings.ProviderName);
        }

        /// <summary>
        /// Create an <see cref="ITrackingServiceResourceAccessor" /> that is responsible
        /// for manipulating the underlying tracking store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        protected override ITrackingServiceResourceAccessor CreateAccessor(IResourceProvider resourceProvider)
        {
            IAdoResourceProvider adoResourceProvider = resourceProvider as IAdoResourceProvider;
            if (adoResourceProvider == null)
                throw new ArgumentException(RM.Get_Error_NotIAdoResourceProvider());

            return AdoTrackingResourceAccessorFactory.Create(
                adoResourceProvider, _nameResolver, _valueReader);
        }

        ///<summary>
        ///Must be overridden in the derived class, and when implemented, returns the channel that the runtime tracking infrastructure uses to send tracking records to the tracking service.
        ///</summary>
        ///
        ///<returns>
        ///The <see cref="T:System.Workflow.Runtime.Tracking.TrackingChannel"></see> that is used to send tracking records to the tracking service.
        ///</returns>
        ///
        ///<param name="parameters">The <see cref="T:System.Workflow.Runtime.Tracking.TrackingParameters"></see> associated with the workflow instance.</param>
        protected override TrackingChannel GetTrackingChannel(TrackingParameters parameters)
        {
            IAdoResourceProvider resourceProvider = ResourceProvider as IAdoResourceProvider;
            if (resourceProvider == null)
                throw new ArgumentException(RM.Get_Error_NotIAdoResourceProvider());

            return new AdoTrackingChannel(resourceProvider, _valueReader, _nameResolver, parameters);
        }
    }
}

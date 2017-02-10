using System;
using System.Configuration;
using System.Data;
using WFTools.Services.Common.Ado;

namespace WFTools.Services.Tracking.Ado
{
    /// <summary>
    /// Concrete implementation of <see cref="GenericTrackingQueryManager" />
    /// that uses the ADO.NET provider architecture for querying workflow
    /// tracking information in any database supported by ADO.NET.
    /// </summary>
    public class AdoTrackingQueryManager : GenericTrackingQueryManager
    {
        /// <summary>
        /// Construct a new <see cref="AdoTrackingQueryManager" />
        /// with the specified connection string settings.
        /// </summary>
        /// <param name="connectionStringSettings">
        /// <see cref="ConnectionStringSettings" /> for the ADO provider.
        /// </param>
        public AdoTrackingQueryManager(ConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings == null)
                throw new ArgumentNullException("connectionStringSettings");

            _connectionStringSettings = connectionStringSettings;
        }

        /// <summary>
        /// <see cref="ConnectionStringSettings" /> used to initialise the resource provider.
        /// </summary>
        private readonly ConnectionStringSettings _connectionStringSettings;

        private ITrackingNameResolver _nameResolver;
        /// <summary>
        /// The active <see cref="ITrackingNameResolver" /> that resolves
        /// names necessary for manipulating the underlying tracking store.
        /// </summary>
        private ITrackingNameResolver nameResolver
        {
            get
            {
                if (_nameResolver == null)
                    _nameResolver = CreateNameResolver();

                return _nameResolver;
            }
        }

        private IAdoValueReader _valueReader;
        /// <summary>
        /// The active <see cref="IAdoValueReader" /> that reads values
        /// from <see cref="IDbCommand" /> and <see cref="IDataReader" /> objects.
        /// </summary>
        private IAdoValueReader valueReader
        {
            get
            {
                if (_valueReader == null)
                    _valueReader = CreateValueReader();

                return _valueReader;
            }
        }

        /// <summary>
        /// Create an <see cref="IResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying tracking store.
        /// </summary>
        /// <remarks>
        /// The resource provider is created upon first access of the 
        /// <see cref="GenericTrackingQueryManager.resourceProvider" /> property.
        /// </remarks>
        protected override IResourceProvider CreateResourceProvider()
        {
            return AdoResourceProviderFactory.Create(_connectionStringSettings);
        }

        /// <summary>
        /// Create an <see cref="ITrackingQueryResourceAccessor" /> that is responsible
        /// for querying the underlying tracking store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        /// <returns>
        /// A <see cref="ITrackingQueryResourceAccessor" />.
        /// </returns>
        protected override ITrackingQueryResourceAccessor CreateAccessor(IResourceProvider resourceProvider)
        {
            IAdoResourceProvider adoResourceProvider = resourceProvider as IAdoResourceProvider;
            if (adoResourceProvider == null)
                throw new ArgumentException(RM.Get_Error_NotIAdoResourceProvider());

            return AdoTrackingResourceAccessorFactory.Create(
                adoResourceProvider, nameResolver, valueReader);
        }

        /// <summary>
        /// Create an <see cref="ITrackingNameResolver" /> that resolves names
        /// of commands and parameters for the relevant persistence store.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="ITrackingNameResolver" /> appropriate for the 
        /// tracking store.
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
        /// tracking store.
        /// </returns>
        protected virtual IAdoValueReader CreateValueReader()
        {
            return AdoValueReaderFactory.Create(_connectionStringSettings.ProviderName);
        }
    }
}

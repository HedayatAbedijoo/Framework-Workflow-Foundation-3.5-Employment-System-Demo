using WFTools.Services.Common.Ado.Configuration;
using WFTools.Utilities;

namespace WFTools.Services.Persistence.Ado
{
    /// <summary>
    /// Factory for creating <see cref="IAdoNameResolver" /> implementations.
    /// </summary>
    public static class AdoNameResolverFactory
    {
        /// <summary>
        /// Given a provider name locate the necessary <see cref="IAdoNameResolver" /> 
        /// in the configuration file.
        /// </summary>
        /// <param name="providerName">
        /// The name that uniquely identifies an ADO.NET provider.
        /// </param>
        /// <returns>
        /// An <see cref="IAdoNameResolver" />.
        /// </returns>
        public static IAdoNameResolver Create(string providerName)
        {
            // locate any mappings for the specified provider
            ProviderNameTypeMapping mapping =
                DbProviderSettings.Get(AdoWorkflowPersistenceService.ConfigurationSectionName)
                    .NameResolvers.FindByProviderName(providerName);

            IAdoNameResolver nameResolver = null;
            if (mapping != null)
                nameResolver = TypeUtilities.CreateInstance<IAdoNameResolver>(mapping.Type);

            return nameResolver;
        }
    }
}

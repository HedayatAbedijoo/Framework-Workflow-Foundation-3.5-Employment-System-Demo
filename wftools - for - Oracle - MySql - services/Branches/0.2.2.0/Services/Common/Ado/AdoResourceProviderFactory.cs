using WFTools.Services.Common.Ado;
using WFTools.Services.Common.Ado.Configuration;
using WFTools.Utilities;

namespace WFTools.Services.Persistence.Ado
{
    /// <summary>
    /// Factory for creating <see cref="IAdoResourceProvider" /> implementations.
    /// </summary>
    public static class AdoResourceProviderFactory
    {
        /// <summary>
        /// Given a provider name either locate the necessary <see cref="IAdoResourceProvider" /> 
        /// in the configuration file or return the default (<see cref="DefaultAdoResourceProvider" />).
        /// </summary>
        /// <param name="sectionName">
        /// Configuration section to retrieve data from.
        /// </param>
        /// <param name="providerName">
        /// The name that uniquely identifies an ADO.NET provider.
        /// </param>
        /// <returns>
        /// An <see cref="IAdoResourceProvider" />.
        /// </returns>
        public static IAdoResourceProvider Create(string sectionName, string providerName)
        {
            // locate any mappings for the specified provider
            ProviderNameTypeMapping mapping =
                DbProviderSettings.Get(sectionName).ResourceProviders.FindByProviderName(
                    providerName);

            IAdoResourceProvider resourceProvider;
            if (mapping != null)
                resourceProvider = TypeUtilities.CreateInstance<IAdoResourceProvider>(mapping.Type);
            else
            {
                // no resource provider mapping found in config
                // return the default resource provider
                resourceProvider = new DefaultAdoResourceProvider();
            }

            return resourceProvider;
        }
    }
}
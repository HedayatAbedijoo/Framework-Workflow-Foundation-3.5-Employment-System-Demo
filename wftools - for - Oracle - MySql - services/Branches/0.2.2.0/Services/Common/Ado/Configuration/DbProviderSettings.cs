using System;
using System.Configuration;

namespace WFTools.Services.Common.Ado.Configuration
{
    /// <summary>
    /// Represents the configuration for individual database providers.
    /// </summary>
    [Serializable]
    public class DbProviderSettings : ConfigurationSection
    {
        /// <summary>
        /// Retrieve database provider settings using the specified section name.
        /// </summary>
        /// <param name="sectionName">
        /// Name of the section containing service settings.
        /// </param>
        /// <returns>
        /// <see cref="DbProviderSettings" /> containing information from the configuration.
        /// </returns>
        public static DbProviderSettings Get(string sectionName)
        {
            return (DbProviderSettings)ConfigurationManager.GetSection(sectionName);
        }

        /// <summary>
        /// Available resource providers for the persistence provider.
        /// </summary>
        [ConfigurationProperty("resourceProviders")]
        [ConfigurationCollection(typeof(ProviderNameTypeMappingCollection))]
        public ProviderNameTypeMappingCollection ResourceProviders
        {
            get { return (ProviderNameTypeMappingCollection)base["resourceProviders"]; }
        }

        /// <summary>
        /// Available name resolvers for the persistence provider.
        /// </summary>
        [ConfigurationProperty("nameResolvers")]
        [ConfigurationCollection(typeof(ProviderNameTypeMappingCollection))]
        public ProviderNameTypeMappingCollection NameResolvers
        {
            get { return (ProviderNameTypeMappingCollection)base["nameResolvers"]; }
        }

        /// <summary>
        /// Available value readers for the persistence provider.
        /// </summary>
        [ConfigurationProperty("valueReaders")]
        [ConfigurationCollection(typeof(ProviderNameTypeMappingCollection))]
        public ProviderNameTypeMappingCollection ValueReaders
        {
            get { return (ProviderNameTypeMappingCollection)base["valueReaders"]; }
        }
    }
}
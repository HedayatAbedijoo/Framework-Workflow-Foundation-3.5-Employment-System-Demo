using System;
using System.Configuration;

namespace WFTools.Services.Common.Ado.Configuration
{
    /// <summary>
    /// Represents the mapping between an ADO.NET provider name and 
    /// a .NET type that performs some function for it.
    /// </summary>
    [Serializable]
    public class ProviderNameTypeMapping : ConfigurationElement
    {
        /// <summary>
        /// The name of the provider with which the type is associated.
        /// </summary>
        [ConfigurationProperty("providerName")]
        public string ProviderName
        {
            get { return (string)base["providerName"]; }
            set { base["providerName"] = value; }
        }

        /// <summary>
        /// The name of the type.
        /// </summary>
        [ConfigurationProperty("type")]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }
    }
}
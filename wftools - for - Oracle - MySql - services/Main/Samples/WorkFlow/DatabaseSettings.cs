using System;
using System.Configuration;

namespace WFTools.Samples.Windows
{
    /// <summary>
    /// Settings used when initially creating the workflow runtime.
    /// </summary>
    public class DatabaseSettings
    {
        private ConnectionStringSettings persistenceConnectionString;
        /// <summary>
        /// Gets/sets the connection string for the persistence
        /// service of the workflow runtime.
        /// </summary>
        public ConnectionStringSettings PersistenceConnectionString
        {
            get { return persistenceConnectionString; }
            set { persistenceConnectionString = value; }
        }

        /// <summary>
        /// Gets a value indicating whether a persistence connection string was specified.
        /// </summary>
        public Boolean HasPersistenceConnectionString
        {
            get { return persistenceConnectionString != null; }
        }

        private ConnectionStringSettings trackingConnectionString;
        /// <summary>
        /// Gets/sets the connection string for the tracking
        /// service of the workflow runtime.
        /// </summary>
        public ConnectionStringSettings TrackingConnectionString
        {
            get { return trackingConnectionString; }
            set { trackingConnectionString = value; }
        }

        /// <summary>
        /// Gets a value indicating whether a tracking connection string was specified.
        /// </summary>
        public Boolean HasTrackingConnectionString
        {
            get { return trackingConnectionString != null; }
        }

        private Boolean useLocalTransactions;
        /// <summary>
        /// Gets/sets whether to use local transactions when
        /// using the persistence and tracking services.
        /// </summary>
        public Boolean UseLocalTransactions
        {
            get { return useLocalTransactions; }
            set { useLocalTransactions = value; }
        }
    }
}
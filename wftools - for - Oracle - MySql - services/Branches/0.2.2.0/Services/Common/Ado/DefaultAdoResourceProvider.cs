using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Transactions;
using WFTools.Services.Common.Ado;

namespace WFTools.Services.Common.Ado
{
    /// <summary>
    /// Implementation of <see cref="IAdoResourceProvider" /> that uses the generic ADO
    /// methods to create database resources.
    /// 
    /// It also serves as a base class for other <see cref="IAdoResourceProvider" />
    /// implementations as it contains several helper methods for that
    /// very purpose.
    /// </summary>
    public class DefaultAdoResourceProvider : IAdoResourceProvider
    {
        /// <summary>
        /// Initialise the resource provider with connection string details.
        /// </summary>
        /// <param name="connectionStringSettings">
        /// The connection string settings to initialise with.
        /// </param>
        public virtual void Initialise(ConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings == null)
                throw new ArgumentNullException("connectionStringSettings");

            this.connectionStringSettings = connectionStringSettings;

            this.dbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
            if (this.dbProviderFactory == null)
            {
                throw new ArgumentOutOfRangeException("connectionStringSettings",
                    "Invalid connection string settings specified.");
            }
        }

        private ConnectionStringSettings connectionStringSettings;
        /// <summary>
        /// The connection string settings used to initialise the resource provider.
        /// </summary>
        protected virtual ConnectionStringSettings ConnectionStringSettings
        {
            get { return connectionStringSettings; }
        }

        private DbProviderFactory dbProviderFactory;
        /// <summary>
        /// The database provider factory used to provide ADO resources.
        /// </summary>
        protected virtual DbProviderFactory DbProviderFactory
        {
            get { return dbProviderFactory; }
        }

        /// <summary>
        /// Creates an ADO connection and enlists it in the specified transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public DbConnection CreateEnlistedConnection(Transaction transaction)
        {
            DbConnectionStringBuilder dbConnectionStringBuilder = dbProviderFactory.CreateConnectionStringBuilder();
            dbConnectionStringBuilder.ConnectionString = connectionStringSettings.ConnectionString;
            // make sure the connection isn't enlisted by default
            if (dbConnectionStringBuilder.ContainsKey("enlist"))
                dbConnectionStringBuilder.Remove("enlist");

            AddEnlistToConnectionString(dbConnectionStringBuilder);

            DbConnection dbConnection = dbProviderFactory.CreateConnection();
            // enlist the connection in the specified transaction
            dbConnection.ConnectionString = dbConnectionStringBuilder.ToString();
            dbConnection.Open();
            dbConnection.EnlistTransaction(transaction);

            return dbConnection;
        }

        /// <summary>
        /// Add the 'enlist' parameter to the connection string.
        /// </summary>
        protected virtual void AddEnlistToConnectionString(DbConnectionStringBuilder dbConnectionStringBuilder)
        {
            try
            {
                dbConnectionStringBuilder.Add("enlist", false);
            }
            catch (Exception e)
            {
                // REVIEW
                // shouldn't be catching this exception - if the provider doesn't
                // support the enlist keyword then it probably doesn't support
                // the System.Transactions namespace properly!
                //
                // Remove once MySql supports System.Transactions properly!
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// Creates an ADO connection initialised using the previously
        /// specified connection string.
        /// </summary>
        public virtual DbConnection CreateConnection()
        {
            DbConnection dbConnection = dbProviderFactory.CreateConnection();

            dbConnection.ConnectionString = connectionStringSettings.ConnectionString;

            return dbConnection;
        }

        /// <summary>
        /// Creates an ADO command initialised with the specified connection
        /// and command text. This uses the default command type.
        /// </summary>
        public virtual DbCommand CreateCommand(DbConnection dbConnection, string commandText)
        {
            return CreateCommand(dbConnection, commandText, CommandType.Text);
        }

        /// <summary>
        /// Creates an ADO command initialised with the specified connection, 
        /// command text and type.
        /// </summary>
        public virtual DbCommand CreateCommand(DbConnection dbConnection, string commandText, CommandType commandType)
        {
            if (dbConnection == null)
                throw new ArgumentNullException("dbConnection");

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            DbCommand dbCommand = dbProviderFactory.CreateCommand();

            dbCommand.CommandText = commandText;
            dbCommand.CommandType = commandType;
            dbCommand.Connection = dbConnection;

            return dbCommand;
        }

        /// <summary>
        /// Add an input parameter initialised with the value and type to the 
        /// specified <see cref="DbCommand" />.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public virtual DbParameter AddParameter(DbCommand dbCommand, string name, object value, AdoDbType type)
        {
            return AddParameter(dbCommand, name, value, type, ParameterDirection.Input);
        }

        /// <summary>
        /// Indicates whether the AddParameter methods should deal with
        /// <see cref="AdoDbType.Cursor" />.
        /// </summary>
        /// <remarks>
        /// Default is false.
        /// </remarks>
        protected virtual bool SupportCursors
        {
            get { return false; }
        }

        /// <summary>
        /// Add a parameter initialised with the value, type and direction to 
        /// the specified <see cref="DbCommand" />.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public virtual DbParameter AddParameter(DbCommand dbCommand, string name, object value, AdoDbType type, ParameterDirection direction)
        {
            if (!SupportCursors && type == AdoDbType.Cursor)
                return null;

            DbParameter dbParameter = dbProviderFactory.CreateParameter();

            dbParameter.ParameterName = name;
            dbParameter.Direction = direction;

            PopulateValue(dbParameter, value);
            PopulateType(dbParameter, type);

            dbCommand.Parameters.Add(dbParameter);

            return dbParameter;
        }

        /// <summary>
        /// Add a parameter initialised with the type and direction to the
        /// specified <see cref="DbCommand" />.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public virtual DbParameter AddParameter(DbCommand dbCommand, string name, AdoDbType type, ParameterDirection direction)
        {
            return AddParameter(dbCommand, name, null, type, direction);
        }

        /// <summary>
        /// Populate the specified <see cref="DbParameter" /> with its value.
        /// </summary>
        /// <remarks>
        /// Derived classes can use this as an opportunity to replace the value
        /// with something more appropriate to the underlying provider.
        /// </remarks>
        /// <param name="dbParameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual void PopulateValue(DbParameter dbParameter, object value)
        {
            if (value == null)
                dbParameter.Value = DBNull.Value;
            else if (value is Enum)
                dbParameter.Value = (int) value;
            else
                dbParameter.Value = value;
        }

        /// <summary>
        /// Populate the specified <see cref="DbParameter" /> with its type.
        /// </summary>
        /// <remarks>
        /// Derived classes can use this as an opportunity to replace the type
        /// with something more appropriate to the underlying provider.
        /// </remarks>
        /// <param name="dbParameter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual void PopulateType(DbParameter dbParameter, AdoDbType type)
        {
            switch (type)
            {
                case AdoDbType.AnsiString:
                    dbParameter.DbType = DbType.AnsiString;
                    break;
                case AdoDbType.Binary:
                    dbParameter.DbType = DbType.Binary;
                    break;
                case AdoDbType.Boolean:
                    dbParameter.DbType = DbType.Boolean;
                    break;
                case AdoDbType.Byte:
                    dbParameter.DbType = DbType.Byte;
                    break;
                case AdoDbType.Currency:
                    dbParameter.DbType = DbType.Currency;
                    break;
                case AdoDbType.Date:
                    dbParameter.DbType = DbType.Date;
                    break;
                case AdoDbType.DateTime:
                    dbParameter.DbType = DbType.DateTime;
                    break;
                case AdoDbType.Decimal:
                    dbParameter.DbType = DbType.Decimal;
                    break;
                case AdoDbType.Double:
                    dbParameter.DbType = DbType.Double;
                    break;
                case AdoDbType.Guid:
                    dbParameter.DbType = DbType.Guid;
                    break;
                case AdoDbType.Int16:
                    dbParameter.DbType = DbType.Int16;
                    break;
                case AdoDbType.Int32:
                    dbParameter.DbType = DbType.Int32;
                    break;
                case AdoDbType.Int64:
                    dbParameter.DbType = DbType.Int64;
                    break;
                case AdoDbType.Object:
                    dbParameter.DbType = DbType.Object;
                    break;
                case AdoDbType.SByte:
                    dbParameter.DbType = DbType.SByte;
                    break;
                case AdoDbType.Single:
                    dbParameter.DbType = DbType.Single;
                    break;
                case AdoDbType.String:
                    dbParameter.DbType = DbType.String;
                    break;
                case AdoDbType.Time:
                    dbParameter.DbType = DbType.Time;
                    break;
                case AdoDbType.UInt16:
                    dbParameter.DbType = DbType.UInt16;
                    break;
                case AdoDbType.UInt32:
                    dbParameter.DbType = DbType.UInt32;
                    break;
                case AdoDbType.UInt64:
                    dbParameter.DbType = DbType.UInt64;
                    break;
                case AdoDbType.VarNumeric:
                    dbParameter.DbType = DbType.VarNumeric;
                    break;
                case AdoDbType.AnsiStringFixedLength:
                    dbParameter.DbType = DbType.AnsiStringFixedLength;
                    break;
                case AdoDbType.StringFixedLength:
                    dbParameter.DbType = DbType.StringFixedLength;
                    break;
                case AdoDbType.Xml:
                    dbParameter.DbType = DbType.Xml;
                    break;
            }
        }
    }
}
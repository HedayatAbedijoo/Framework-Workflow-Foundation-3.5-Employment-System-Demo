using System;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using WFTools.Services.Common.Ado;

namespace WFTools.Services.MySql
{
    /// <summary>
    /// MySQL specific implementation of <see cref="IAdoResourceProvider" />.
    /// </summary>
    /// <remarks>
    /// This implementation modifies the parameter value/type population
    /// so that GUIDs are correctly populated.
    /// </remarks>
    public class MySqlResourceProvider : DefaultAdoResourceProvider
    {
        /// <summary>
        /// Replaces any existing enlist parameter in the connection string
        /// with a value indicating that manual enlist is necessary.
        /// </summary>
        /// <remarks>
        /// MySql uses the keywords 'auto enlist' for transaction enlistment.
        /// </remarks>
        protected override void ReplaceEnlistInConnectionString(DbConnectionStringBuilder dbConnectionStringBuilder)
        {
            if (dbConnectionStringBuilder.ContainsKey("auto enlist"))
                dbConnectionStringBuilder.Remove("auto enlist");

            dbConnectionStringBuilder.Add("auto enlist", false);
        }

        /// <summary>
        /// Populate the specified <see cref="DbParameter" /> with its value.
        /// </summary>
        /// <remarks>
        /// This implementation makes sure that GUIDs are passed as strings.
        /// </remarks>
        /// <param name="dbParameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override void PopulateValue(DbParameter dbParameter, object value)
        {
            if (value is Guid)
                dbParameter.Value = value.ToString();
            else
                base.PopulateValue(dbParameter, value);
        }

        /// <summary>
        /// Populate the specified <see cref="DbParameter" /> with its type.
        /// </summary>
        /// <remarks>
        /// This implementation makes sure that GUIDs are passed as strings.
        /// </remarks>
        /// <param name="dbParameter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override void PopulateType(DbParameter dbParameter, AdoDbType type)
        {
            MySqlParameter mySqlParameter = dbParameter as MySqlParameter;
            if (mySqlParameter == null)
                throw new ArgumentException(RM.Get_Error_MySqlParameterExpected());

            if (type == AdoDbType.Guid)
                mySqlParameter.DbType = DbType.String;


        }
    }
}

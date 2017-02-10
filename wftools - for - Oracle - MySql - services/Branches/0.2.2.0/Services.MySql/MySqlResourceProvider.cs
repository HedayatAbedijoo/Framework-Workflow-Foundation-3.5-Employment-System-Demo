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
                throw new ArgumentException("Expected a MySqlParameter.");

            if (type == AdoDbType.Guid)
                mySqlParameter.DbType = DbType.String;
        }
    }
}

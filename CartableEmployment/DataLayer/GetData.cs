using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataLayer
{
    public class GetData
    {
        private static string ConnectionString = "Data Source=.;Initial Catalog=Cartable;Integrated Security=True";

        public static string GetPageToGoTo(string StateName)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("Select * From Activity_State WHERE StateName ='{0}' ", StateName);
            //SqlCommand command = new SqlCommand(sql);
            SqlDataAdapter dt = new SqlDataAdapter(sql, con);

            DataSet ds = new DataSet();
            dt.Fill(ds);

            return ds.Tables[0].Rows[0]["PageKey"].ToString();
        }

        public static int GetPageToGoToId(string StateName)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("Select * From Activity_State WHERE PageKey ='{0}' ", StateName);
            //SqlCommand command = new SqlCommand(sql);
            SqlDataAdapter dt = new SqlDataAdapter(sql, con);

            DataSet ds = new DataSet();
            dt.Fill(ds);

            return Convert.ToInt16(ds.Tables[0].Rows[0]["Id"].ToString());
        }
        public static string GetStarterPage()
        {
            return "StartMailPage";
        }

        public static DataSet GetInstanceState()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("Select * From InstanceState");
            SqlDataAdapter dt = new SqlDataAdapter(sql, con);

            DataSet ds = new DataSet();
            dt.Fill(ds);
            return ds;
        }
        public static DataSet GetCartable()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("Select * From CartableInfo");
            SqlDataAdapter dt = new SqlDataAdapter(sql, con);

            DataSet ds = new DataSet();
            dt.Fill(ds);
            return ds;
        }

        public static void StartIdleJob(Guid workflowId)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("UPDATE CartableInfo " +
                                       "SET StatusId = 'Run' WHERE WorkflowId = '{0}'", workflowId);
            SqlCommand command = new SqlCommand(sql, con);
            try
            {
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }
        public static void CompeleteSubJob(Guid workflowId)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("UPDATE CartableInfo " +
                                       "SET StatusId = 'Completed' WHERE WorkflowId = '{0}'", workflowId);
            SqlCommand command = new SqlCommand(sql, con);
            try
            {
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }

        public static void WaiteParentJob(Guid parentInstanceId)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("UPDATE CartableInfo" +
                                       " SET StatusId = 'Wait' WHERE WorkflowId = '{0}'", parentInstanceId);
            SqlCommand command = new SqlCommand(sql, con);
            try
            {
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }

        public static void CreateNewToSubFlowCartable(string instanceId, Guid parentInstanceId, string statusId, string stateKey, byte[] EventWating)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Empty;
            if (EventWating != null)
                sql = string.Format("INSERT INTO CartableInfo" +
                                              "(CartableId, WorkflowId, PageKey_Name , EventWaiting, statusId,parentWorkflowId )" +
                                              "VALUES     ('{0}', '{1}', '{2}' , @PramEvents ,'{3}','{4}')", Guid.NewGuid(), instanceId, stateKey, statusId, parentInstanceId.ToString());

            else
                sql = string.Format("INSERT INTO CartableInfo" +
                                           "(CartableId, WorkflowId, PageKey_Name , statusId,parentWorkflowId )" +
                                           "VALUES     ('{0}', '{1}', '{2}' ,'{3}','{4}')", Guid.NewGuid(), instanceId, stateKey, statusId, parentInstanceId.ToString());


            SqlCommand command = new SqlCommand(sql, con);

            try
            {
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }
        public static void CreateNewToCartable(string instanceId, string parentInstanceId, string statusId, string stateKey, byte[] EventWating)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("INSERT INTO CartableInfo" +
                                       "(CartableId, WorkflowId, PageKey_Name , EventWaiting, statusId )" +
                                       "VALUES     ('{0}', '{1}', '{2}' , @PramEvents ,'{3}')", Guid.NewGuid(), instanceId, stateKey, statusId);



            SqlCommand command = new SqlCommand(sql, con);
            command.Parameters.Add("@PramEvents", (object)EventWating);
            // command.Parameters.Add("@parentWorkflowId", DBNull.Value);
            try
            {
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }

        public static void UpdateJob(string instanceId, string stateKey, byte[] EventWating)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("UPDATE    CartableInfo SET       " +
                                       "PageKey_Name = '{0}' , EventWaiting = @PramEvents " +
                                       "WHERE WorkflowId = '{1}'", stateKey, instanceId);



            SqlCommand command = new SqlCommand(sql, con);
            command.Parameters.Add("@PramEvents", (object)EventWating);
            try
            {
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }
        public static void UpdateJobStaus(string instanceId, string StatusId)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("UPDATE    CartableInfo SET       " +
                                       "StatusId = '{0}'  " +
                                       "WHERE WorkflowId = '{1}'", StatusId, instanceId);



            SqlCommand command = new SqlCommand(sql, con);
            try
            {
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }
        public static void UpdateCartable(string instanceId, string stateKey, byte[] EventWating)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("UPDATE    CartableInfo SET       " +
                                       "PageKey_Name = '{0}' , EventWaiting = @PramEvents " +
                                       "WHERE WorkflowId = '{1}'", stateKey, instanceId);



            SqlCommand command = new SqlCommand(sql, con);
            command.Parameters.Add("@PramEvents", (object)EventWating);
            try
            {
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }

        public static byte[] GetEventByte(string instanceId)
        {
            byte[] tmp = null;
            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("Select EventWaiting From CartableInfo WHERE WorkflowId ='{0}' ", instanceId);
            SqlCommand command = new SqlCommand(sql, con);
            SqlDataReader reader;

            try
            {
                con.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tmp = reader["EventWaiting"] as byte[];
                    //Response.BinaryWrite((Byte[])objReader["Image"]);
                }
                con.Close();
            }
            catch (Exception)
            {

                throw;
            }

            return tmp;



            //DataSet ds = new DataSet();
            //return Convert.ToByte(ds.Tables[0].Rows[0][0]);

        }

        public static string GetStateNameByPageKey(string PageKey)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            string sql = string.Format("Select * From Activity_State WHERE StateName ='{0}' ", PageKey);
            //SqlCommand command = new SqlCommand(sql);
            SqlDataAdapter dt = new SqlDataAdapter(sql, con);

            DataSet ds = new DataSet();
            dt.Fill(ds);

            return ds.Tables[0].Rows[0]["StateName"].ToString();
        }

        public static byte[] DataSet2Byte(DataSet ds)
        {
            MemoryStream ms = new MemoryStream();
            if (ds == null)
                return null;
            else
            {
                new BinaryFormatter().Serialize(ms, ds);
                return ms.ToArray();
            }
        }

    }

}

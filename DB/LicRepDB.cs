using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using DataPostWS.DataType;
using DataPostWS.Util;
using iPushData;
namespace DataPostWS.DB
{
    public class LicRepDB 
    {
        string dbString = "";
        SqlConnection conn = null ;
        SqlDataReader rdr = null;
        
        public LicRepDB(string SiteName)
        {
            dbString = Common.GetVendorConnectionString(SiteName);
            conn = new System.Data.SqlClient.SqlConnection(dbString);
        }

        public Bet[] Dot_Dep_GetBetDetail_All(Int64 version_key, bool isTest, out Int64 last_version_key)
        {

            Bet[] ary = null;
            List<SqlParameter> parameters = new List<SqlParameter>();
            SqlParameter NVK = new SqlParameter();
            NVK.ParameterName = "@EndVerKey";
            NVK.DbType = DbType.Int32;
            NVK.Direction = ParameterDirection.Output;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Dot_Dep_GetBetDetail_ALL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@version_key", version_key));
                cmd.Parameters.Add(new SqlParameter("@isTest", isTest));
                cmd.Parameters.Add(NVK);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    ary = ClassBase.CreateObjectFromReader<Bet>(rdr);
                }
                last_version_key = Convert.ToInt64(NVK.Value);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                if (rdr != null)
                {
                    rdr.Close();
                }
            }
            return ary;
        }
        public Int64 Dot_Dep_GetLastVersionKey(bool isTest, int iVK)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            object val = null;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Dot_Dep_GetLastVersionKey", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@isTest", isTest));
                cmd.Parameters.Add(new SqlParameter("@id", iVK));
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    val = rdr["counter"].ToString();
                }

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                if (rdr != null)
                {
                    rdr.Close();
                }
            }
            return Convert.ToInt64(val == null || val == DBNull.Value ? 0 : val);
        }
        public void Dot_Dep_UpdLastVersionKey(Int64 version_key, int iVK)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Dot_Dep_UpdLastVersionKey", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@id", iVK));
                cmd.Parameters.Add(new SqlParameter("@version_key", version_key));
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                if (rdr != null)
                {
                    rdr.Close();
                }
            }

        }

    }
}

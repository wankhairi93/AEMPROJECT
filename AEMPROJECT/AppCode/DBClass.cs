using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEMPROJECT
{
    public class DBClass
    {
        public string ConnStr = ConfigurationManager.AppSettings["ConnString"];
        SqlConnection conn = default(SqlConnection);
        SqlCommand cmd = default(SqlCommand);
        SqlDataAdapter da = default(SqlDataAdapter);

        #region Database
        public DBClass()
        {
            conn = new SqlConnection();
            cmd = new SqlCommand();
            da = new SqlDataAdapter();
            conn = new SqlConnection(ConnStr);
            conn.Open();
            cmd.Connection = conn;
        }
        public void Disconnect()
        {
            conn.Close();
            conn.Dispose();
        }
        public DataSet Execute_FillDS(string query)
        {
            DataSet ds = new DataSet();
            try
            {
                cmd.CommandText = query;
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                Log.LogEvents("[AEM] Execute_FillDS Error:" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                ds = null;
                throw ex;
            }
            return ds;
        }
        public String Execute_NonQuery(string query, bool isRtnValue = false)
        {
            string result = string.Empty;
            try
            {
                cmd.CommandText = query;
                if (isRtnValue == true)
                    result = Convert.ToString(cmd.ExecuteNonQuery());
                else
                    cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.LogEvents("[AEM] Execute_NonQuery Error:" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                result = ex.Message;
                throw ex;
            }
            return result;
        }
        public String Execute_Scalar(string query, bool isRtnErrMsg = false)
        {
            object result = null;
            try
            {
                cmd.CommandText = query;
                result = cmd.ExecuteScalar();
                if (result == null)
                    result = string.Empty;
            }
            catch (Exception ex)
            {
                Log.LogEvents("[AEM] Execute_Scalar Error:" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                if (isRtnErrMsg == true)
                    result = ex.Message;
                else
                    result = "-1";

                throw ex;
            }
            return Convert.ToString(result);
        }
        public static String SQLNStr(string data)
        {
            return "N'" + data.Replace("'", "''") + "'";
        }
        public static String SQLStr(string data)
        {
            return "'" + data.Replace("'", "''") + "'";
        }
        #endregion

        #region AEM
        public String InsertPlatform(Int32 id, string uniqueName, double latitude, double longitude, DateTime createdAt, DateTime updatedAt)
        {
            string result = string.Empty;
            try
            {
                string query = string.Empty, exist = string.Empty;
                query = $"SELECT id FROM Platform WHERE id = {id}";
                exist = Execute_Scalar(query);

                if (exist != string.Empty)
                {
                    query = $"UPDATE Platform SET uniqueName = {SQLNStr(uniqueName)}, latitude = {latitude}, longitude = {longitude}, createdAt = {SQLNStr(createdAt.ToString("yyyy-MM-dd hh:mm:ss tt"))}," +
                        $"updatedAt = {SQLNStr(updatedAt.ToString("yyyy-MM-dd hh:mm:ss tt"))} WHERE id = {id}";
                }
                else
                {
                    query = $"INSERT INTO Platform(id,uniqueName,latitude,longitude,createdAt,updateAt) VALUES ({id}, {SQLNStr(uniqueName)}, {latitude}, {longitude}, " +
                        $"{SQLNStr(createdAt.ToString("yyyy-MM-dd hh:mm:ss tt"))},{SQLNStr(updatedAt.ToString("yyyy-MM-dd hh:mm:ss tt"))})";
                }
                result = Execute_NonQuery(query);
            }
            catch (Exception ex)
            {
                Log.LogEvents("[AEM] InsertPlatform Error:" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw ex;
            }
            return result;
        }
        public String InsertWell(Int32 id, Int32 platformid, string uniqueName, double latitude, double longitude, DateTime createdAt, DateTime updatedAt)
        {
            string result = string.Empty;
            try
            {
                string query = string.Empty, exist = string.Empty;
                query = $"SELECT id FROM Well WHERE id = {id} AND platformId = {platformid}";
                exist = Execute_Scalar(query);

                if (exist != string.Empty)
                {
                    query = $"UPDATE Well SET uniqueName = {SQLNStr(uniqueName)}, latitude = {latitude}, longitude = {longitude}, createdAt = {SQLNStr(createdAt.ToString("yyyy-MM-dd hh:mm:ss tt"))}, " +
                        $"updatedAt = {SQLNStr(updatedAt.ToString("yyyy-MM-dd hh:mm:ss tt"))} WHERE id = {id} AND platformId = {platformid}";
                }
                else
                {
                    query = $"INSERT INTO Well (id, platformId, uniqueName, latitude, longitude, createdAt, updateAt) VALUES ({id}, {platformid}, {SQLNStr(uniqueName)}, {latitude}, " +
                        $"{longitude}, {SQLNStr(createdAt.ToString("yyyy-MM-dd hh:mm:ss tt"))}, {SQLNStr(updatedAt.ToString("yyyy-MM-dd hh:mm:ss tt"))})";
                }
                result = Execute_NonQuery(query);
            }
            catch (Exception ex)
            {
                Log.LogEvents("[AEM] InsertWell Error:" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw ex;
            }
            return result;
        }
        public String InsertPlatformDummy(Int32 id, string uniqueName, double latitude, double longitude, DateTime lastUpdate)
        {
            string result = string.Empty;
            try
            {
                string query = string.Empty, exist = string.Empty;
                query = $"SELECT id FROM Platform WHERE id = {id}";
                exist = Execute_Scalar(query);

                if (exist != string.Empty)
                {
                    query = $"UPDATE Platform SET uniqueName = {SQLNStr(uniqueName)}, latitude = {latitude}, longitude = {longitude}, updatedAt = {SQLNStr(lastUpdate.ToString("yyyy-MM-dd hh:mm:ss tt"))} " +
                        $"WHERE id = {id}";
                }
                else
                {
                    query = $"INSERT INTO Platform (id, uniqueName, latitude, longitude, createdAt, updatedAt) VALUES ({id}, {SQLNStr(uniqueName)}, {latitude}, {longitude}, " +
                        $"{SQLNStr(lastUpdate.ToString("yyyy-MM-dd hh:mm:ss tt"))},{SQLNStr(lastUpdate.ToString("yyyy-MM-dd hh:mm:ss tt"))})";

                }
                result = Execute_NonQuery(query);
            }
            catch (Exception ex)
            {
                Log.LogEvents("[AEM] InsertPlatformDummy Error:" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            finally
            {

            }
            return result;
        }
        public String InsertWellDummy(Int32 id, Int32 platformid, string uniqueName, double latitude, double longitude, DateTime lastUpdate)
        {
            string result = string.Empty;
            try
            {
                string query = string.Empty, exist = string.Empty;
                query = $"SELECT id FROM Well WHERE id = {id} AND platformId = {platformid}";
                exist = Execute_Scalar(query);

                if (exist != string.Empty)
                {
                    query = $"UPDATE Well SET uniqueName = {SQLNStr(uniqueName)}, latitude = {latitude}, longitude = {longitude}, " +
                        $"updatedAt = {SQLNStr(lastUpdate.ToString("yyyy-MM-dd hh:mm:ss tt"))} WHERE id = {id} AND platformId = {platformid}";
                }
                else
                {
                    query = $"INSERT INTO Well (id, platformId, uniqueName, latitude, longitude, createdAt, updateAt) VALUES ({id}, {platformid}, {SQLNStr(uniqueName)}, {latitude}, " +
                        $"{longitude}, {SQLNStr(lastUpdate.ToString("yyyy-MM-dd hh:mm:ss tt"))}, {SQLNStr(lastUpdate.ToString("yyyy-MM-dd hh:mm:ss tt"))})";
                }
                result = Execute_NonQuery(query);
            }
            catch (Exception ex)
            {
                Log.LogEvents("[AEM] InsertWellDummy Error:" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            finally
            {

            }
            return result;
        }
        #endregion
    }
}

using PrecipitationDataAppModels;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PrecipitationDataApp.Helpers
{
    public static class DbHelper
    {
        private static SqlConnection _connection = null;
        public static SqlConnection Connection {

            get {
                return _connection;
            }

        }

        public static string CreateDbTable() {

            var connectionString = ConfigurationManager.ConnectionStrings["PrecipitationDbSql"].ToString();
            _connection = new SqlConnection(connectionString);

            var dateTimeNow = DateTime.Now.ToString("ddMMyyyyHHmmss");
            var databaseName = "PrecipitationData" + dateTimeNow;

            if (_connection.State == ConnectionState.Open)
                _connection.Close();

            _connection.Open();

            var _sql = "CREATE TABLE " + databaseName + " (" +
                    "Xref INT," +
                    "Yref INT," +
                    "Date DATE," +
                    "Value INT,"
                    + ");";
            var _cmd = new SqlCommand(_sql, _connection);

            try
            {
                _cmd.ExecuteNonQuery();

            }
            catch (SqlException)
            {
                return null;
            }

            return databaseName;
        }

        public static bool InsertIntoDb(PrecipitationData precipitationData, string databaseName) {
            
            var _sql = "INSERT INTO "+ databaseName + " ( Xref, Yref, Date, Value )" +
                    "VALUES( " +
                    precipitationData.GridRefXY.Xref +
                    ", " + precipitationData.GridRefXY.Yref +
                    ", '" + precipitationData.Date.Value + "', " +
                    precipitationData.Value + ")";

            var _cmd = new SqlCommand(_sql, Connection);

            try
            {
                _cmd.ExecuteNonQuery();

            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }

        public static void CloseConnection() {

            Connection.Close();
        }
    }
}
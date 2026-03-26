using System;
using System.Data;
using System.Data.SqlClient;

namespace ProudMedStatusAPI
{
    public class Database
    {
        private readonly string _connectionString;

        public Database()
        {
            var config = new Config();
            // Config.ini เก็บแค่ส่วนหลัง → เติม Data Source= ให้ครบ
            _connectionString = $"Data Source={config.ConnectionStringJSD}";
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                return false;
            }
        }

        public DataTable ExecuteQuery(string sql, SqlParameter[]? parameters = null)
        {
            var dt = new DataTable();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public int ExecuteNonQuery(string sql, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }
    }
}
using field_recording_api.Models.DataAccess;
using Microsoft.Data.SqlClient;
using System.Data;

namespace field_recording_api.DataAccess
{
    public class MyRepository
    {
        private readonly string connectionString;

        public MyRepository(string conn)
        {
            this.connectionString = conn;
        }

        public DataTable GetDataToDatatable(string sp_name, List<SqlParameter> Param)
        {
            DataTable dt = new DataTable();

            using(SqlConnection sqlConnection = new SqlConnection(connectionString)) {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = sp_name;
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in Param)
                    cmd.Parameters.AddWithValue(p.ParameterName, p.Value);

                cmd.Connection = sqlConnection;
                cmd.CommandTimeout = 3600;

                sqlConnection.Open();
                reader = cmd.ExecuteReader();

                dt.Load(reader);
            }

            return dt;
        }

        public DataTable RunSQLTextToDatatable(string sql)
        {
            DataTable dt = new DataTable();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                //foreach (SqlParameter p in Param)
                //    cmd.Parameters.AddWithValue(p.ParameterName, p.Value);

                cmd.Connection = sqlConnection;
                cmd.CommandTimeout = 3600;

                sqlConnection.Open();
                reader = cmd.ExecuteReader();

                dt.Load(reader);
            }

            return dt;
        }
    }
}

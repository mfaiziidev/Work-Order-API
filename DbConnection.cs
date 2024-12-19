using System.Configuration;
using System.Data.SqlClient;


namespace WOAPI
{
    public class DBConnection
    {
        public SqlConnection GetDBConnection()
        {
            string strConn = "";

            strConn = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(strConn);
            return conn;
        }
    }
}
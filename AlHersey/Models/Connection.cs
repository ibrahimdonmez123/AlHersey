using Microsoft.Data.SqlClient;

namespace AlHersey.Models
{
    public class Connection
    {
        public static SqlConnection ServerConnect
        {
            get
            {
                SqlConnection sqlConnection = new SqlConnection("Server=DESKTOP-JI1JKUA\\SQLEXPRESS;Database=AlHerseyDataBase;Trusted_Connection=True;TrustServerCertificate=True;");
                return sqlConnection;
            }
        }


    }
}

using System;
using System.Data.SqlClient;

namespace Guldkortet
{
    public class DatabaseManager
    {
        // Anslutningssträng till din lokala databasfil (.mdf)
        private string connectionString = new SqlConnectionStringBuilder
        {
            DataSource = @"(LocalDB)\MSSQLLocalDB",
            AttachDBFilename = @"|DataDirectory|\Kortregister.mdf",
            IntegratedSecurity = true
        }.ConnectionString;

        // Metod som hämtar 
        public string GetRewardTypeByCardId(string cardId)
        {
            string query = "SELECT KortTyp FROM Kort WHERE KortNr = @CardId";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CardId", cardId);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    return result?.ToString();
                }
            }
        }
    }
}

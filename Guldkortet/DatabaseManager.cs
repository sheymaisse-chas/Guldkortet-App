using System;
using System.Data.SqlClient;

namespace Guldkortet
{
    public class DatabaseManager
    {
        // Ansluter till min lokala databasfil (.mdf)
        private string connectionString = new SqlConnectionStringBuilder
        {
            DataSource = @"(LocalDB)\MSSQLLocalDB",
            AttachDBFilename = @"|DataDirectory|\Kortregister.mdf",
            IntegratedSecurity = true
        }.ConnectionString;

        // Metod som hämtar korttypen ("Eldtomat" osv.) från databasen utifrån kortets ID
        public string GetRewardTypeByCardId(string cardId)
        {
            // SQL-fråga med parameter(@CardId) för att förhindra SQL Injection(attacker)
            string query = "SELECT KortTyp FROM Kort WHERE KortNr = @CardId";

            // Skapar och öppnar anslutningen säkert. 
            // 'using' ser till att databasanslutningen stängs och frigörs automatiskt även vid eventuella fel.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Skapar SQL-kommandot med vår fråga och aktiva anslutning
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Matchar parametern cardId med @CardId
                    command.Parameters.AddWithValue("@CardId", cardId);

                    // Öppna anslutningen mot databasfilen
                    connection.Open();

                    // ExecuteScalar används eftersom vi bara förväntar oss ett enskilt värde i retur
                    object result = command.ExecuteScalar();

                    // Om resultatet inte är null returneras korttypen som text, annars returner den null
                    return result?.ToString();
                }
            }
        }
    }
}

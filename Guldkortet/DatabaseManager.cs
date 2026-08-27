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

                    // Om resultatet inte är null returneras korttypen som text, annars returnerar den null
                    if (result != null)
                    {
                        return result.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        // Metod för att uppdatera statusen på ett kort och markera det som utnyttjat
        public bool MarkCardUsed(string cardId)
        {
            // SQL-fråga som uppdaterar kolumnen Uttnyttjad till 1 (true) för det angivna kortnumret
            string query = "UPDATE Kort SET Uttnyttjad = 1 WHERE KortNr = @CardId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CardId", cardId);

                    connection.Open();

                    // ExecuteNonQuery används för UPDATE/INSERT/DELETE där vi inte förväntar oss rader i retur, 
                    // utan antalet rader som påverkades i databasen.
                    int rowsAffected = command.ExecuteNonQuery();

                    // Om mer än 0 rader uppdaterades returneras true (lyckades)
                    return rowsAffected > 0;
                }
            }
        }

        // Metod för att lägga till historik av alla godkända inlösen i databasen
        public bool InsertTransactionLog(string cardId, string customerId, string rewardName)
        {
            // SQL-fråga för att lägga till en ny rad i en tabell (t.ex. Transaktioner)
            string query = "INSERT INTO Transaktioner (KortNr, KundID, Beloning, Datum) VALUES (@CardId, @CustomerId, @RewardName, @Datum)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Använder parametrar för att göra databasanropet säkert mot SQL-injection
                    command.Parameters.AddWithValue("@CardId", cardId);
                    command.Parameters.AddWithValue("@CustomerId", customerId);
                    command.Parameters.AddWithValue("@RewardName", rewardName);
                    command.Parameters.AddWithValue("@Datum", DateTime.Now);

                    connection.Open();

                    // ExecuteNonQuery kör INSERT-frågan och returnerar antalet rader som lagts till
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }
    }
}

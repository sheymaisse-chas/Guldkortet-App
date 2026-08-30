using System;
using Microsoft.Data.Sqlite;

namespace Guldkortet
{
    public class DatabaseManager
    {
        // Ansluter till min lokala databasfil
        private string connectionString = @"Data Source=C:\Users\Sheyma\Documents\C-Sharp\Guldkortet\Guldkortet\Guldkortet.db";

        // Metod som hämtar korttypen ("Eldtomat" osv.) från databasen utifrån kortets ID
        public Card GetCardInfo(string cardId)
        {
            // SQL-fråga med parameter(@CardId) för att förhindra SQL Injection(attacker)
            string query = "SELECT CardName, IsGoldCard, IsUsed FROM Cards WHERE CardID = @CardId";

            // Skapar och öppnar anslutningen säkert. 
            // 'using' ser till att databasanslutningen stängs och frigörs automatiskt även vid eventuella fel.
            using (SqliteConnection connection = new SqliteConnection(connectionString))

            // Skapar SQL-kommandot med vår fråga och aktiva anslutning
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                // Matchar parametern cardId med @CardId
                command.Parameters.AddWithValue("@CardId", cardId);

                try
                {
                    // Öppna anslutningen mot databasfilen
                    connection.Open();

                    // ExecuteReader används eftersom vi nu hämtar flera kolumner (CardName, IsGoldCard, IsUsed) från samma rad
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        // Om en rad hittas byggs ett CardLookupResult med kortets data, annars returneras Found = false
                        if (reader.Read())
                        {
                            return new Card(
                                cardId,
                                reader.GetString(0),
                                reader.GetInt32(1) == 1,
                                reader.GetInt32(2) == 1

                                );
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
                catch (SqliteException ex)
                {
                    // Om databasanslutningen eller frågan misslyckas kastas ett eget undantag
                    throw new DatabaseConnectionException("Kunde inte hämta kortinformation från databasen.", ex);
                }
            }
        }

        // Hämtar kunduppgifter från databasen utifrån CustomerID, eller null om kunden inte hittas
        public Customer GetCustomerById(string customerId)
        {
            string query = "SELECT CustomerID, Name, Email FROM Customers WHERE CustomerID = @CustomerId";

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerId", customerId);

                try
                {
                    connection.Open();

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Bygger ett Customer-objekt med datan från databasen
                            return new Customer(
                                reader.GetString(0),
                                reader.GetString(1),
                                reader.GetString(2)
                            );
                        }
                        else
                        {
                            // Ingen kund med detta ID hittades
                            return null;
                        }
                    }
                }
                catch (SqliteException ex)
                {
                    throw new DatabaseConnectionException("Kunde inte hämta kunduppgifter från databasen.", ex);
                }
            }
        }

        // Metod för att uppdatera statusen på ett kort och markera det som utnyttjat
        public bool MarkCardUsed(string cardId)
        {
            // SQL-fråga som uppdaterar kolumnen Uttnyttjad till 1 (true) för det angivna kortnumret
            string query = "UPDATE Cards SET IsUsed = 1 WHERE CardID = @CardId";

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CardId", cardId);

                try
                {
                    connection.Open();

                    // ExecuteNonQuery används för UPDATE/INSERT/DELETE där vi inte förväntar oss rader i retur, 
                    // utan antalet rader som påverkades i databasen.
                    int rowsAffected = command.ExecuteNonQuery();


                    // Om mer än 0 rader uppdaterades returneras true (att den lyckades)
                    return rowsAffected > 0;
                }
                catch (SqliteException ex)
                {
                    throw new DatabaseConnectionException("Kunde inte uppdatera kortets status i databasen.", ex);
                }
            }
        }

        // Metod för att lägga till historik av alla godkända inlösen i databasen
        public bool InsertTransactionLog(string cardId, string customerId, string rewardName)
        {
            // SQL-fråga för att lägga till en ny rad i en tabell (t.ex. Transaktioner)
            string query = "INSERT INTO Transaktioner (CardID, CustomerID, RewardName, Date) VALUES (@CardId, @CustomerId, @RewardName, @Date)";

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                // Använder parametrar för att göra databasanropet säkert mot SQL-injection
                command.Parameters.AddWithValue("@CardId", cardId);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@RewardName", rewardName);
                command.Parameters.AddWithValue("@Date", DateTime.Now);

                try
                {
                    connection.Open();

                    // ExecuteNonQuery kör INSERT-frågan och returnerar antalet rader som lagts till
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                catch (SqliteException ex)
                {
                    throw new DatabaseConnectionException("Kunde inte spara transaktionsloggen i databasen.", ex);
                }

            }
        }
    }
}

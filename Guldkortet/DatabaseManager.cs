using System;
using System.Data.SqlClient;

namespace Guldkortet
{
    public class DatabaseManager
    {
        // 1. Hämtar mappen där programmet körs och för Kort-databasen
        private string connectionStringKort = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + AppDomain.CurrentDomain.BaseDirectory + "Kortregister.mdf;Integrated Security=True;";

        // 2. För Kund-databasen
        private string connectionStringKund = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + AppDomain.CurrentDomain.BaseDirectory + "Kundregister.mdf;Integrated Security=True;";

        // Metod som hämtar korttypen ("Eldtomat" osv.) från databasen utifrån kortets ID
        public Kort GetKortByNr(string kortNr)
        {
            // SQL-fråga med parameter(@KortNr) för att förhindra SQL Injection(attacker)
            string query = "SELECT KortTyp FROM Kort WHERE KortNr = @KortNr";

            // Skapar och öppnar anslutningen säkert. 
            // 'using' ser till att databasanslutningen stängs och frigörs automatiskt även vid eventuella fel.
            using (SqlConnection connection = new SqlConnection(connectionStringKort))

            // Skapar SQL-kommandot med vår fråga och aktiva anslutning
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Matchar parametern kortNr med @KortNr
                command.Parameters.AddWithValue("@KortNr", kortNr);

                try
                {
                    // Öppna anslutningen mot databasfilen
                    connection.Open();

                    // ExecuteReader används eftersom vi nu hämtar flera kolumner (KortNr, KortTyp) från samma rad
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Om en rad hittas byggs ett Kort-objekt med kortets data, annars returneras Found = false
                        if (reader.Read())
                        {
                            return new Kort(
                                kortNr,
                                reader.GetString(0)
                                );
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
                catch (SqlException ex)
                {
                    // Om databasanslutningen eller frågan misslyckas kastas ett eget undantag
                    throw new DatabaseConnectionException("Kunde inte hämta kortinformation från databasen.", ex);
                }
            }
        }

        // Hämtar kunduppgifter från databasen utifrån AnvändarNr, eller null om kunden inte hittas
        public Kund GetKundByNr(string användarNr)
        {
            string query = "SELECT AnvändarNr, Namn, Kommun FROM Kunder WHERE AnvändarNr = @AnvändarNr";

            using (SqlConnection connection = new SqlConnection(connectionStringKund))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AnvändarNr", användarNr);

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Bygger ett Kund-objekt med datan från databasen
                            return new Kund(
                                reader.GetString(0),
                                reader.GetString(1),
                                reader.GetString(2)
                            );
                        }
                        else
                        {
                            // Ingen kund med detta nummer hittades
                            return null;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new DatabaseConnectionException("Kunde inte hämta kunduppgifter från databasen.", ex);
                }
            }
        }

        // Metod för att uppdatera en kunds information
        public bool UpdateKund(string användarNr, string nyttNamn, string nyKommun)
        {
            // SQL-fråga som uppdaterar kommun för det angivna kundnumret
            string query = "UPDATE Kunder SET Namn = @NyttNamn, Kommun = @NyKommun WHERE AnvändarNr = @AnvändarNr";

            using (SqlConnection connection = new SqlConnection(connectionStringKund))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NyttNamn", nyttNamn);
                command.Parameters.AddWithValue("@NyKommun", nyKommun);
                command.Parameters.AddWithValue("@AnvändarNr", användarNr);

                try
                {
                    connection.Open();

                    // ExecuteNonQuery används för UPDATE/INSERT/DELETE där vi inte förväntar oss rader i retur, 
                    // utan antalet rader som påverkades i databasen.
                    int rowsAffected = command.ExecuteNonQuery();


                    // Om mer än 0 rader uppdaterades returneras true (att den lyckades)
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    throw new DatabaseConnectionException("Kunde inte uppdatera kundens uppgifter i databasen.", ex);
                }
            }
        }

        // Metod för att lägga till historik av alla godkända inlösen i databasen
        public bool InsertKund(string användarNr, string namn, string kommun)
        {
            // SQL-fråga för att lägga till en ny kund i Kunder-tabellen
            string query = "INSERT INTO Kunder (AnvändarNr, Namn, Kommun) VALUES (@AnvändarNr, @Namn, @Kommun)";

            using (SqlConnection connection = new SqlConnection(connectionStringKund))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Använder parametrar för att göra databasanropet säkert mot SQL-injection
                command.Parameters.AddWithValue("@AnvändarNr", användarNr);
                command.Parameters.AddWithValue("@Namn", namn);
                command.Parameters.AddWithValue("@Kommun", kommun);

                try
                {
                    connection.Open();

                    // ExecuteNonQuery kör INSERT-frågan och returnerar antalet rader som lagts till
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    throw new DatabaseConnectionException("Kunde inte spara den nya kunden i databasen.", ex);
                }

            }
        }
    }
}

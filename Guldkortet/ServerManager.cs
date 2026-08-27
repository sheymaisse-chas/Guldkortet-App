using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Guldkortet
{
    // Hanterar TCP-servern som tar emot kortkoder från NOS_Export.exe på port 12345.
    public class ServerManager
    {
        // Properties som som behövs för att säkerställa TCP kopplingen.
        private readonly int port = 12345;
        private TcpListener listener;
        private bool isRunning;

        private DatabaseManager dbManager;
        private Form1 mainForm;

        // Konstruktor som tar emot gränssnittet (Form1)
        public ServerManager(Form1 form)
        {
            mainForm = form;
            dbManager = new DatabaseManager();
        }

        // Startar servern och börjar lyssna
        public void StartServer()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            isRunning = true;

            // Loop som tar emot anslutningar
            while (isRunning)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    HandleClient(client);
                }
                catch
                {
                    // Stränger loopen om servern stängs av
                    break;
                }
            }
        }

        // Hanterar den anslutna klienten NOS_Export
        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            try
            {
                // 1. Läser texten från NOS_Export (t.ex. "A1256720-K57295726")
                string rawData = reader.ReadLine();

                // Kontrollera att texten inte är tom och innehåller ett bindestreck
                if (string.IsNullOrEmpty(rawData) || !rawData.Contains("-"))
                {
                    writer.WriteLine("Fel: Koden har ett ogiltigt format.");
                    return;
                }

                // 2. Delar upp strängen vid bindestrecket
                string[] parts = rawData.Split('-');
                string userId = parts[0].Trim();
                string cardId = parts[1].Trim();

                // 3. Söker i databasen efter korttypen
                string rewardType = dbManager.GetRewardTypeByCardId(cardId);

                // 4. Om kortet hittades i databasen
                if (rewardType != null)
                {
                    // Skapar ett Reward-objekt via fabriken
                    Reward reward = RewardFactory.CreateReward(rewardType);

                    // Skickar svarspopup tillbaka till NOS_Export
                    writer.WriteLine("Kort godkänt! Belöning: " + rewardType);

                    // Skickar kortet till gränssnittet
                    mainForm.AddRewardToList(reward, userId);
                }
                else
                {
                    // Annars visas ett felet att kortet inte hittades
                    writer.WriteLine("Fel: Kortet hittades inte i databasen.");
                }
            }
            catch (Exception ex)
            {
                // Om ett fel sker vid behandlingen av en kod
                writer.WriteLine("Fel vid behandling: " + ex.Message);
            }
            finally
            {
                // Stänger strömmar och anslutning säkert
                writer.Close();
                reader.Close();
                stream.Close();
                client.Close();
            }
        }

        // Stänger ner servern
        public void StopServer()
        {
            isRunning = false;
            if (listener != null)
            {
                listener.Stop();
            }
        }
    }
}

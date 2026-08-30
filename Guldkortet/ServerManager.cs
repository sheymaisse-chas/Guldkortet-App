using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            // Try-Catch sats som tar emot anslutningar
            try
            {
                // Om instruktionen anger 127.0.0.1 (Localhost):
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                listener = new TcpListener(localAddr, port);
                listener.Start();
                isRunning = true;

                // Anropar den asynkrona lyssnarloopen direkt
                ListenForClientsAsync();
            }
            catch (Exception ex)
            {
                // Om servern ej startas
                Console.WriteLine("Kunde inte starta servern: " + ex.Message);
            }
        }

        // Loop som tar emot anslutningar i bakgrunden
        private async void ListenForClientsAsync()
        {
            while (isRunning)
            {
                try
                {
                    // await gör att programmet väntar här i bakgrunden tills en klient ansluter
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    // Startar hanteringen av klienten asynkront
                    HandleClientAsync(client);
                }
                catch
                {
                    // Stänger loopen om servern stängs av
                    break;
                }
            }
        }

        // Hanterar den anslutna klienten NOS_Export
        private async Task HandleClientAsync(TcpClient client)
        {
            // 'using' ser till att allt stängs automatiskt när det är klart
            using (client)
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.Default))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.Default) { AutoFlush = true })
            {

                try
                {
                    // Loggar i gränssnittet att någon faktiskt anslöt
                    mainForm.Invoke(new Action(() =>
                        mainForm.AddDebugLog("Klient ansluten! Väntar på data...")));

                    // Loggar i gränssnittet om data är direkt tillgängligt
                    mainForm.Invoke(new Action(() =>
                        mainForm.AddDebugLog($"DataAvailable direkt vid anslutning: {stream.DataAvailable}")));

                    while (client.Connected)
                    {
                        // Läser texten från NOS_Export (t.ex. "A1256720-K57295726")
                        string rawData = await reader.ReadToEndAsync();

                        if (rawData == null)
                        {
                            mainForm.Invoke(new Action(() =>
                                mainForm.AddDebugLog("RawData variabeln är null")));
                        }

                        // Loggar i gränssnittet exakt vad simulatorn skickade
                        mainForm.Invoke(new Action(() =>
                            mainForm.AddDebugLog($"Mottog rådata: '{rawData}'")));

                        // Kontrollerar att texten inte är tom
                        if (string.IsNullOrEmpty(rawData))
                        {
                            await TryWriteStringAsync(writer, "Fel: Tom data.");
                            return;
                        }

                        // Kontrollera att texten innehåller ett bindestreck
                        if (!rawData.Contains("-"))
                        {
                            throw new InvalidCodeFormatException("Koden saknar bindestreck.");
                        }

                        // Delar upp strängen vid bindestrecket
                        string[] parts = rawData.Split('-');
                        string customerId = parts[0].Trim();
                        string cardId = parts[1].Trim();

                        // 3. Söker i databasen efter korttypen
                        CardLookupResult cardInfo = dbManager.GetCardInfo(cardId);

                        // 4.Hantera de olika utfallen baserat på kortets status
                        if (!cardInfo.Found)
                        {
                            await TryWriteStringAsync(writer, "Fel: Okänd kod.");

                            mainForm.Invoke(new Action(() =>
                                mainForm.AddDebugLog($"Kort-ID {cardId} hittades inte i DB.")));
                        }
                        else if (!cardInfo.IsGoldCard)
                        {
                            await TryWriteStringAsync(writer, "Koden är giltig, men ger ingen vinst.");

                            mainForm.Invoke(new Action(() =>
                                mainForm.AddDebugLog($"Kort-ID {cardId} är giltigt men inte ett Guldkort.")));
                        }
                        else if (cardInfo.IsUsed)
                        {
                            throw new CardAlreadyUsedException();
                        }
                        else
                        {
                            Reward reward = RewardFactory.CreateReward(cardInfo.CardName);

                            if (reward != null)
                            {
                                dbManager.MarkCardUsed(cardId);
                                dbManager.InsertTransactionLog(cardId, customerId, reward.Name);

                                await TryWriteStringAsync(writer, reward.GenerateMessage());

                                mainForm.AddRewardToList(reward, customerId);
                            }
                        }
                    }
                }
                catch (InvalidCodeFormatException ex)
                {
                    // Sker om formatet är fel
                    await TryWriteStringAsync(writer, "Fel: " + ex.Message);

                    mainForm.Invoke(new Action(() =>
                        mainForm.AddDebugLog("Formatfel: " + ex.Message)));
                }
                catch (CardAlreadyUsedException ex)
                {
                    await TryWriteStringAsync(writer, "Fel: " + ex.Message);

                    mainForm.Invoke(new Action(() =>
                        mainForm.AddDebugLog("Kortet redan använt: " + ex.Message)));
                }
                catch (DatabaseConnectionException ex)
                {
                    // Sker om databasanslutningen eller frågan misslyckas
                    await TryWriteStringAsync(writer, "Fel: " + ex.Message);

                    mainForm.Invoke(new Action(() =>
                        mainForm.AddDebugLog("Databasfel: " + ex.Message)));
                }
                catch (IOException ex )
                {
                    // Sker om klienten kopplade från oväntat
                    Console.WriteLine($"[Nätverksfel] Klienten kopplades från oväntat: {ex.Message}");
                }
                catch (SocketException ex)
                {
                    // Sker om socketen stängdes från klientens sida
                    Console.WriteLine($"[Socketfel] Nätverksanslutningen bröts: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Om ett fel sker vid behandlingen av en kod
                    Console.WriteLine($"[Systemfel] Oväntat fel vid behandling: {ex.Message}");


                    // Loggar i gränssnittet att det skett ett undantag i servern
                    mainForm.Invoke(new Action(() =>
                        mainForm.AddDebugLog("Undantag i server: " + ex.Message)));

                }
            }
        }

        // Hjälpmetod för att skriva säkert utan att krascha om klienten har kopplat från
        private async Task TryWriteStringAsync(StreamWriter writer, string message)
        {
            try
            {
                await writer.WriteLineAsync(message);
            }
            catch (Exception ex)
            {
                // Om klienten hann koppla från innan vi svarade ignorerar vi skrivfelet
                Console.WriteLine($"[Skrivfel] Kunde inte skicka svar till klienten ('{message}'). Orsak: {ex.Message}");
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

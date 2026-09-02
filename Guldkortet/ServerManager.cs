using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                listener = new TcpListener(localAddr, port);
                listener.Start();
                isRunning = true;

                // Loggar i GUI att servern startat
                mainForm.Invoke(new Action(() =>
                    mainForm.AddDebugLog($"Server startad på port {port}...")));

                // Anropar den asynkrona lyssnarloopen direkt
                ListenForClientsAsync();
            }
            catch (Exception ex)
            {
                // Om servern ej startas loggas felet i GUI
                mainForm.Invoke(new Action(() =>
                    mainForm.AddDebugLog("Kunde inte starta servern: " + ex.Message)));
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
            using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode) { AutoFlush = true })
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
                        char[] buffer = new char[19];
                        int charsRead = await ReadExactAsync(reader, buffer, 19);

                        if (charsRead == 0)
                        {
                            // Klienten stängde anslutningen normalt
                            return;
                        }

                        string rawData = new string(buffer, 0, charsRead);

                        mainForm.Invoke(new Action(() =>
                            mainForm.AddDebugLog($"Längd på rawData: {rawData.Length} tecken")));

                        if (charsRead != 19)
                        {
                            throw new InvalidCodeFormatException("Koden uppfyller inte det förväntade antalet tecken.");
                        }
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
                        }

                        // Kontrollera att texten innehåller ett bindestreck
                        if (!rawData.Contains("-"))
                        {
                            throw new InvalidCodeFormatException("Koden saknar bindestreck.");
                        }

                        // Delar upp strängen vid bindestrecket
                        string[] parts = rawData.Split('-');
                        string användarNr = parts[0].Trim();
                        string kortNr = parts[1].Trim();

                        // 3. Söker i databasen

                        // Söker upp kortinfo i databasen
                        Kort kortInfo = dbManager.GetKortByNr(kortNr);

                        // Söker upp kunden i databasen
                        Kund kundInfo = dbManager.GetKundByNr(användarNr);

                        if (kundInfo == null)
                        {
                            await TryWriteStringAsync(writer, "Fel: Kund hittades inte.");

                            mainForm.Invoke(new Action(() =>
                                mainForm.AddDebugLog($"Kund nummer {användarNr} hittades inte i DB.")));
                            continue;
                        }

                        // 4.Hantera de olika utfallen baserat på kortets status
                        if (kortInfo == null)
                        {
                            await TryWriteStringAsync(writer, "Fel: Okänd kod.");

                            mainForm.Invoke(new Action(() =>
                                mainForm.AddDebugLog($"Kort-ID {kortNr} hittades inte i DB.")));
                        }
                        // 1. KONTROLLERA OM KORTET REDAN ÄR ANVÄNT
                        else if (mainForm.IsKortUsed(kortNr))
                        {
                            throw new CardAlreadyUsedException("Kortet har redan lösts in.");
                        }
                        else
                        {
                            // Skapar vinst baserat på korttypen från databasen
                            Reward reward = RewardFactory.CreateReward(kortInfo.KortTyp);

                            if (reward != null)
                            {
                                // Skickar vinstmeddelandet till simulatorn/klienten
                                await TryWriteStringAsync(writer, reward.GenerateMessage());

                                // Lägger till vinsten i listan i gränssnittet
                                mainForm.Invoke(new Action(() =>
                                    mainForm.AddRewardToList(reward, användarNr, kortNr)));
                            }
                            else
                            {
                                await TryWriteStringAsync(writer, "Koden är giltig, men ger ingen vinst.");

                                mainForm.Invoke(new Action(() =>
                                    mainForm.AddDebugLog($"Kort {kortNr} är giltigt men gav ingen vinst.")));
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

        // Läser exakt "count" tecken från strömmen, även om datan kommer i flera små delar/paket.
        // Detta behövs eftersom ReadAsync() kan returnera innan hela meddelandet hunnit anlända,
        // särskilt om avsändaren (t.ex. NOS_Export) skickar data i flera små bitar istället för allt på en gång.
        private async Task<int> ReadExactAsync(StreamReader reader, char[] buffer, int count)
        {
            int totalRead = 0;

            // Fortsätter läsa tills vi antingen fått ihop "count" tecken totalt, eller anslutningen stängs
            while (totalRead < count)
            {
                // Läser in nästa del av datan, med offset så vi inte skriver över det vi redan läst
                int read = await reader.ReadAsync(buffer, totalRead, count - totalRead);

                if (read == 0)
                {
                    // Anslutningen stängdes innan vi fick all förväntad data
                    break;
                }

                totalRead += read;
            }

            return totalRead;
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

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Guldkortet
{
    // Hanterar TCP-servern som tar emot kortkoder från NOS_Export.exe på port 12345.
    public class TcpManager
    {
        private DatabaseManager dbManager;
        private Form1 mainForm;

        // Properties som som behövs för att säkerställa TCP kopplingen.
        private int port = 12345;
        private IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        private TcpListener listener;
        private TcpClient client;
        private bool isRunning = false;

        // Skapa ett fält i toppen av klassen för att spara referensen
        private IProgress<string> _logProgress;
        private IProgress<RewardResult> _rewardProgress;

        // Konstruktor som tar emot gränssnittet (Form1)
        public TcpManager(Form1 form, DatabaseManager db)
        {
            mainForm = form;
            dbManager = db;
        }

        // Startar servern och börjar lyssna
        public async void StartServerAsync(IProgress<string> logProgress, IProgress<RewardResult> rewardProgress)
        {
            _logProgress = logProgress;
            _rewardProgress = rewardProgress;

            // Try-Catch sats som tar emot anslutningar
            try
            {
                listener = new TcpListener(localAddr, port);
                listener.Start();
                isRunning = true;

                // Loggar i GUI att servern startat
                _logProgress?.Report("Server startad!");

                while (isRunning) {
                    // await gör att programmet väntar här i bakgrunden tills en klient ansluter
                    client = await listener.AcceptTcpClientAsync();
                    _logProgress?.Report("Klient ansluten!");

                    // Startar hanteringen av klienten asynkront
                    StartReadingAsync(client);
                }
            }
            catch (Exception error)
            {
                if (isRunning)
                {
                    _logProgress?.Report("Kunde inte starta servern: " + error.Message);
                }
            }
        }

        // Hanterar den anslutna klienten NOS_Export
        private async void StartReadingAsync(TcpClient c)
        {
            // 3. Använd den överallt där du vill logga något
            _logProgress?.Report("Klient ansluten!");

            try
            {
                // Skapar en lokal nätverksström för just denna klient
                using (NetworkStream stream = c.GetStream())
                {
                    while (c.Connected)
                    {
                        byte[] indata = new byte[1024];
                        int n = await stream.ReadAsync(indata, 0, indata.Length);

                        if (n == 0)
                        {
                            // Klienten stängde anslutningen normalt
                            _logProgress?.Report("Klienten kopplade från.");
                            break;
                        }

                        // Omvandlar bytes till text
                        string rawData = Encoding.Unicode.GetString(indata, 0, n).Trim();


                        // Loggar i gränssnittet exakt vad simulatorn skickade
                        _logProgress?.Report($"Mottog rådata ({n} bytes): '{rawData}'");

                        // Validering och databashantering i ett eget try-catch inuti loopen
                        // så att anslutningen hålls öppen även om en kod var felaktig
                        try
                        {
                            if (rawData.Length != 19)
                            {
                                throw new InvalidCodeFormatException("Koden uppfyller inte det förväntade antalet tecken.");
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

                            // Söker upp kunden i databasen
                            Kund kundInfo = await dbManager.GetKundByNrAsync(användarNr);

                            if (kundInfo == null)
                            {
                                throw new InvalidCodeFormatException($"Kund nummer {användarNr} hittades inte i databasen.");

                            }

                            // Söker upp kortinfo i databasen
                            Kort kortInfo = await dbManager.GetKortByNrAsync(kortNr);

                            // 4.Hantera de olika utfallen baserat på kortets status
                            if (kortInfo == null)
                            {
                                throw new InvalidCodeFormatException($"Kort-ID {kortNr} hittades inte i databasen.");
                            }
                            // 1. KONTROLLERA OM KORTET REDAN ÄR ANVÄNT
                            else if (mainForm.IsKortUsed(kortNr))
                            {
                                throw new CardAlreadyUsedException($"Kortet {kortNr} har redan lösts in.");
                            }
                            else
                            {
                                // Skapar vinst baserat på korttypen från databasen
                                Reward reward = RewardFactory.CreateReward(kortInfo.KortTyp);

                                if (reward != null)
                                {
                                    // Skickar vinstmeddelandet till simulatorn/klienten
                                    await TryWriteStringAsync(c, reward.GenerateMessage());

                                    _rewardProgress?.Report(new RewardResult
                                    {
                                        Reward = reward,
                                        AnvändarNr = användarNr,
                                        KortNr = kortNr
                                    });

                                    // Lägger till vinsten i listan i gränssnittet
                                    _logProgress?.Report($"Vinst registrerad för kund {användarNr}!");
                                }
                                else
                                {
                                    await TryWriteStringAsync(c, "Koden är giltig, men ger ingen vinst.");
                                    _logProgress?.Report($"Kort {kortNr} är giltigt men gav ingen vinst.");
                                }
                            }
                        }
                        catch (InvalidCodeFormatException error)
                        {
                            // Sker om formatet är fel
                            await TryWriteStringAsync(c, "Fel: " + error.Message);
                            _logProgress?.Report("Formatfel: " + error.Message);
                        }
                        catch (CardAlreadyUsedException error)
                        {
                            await TryWriteStringAsync(c, "Fel: " + error.Message);
                            _logProgress?.Report("Kortet redan använt: " + error.Message);
                        }
                        catch (DatabaseConnectionException error)
                        {
                            // Sker om databasanslutningen eller frågan misslyckas
                            await TryWriteStringAsync(c, "Fel: " + error.Message);
                            _logProgress?.Report("Databasfel: " + error.Message);
                        }
                        catch (IOException error)
                        {
                            // Sker om klienten kopplade från oväntat
                            _logProgress?.Report($"[Nätverksfel] Klienten kopplades från oväntat: {error.Message}");
                        }
                        catch (SocketException error)
                        {
                            // Sker om socketen stängdes från klientens sida
                            _logProgress?.Report($"[Socketfel] Nätverksanslutningen bröts: {error.Message}");
                        }
                    }
                }
                
            }
            catch (Exception error)
            {
                // Loggar i gränssnittet att det skett ett undantag i servern
                _logProgress?.Report("Undantag i server: " + error.Message);
            }
            finally
            {
                c.Close();
                _logProgress?.Report("Klient frånkopplad.");
            }
        }

        // Hjälpmetod för att skriva säkert utan att krascha om klienten har kopplat från
        private async Task TryWriteStringAsync(TcpClient c, string message)
        {
            try
            {
                byte[] buffer = Encoding.Unicode.GetBytes(message + "\n");
                await c.GetStream().WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception error)
            {
                // Om klienten hann koppla från innan vi svarade ignorerar vi skrivfelet
                _logProgress?.Report("Fel vid sändning till klient: " + error.Message);
            }
        }

        // Stänger ner servern
        public void StopServer()
        {
            try
            {
                isRunning = false;
                listener?.Stop();
                _logProgress?.Report("Servern har stängts av.");
            }
            catch (Exception ex)
            {
                _logProgress?.Report("Fel vid avstängning: " + ex.Message);
            }
        }
    }
}

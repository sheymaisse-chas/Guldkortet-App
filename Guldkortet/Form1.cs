using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Guldkortet;

namespace Guldkortet
{
    public partial class Form1 : Form
    {
        // 1. Skapar en generisk lista för belöningar
        private MyRewardList rewardList = new MyRewardList();

        // 2. Detta är en referens till nätverkshanteraren som sköter nätverkskommunikationen (TCP)
        private ServerManager serverManager;

        private DatabaseManager dbManager = new DatabaseManager();

        // Lista för att hålla reda på vilka kort som redan använts
        private List<string> användaKort = new List<string>();

        public Form1()
        {
            InitializeComponent();

            // Skapar ServerManager och skicka med detta formulär
            serverManager = new ServerManager(this);
        }

        /* --- KNAPPAR & HANDLER-METODER --- */

        // Startar servern när användaren klickar på Starta-knappen.
        // Ändrar lblStatus så att den visar att servern är kopplad.
        private void btnStart_Click(object sender, EventArgs e)
        {
            serverManager.StartServer();
            lblStatus.Text = "Status: Online (Lyssnar på port 12345)";
            lblStatus.ForeColor = System.Drawing.Color.Green;
        }

        // Stoppa servern när användaren klickar på Stoppa-knappen
        // Ändrar lblStatus så att den visar att servern inte är kopplad.
        private void btnStop_Click(object sender, EventArgs e)
        {
            serverManager.StopServer();
            lblStatus.Text = "Status: Offline";
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }

        /* --- METOD SOM ANROPAS FRÅN SERVERMANAGER --- */

        // Tar emot ett godkänt belöningsobjekt från ServerManager,
        // sparar det i listan, uppdaterar gränssnittet och loggar till fil.
        public void AddRewardToList(Reward reward, string användarNr, string kortNr)
        {
            // Eftersom TCP körs i bakgrunden använder vi Invoke för att uppdatera UI säkert
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AddRewardToList(reward, användarNr, kortNr)));
                return;
            }

            // Markera kortet som använt
            if (!användaKort.Contains(kortNr))
            {
                användaKort.Add(kortNr);
            }

            // 1. Sparar list-objektet i minnet
            rewardList.Add(reward);

            // 2. Skapar presentationstext via polymorfism
            string displayInfo = $"[Kund: {användarNr}] - {reward.GenerateMessage()}";

            // 3. Visar i gränssnittets ListBox
            lstRewards.Items.Add(displayInfo);

            // 4. Loggar händelsen till log.txt
            LogToFile(användarNr, reward);
        }

        // Metod som kollar om kortnumret redan finns i listan
        public bool IsKortUsed(string kortNr)
        {
            return användaKort.Contains(kortNr);
        }

        // Felsökningsmetod som visar meddelanden i GUI och loggar fel till log.txt
        public void AddDebugLog(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AddDebugLog(message)));
                return;
            }

            lstRewards.Items.Add($"[LOGG {DateTime.Now:HH:mm:ss}] {message}");

            // Om det är ett felmeddelande sparas det i log.txt
            if (message.Contains("Fel") || message.Contains("Undantag") || message.Contains("Formatfel"))
            {
                LogToFile($"FEL: {message}");
            }
        }

        /* --- FILHANTERING (log.txt enligt avsnitt 5.2) --- */

        // Loggar godkända vinster med tidsstämpel till log.txt
        private void LogToFile(string användarNr, Reward reward)
        {
            try
            {
                string logPath = "log.txt";
                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logMessage = $"[{timeStamp}] Användare: {användarNr} löste in kort -> Belöning: {reward.Name}";

                // Använder StreamWriter med append: true för att bevara tidigare data
                using (StreamWriter writer = new StreamWriter(logPath, true, Encoding.UTF8))
                {
                    writer.WriteLine(logMessage);
                }
            }
            // Om programmet inte kan logga eller avbryts i processen
            catch (Exception ex)
            {
                MessageBox.Show("Kunde inte skriva till loggfilen: " + ex.Message);
            }
        }

        private void LogToFile(string logText)
        {
            try
            {
                string logPath = "log.txt";
                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logMessage = $"[{timeStamp}] {logText}";

                // Använder StreamWriter med append: true för att bevara tidigare data
                using (StreamWriter writer = new StreamWriter(logPath, true, Encoding.UTF8))
                {
                    writer.WriteLine(logMessage);
                }
            }
            // Om programmet inte kan logga eller avbryts i processen
            catch (Exception ex)
            {
                MessageBox.Show("Kunde inte skriva till loggfilen: " + ex.Message);
            }
        }

        // Stänger servern säkert om fönstret stängs
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serverManager.StopServer();

            // Skriver ut hela listan med belöningar till en textfil när programmet avslutas
            WriteRewardListToFile();
        }

        // Metod som skriver ut hela listan med belöningar till en textfil när programmet avslutas
        private void WriteRewardListToFile()
        {
            try
            {
                string filePath = "rewardlist.txt";
                Reward[] allRewards = rewardList.GetAll();

                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    foreach (Reward reward in allRewards)
                    {
                        writer.WriteLine(reward.GenerateMessage());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunde inte skriva belöningslistan till fil: " + ex.Message);
            }
        }

        /* --- KNAPPAR: DATABASHANTERING (GUI) --- */

        private void btnUpdateKund_Click(object sender, EventArgs e)
        {
            // 1. Hämtar texten från textrutorna och rensar eventuella överflödiga mellanslag
            string användarNr = txtAnvandarNr.Text.Trim();
            string nyttNamn = txtNamn.Text.Trim();
            string nyKommun = txtKommun.Text.Trim();

            // 2. Validering: Kontrollerar att inget fält lämnats tomt
            if (string.IsNullOrEmpty(användarNr) || string.IsNullOrEmpty(nyttNamn) || string.IsNullOrEmpty(nyKommun))
            {
                MessageBox.Show("Fyll i alla tre fält (Användarnummer, Namn och Kommun) för att uppdatera.",
                                "Inmatning saknas",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 3. Anropar databasklassen för att genomföra uppdateringen
                bool lyckades = dbManager.UpdateKund(användarNr, nyttNamn, nyKommun);

                if (lyckades)
                {
                    MessageBox.Show($"Uppgifterna för kund {användarNr} har uppdaterats!",
                                    "Uppdatering genomförd",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                    // Rensar fälten efter lyckad uppdatering
                    txtAnvandarNr.Clear();
                    txtNamn.Clear();
                    txtKommun.Clear();
                }
                else
                {
                    MessageBox.Show($"Ingen kund med användarnummer '{användarNr}' hittades i databasen.",
                                    "Kunden hittades inte",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (DatabaseConnectionException ex)
            {
                MessageBox.Show("Databasfel: " + ex.Message,
                                "Anslutningsfel",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ett oväntat fel uppstod: " + ex.Message,
                                "Systemfel",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        // --- KNAPP: LÄGGA TILL NY KUND ---
        private void btnAddKund_Click(object sender, EventArgs e)
        {
            string id = txtAnvandarNr.Text.Trim();
            string namn = txtNamn.Text.Trim();
            string kommun = txtKommun.Text.Trim();

            // Enkel validering
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(namn) || string.IsNullOrEmpty(kommun))
            {
                MessageBox.Show("Fyll i alla fält innan du sparar.", "Indata saknas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool success = dbManager.InsertKund(id, namn, kommun);

                if (success)
                {
                    MessageBox.Show("Kunden sparades i databasen!", "Framgång", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Rensa textrutorna efter lyckad sparning
                    txtAnvandarNr.Clear();
                    txtNamn.Clear();
                    txtKommun.Clear();
                }
                else
                {
                    MessageBox.Show("Kunden kunde inte sparas.", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (DatabaseConnectionException ex)
            {
                MessageBox.Show("Databasfel: " + ex.Message, "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
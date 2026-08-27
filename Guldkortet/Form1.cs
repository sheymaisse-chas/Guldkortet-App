using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Guldkortet
{
    public partial class Form1 : Form
    {
        // 1. Skapar en generisk lista för belöningar
        private List<Reward> rewardList = new List<Reward>();

        // 2. Detta är en referens till nätverkshanteraren som sköter nätverkskommunikationen (TCP)
        private ServerManager serverManager;
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
        public void AddRewardToList(Reward reward, string userId)
        {
            // Eftersom TCP körs i bakgrunden använder vi Invoke för att uppdatera UI säkert
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AddRewardToList(reward, userId)));
                return;
            }

            // 1. Sparar list-objektet i minnet
            rewardList.Add(reward);

            // 2. Skapar presentationstext via polymorfism
            string displayInfo = $"[Kund: {userId}] - {reward.Name}: {reward.GenerateMessage()}";

            // 3. Visar i gränssnittets ListBox
            lstRewards.Items.Add(displayInfo);

            // 4. Loggar händelsen till log.txt
            LogToFile(userId, reward);
        }

        /* --- FILHANTERING (log.txt enligt avsnitt 5.2) --- */

        // Loggar godkända vinster med tidsstämpel till log.txt
        private void LogToFile(string userId, Reward reward)
        {
            try
            {
                string logPath = "log.txt";
                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logMessage = $"[{timeStamp}] Användare: {userId} löste in kort -> Belöning: {reward.Name}";

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
        }
    }
}
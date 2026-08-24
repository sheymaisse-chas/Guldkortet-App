using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Guldkortet
{
    // RewardFactory hanterar belöningen.
    // Den tar enot textsträngen från databasen och returnerar rätt objekt.
    public static class RewardFactory
    {

        // Läser in bokdata från textfil och skapar bokobjekt
        public static Reward CreateReward(string rewardTypeFromDb)
        {
            // Switch-satsen matchar databasens text mot programmets Reward underklasser
            switch (rewardTypeFromDb)
            {
                case "Eldtomat":
                    return new FireHound();
                case "Dunderkatt":
                    return new ThunderCat();
                case "Överpanda":
                    return new FlyingSquirrel();
                case "Kristallhäst":
                    return new CrystalSteed();

                default:
                    // Detta sker om strängen inte matchar något känt namn
                    throw new ArgumentException($"Okänd belöningstyp: {rewardTypeFromDb}");
            }
            /*
                // 1. Låtsas att vi hämtat strängen "Eldtomat" från databasen
                string dbRewardString = "Eldtomat"; 

                // 2. Skapa objektet via fabriken
                Reward newCard = RewardFactory.CreateReward(dbRewardString);

                // 3. Lägg till i listan över belöningar
                myRewardList.Add(newCard);

                // 4. Visa meddelandet på skärmen (körs automatiskt via polymorfism!)
                Console.WriteLine(newCard.GenerateMessage());
             */

        }
    }
}

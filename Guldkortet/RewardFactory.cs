using System;

namespace Guldkortet
{
    // RewardFactory hanterar belöningen.
    // Den tar enot textsträngen från databasen och returnerar rätt objekt.
    public static class RewardFactory
    {

        // Läser in bokdata från textfil och skapar kort-objekt
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
                    // Returnerar null om koden är giltig men saknar vinst i fabriken.
                    return null;
            }
        }
    }
}

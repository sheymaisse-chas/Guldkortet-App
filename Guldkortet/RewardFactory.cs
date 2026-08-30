using System;

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
                case "FireHound":
                    return new FireHound();
                case "ThunderCat":
                    return new ThunderCat();
                case "FlyingSquirrel":
                    return new FlyingSquirrel();
                case "CrystalSteed":
                    return new CrystalSteed();

                default:
                    // Detta sker om strängen inte matchar något känt namn
                    throw new ArgumentException($"Okänd belöningstyp: {rewardTypeFromDb}");
            }
        }
    }
}

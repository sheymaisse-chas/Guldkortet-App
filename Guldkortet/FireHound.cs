using System;

namespace Guldkortet
{
    // Underklassen Eldhund och dess properties
    public class FireHound : Reward
    {
        // Unika properties för Eldhunds data
        public int Speed { get; set; } = 105;
        public string Element { get; set; } = "Fire";
        public int BarkVolume { get; set; } = 120;
        public bool IsImmuneToFrost { get; set; } = true;

        // Standardkonstruktor som sätter fördefinierat namn och beskrivning
        public FireHound() : base("Eldhund", "En varm och lojal hund av ren eld!")
        {
        }

        // Konstruktor för att skapa en instans med anpassade värden
        public FireHound(int speed, string element, int barkVolume, bool isImmuneToFrost, string name, string description) : base(name, description)
        {
            Speed = speed;
            Element = element;
            BarkVolume = barkVolume;
            IsImmuneToFrost = isImmuneToFrost;
        }

        // En override metod som skriver ut en anpassad text för denna underklass
        public override string GenerateMessage()
        {
            return $"Grattis! Du har vunnit {Name}: Element - {Element}, Hastighet - {Speed} km/h, Skallvolym - {BarkVolume}. Frostimmunitet - {IsImmuneToFrost}, {Description}";
        }
    }
}

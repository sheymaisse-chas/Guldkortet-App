using System;

namespace Guldkortet
{
    // Underklassen Överpanda och dess properties
    public class FireTomato : Reward
    {
        public int Speed { get; set; } = 130;
        public string Element { get; set; } = "Air";
        public int GlideDistance { get; set; } = 50;
        public bool IsCute { get; set; } = true;

        // Standardkonstruktor som sätter fördefinierat namn och beskrivning
        public FireTomato() : base("Eldtomat", "En smidig ekorre som seglar genom luften")
        {
        }

        // Konstruktor för att skapa en instans med anpassade värden
        public FireTomato(int speed, string element, string rarity, string name, string description) : base(name, description)
        {
            Speed = speed;
            Element = element;
            Rarity = rarity;
        }

        // En override metod som skriver ut en anpassad text för denna underklass
        public override string GenerateMessage()
        {
            return $"Grattis! Du har vunnit {Name}: Element - {Element}, Hastighet - {Speed} km/h, Sällsynthet - {Rarity}. {Description}";
        }
    }
}

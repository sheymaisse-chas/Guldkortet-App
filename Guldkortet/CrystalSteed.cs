using System;

namespace Guldkortet
{
    // Underklassen Kristallhäst och dess properties
    public class CrystalSteed : Reward
    {
        public int Speed { get; set; } = 145;
        public string Element { get; set; } = "Kristall";
        public string Rarity { get; set; } = "Sällsynt";

        // Standardkonstruktor som sätter fördefinierat namn och beskrivning
        public CrystalSteed() : base("Kristallhäst", "En stolt häst gjord av ren kristall!")
        {
        }

        // Konstruktor för att skapa en instans med anpassade värden
        public CrystalSteed(int speed, string element, string rarity, string name, string description) : base(name, description)
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

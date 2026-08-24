using System;

namespace Guldkortet
{
    // Underklassen Överpanda och dess properties
    public class OverPanda : Reward
    {
        // Properties för Överpandas datan
        public int Speed { get; set; } = 95;
        public string Element { get; set; } = "None";
        public string Rarity { get; set; } = "vanlig";
        public string SpecialAbility { get; set; } = "Super styrka";

        // Standardkonstruktor som sätter fördefinierat namn och beskrivning
        public OverPanda() : base("Överpanda", "En stolt häst gjord av ren kristall!")
        {
        }

        // Konstruktor för att skapa en instans med anpassade värden
        public OverPanda(int speed, string element, string specialAbility, string name, string description) : base(name, description)
        {
            Speed = speed;
            Element = element;
            SpecialAbility = specialAbility;
        }

        // En override metod som skriver ut en anpassad text för denna underklass
        public override string GenerateMessage()
        {
            return $"Grattis! Du har vunnit {Name}: Element - {Element}, Hastighet - {Speed} km/h, Special förmåga - {SpecialAbility}. {Description}";
        }
    }
}

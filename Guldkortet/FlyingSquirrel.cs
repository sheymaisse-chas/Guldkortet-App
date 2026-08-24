using System;
using System.ComponentModel;

namespace Guldkortet
{
    // Underklassen FlygandeEkorre och dess properties
    public class FlyingSquirrel : Reward
    {
        // Unika properties för FlygandeEkorre datan
        public int Speed { get; set; } = 130;
        public string Element { get; set; } = "Air";
        public int FlySpeed { get; set; } = 50;
        public bool IsCute { get; set; } = true;

        // Standardkonstruktor som sätter fördefinierat namn och beskrivning
        public FlyingSquirrel() : base("Flygande ekorre", "En smidig ekorre som seglar genom luften!")
        {
        }

        // Konstruktor för att skapa en instans med anpassade värden
        public FlyingSquirrel(int speed, string element, int flySpeed, bool isCute, string name, string description) : base(name, description)
        {
            Speed = speed;
            Element = element;
            FlySpeed = flySpeed;
            IsCute = isCute;
        }

        // En override metod som skriver ut en anpassad text för denna underklass
        public override string GenerateMessage()
        {
            return $"Grattis! Du har vunnit {Name}: Element - {Element}, Flyghastighet - {FlySpeed} km/h, Är den gullig - {IsCute}. {Description}";
        }
    }
}

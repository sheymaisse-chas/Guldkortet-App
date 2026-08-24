using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Guldkortet
{
    public class ThunderCat : Reward
    {
        public int Speed { get; set; } = 120;
        public string Element { get; set; } = "Blixt";
        public string SpecialAttack { get; set; } = "Eld kula";

        public ThunderCat (int speed, string element, string specialAttack, string name, string description) : base (name, description)
        {
            Speed = speed;
            Element = element;
            SpecialAttack = specialAttack;
        }
        public override string GenerateMessage()
        {
            return $"Grattis! Du har vunnit {Name}: {Description}";
        }
    }
}

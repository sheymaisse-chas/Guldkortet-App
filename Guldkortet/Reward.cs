using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guldkortet
{
    // Belöning klassen och dess properties
    public class Reward
    {
        // Properties för belönings datan
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        // Tom konstruktor så underklasserna enkelt kan sätta Name och Description själva
        public Reward() { }

        public Reward (string name, string description, string imagePath)
        {
            Name = name;
            Description = description;
            ImagePath = imagePath;
        }
        
        // En virtual metod som genererar texten som kommer visas på gränssnittet
        public virtual string GenerateMessage()
        {
            return $"Grattis! Du har vunnit {Name}: {Description}";
        }
    }
}

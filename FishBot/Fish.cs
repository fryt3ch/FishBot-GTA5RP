using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishBot
{
    public class Fish
    {
        public string Name { get; }
        public int MinCost { get; set; }
        public int MaxCost { get; set; }
        public int Amount { get; set; }
        public double Weight { get; set; }
        public string Chance { get; set; }

        public Fish(string Name)
        {
            this.Name = Name;

            MinCost = 0;
            MaxCost = 0;

            Amount = 0;

            Weight = 0.0;

            Chance = "0,00";
        }

        public Fish() { }
    }
}

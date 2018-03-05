using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabiticaTravel.Models.Habitica.UserStats
{

    public class UserStats
    {
        public Training Training { get; set; }
        public Buffs Buffs { get; set; }
        public int Per { get; set; }
        public int _int { get; set; }
        public int Con { get; set; }
        public int Str { get; set; }
        public int Points { get; set; }
        public string _class { get; set; }
        public int Lvl { get; set; }
        public int Gp { get; set; }
        public int Exp { get; set; }
        public int Mp { get; set; }
        public int Hp { get; set; }
        public int ToNextLevel { get; set; }
        public int MaxHealth { get; set; }
        public int MaxMP { get; set; }
    }

    public class Training
    {
        public int Con { get; set; }
        public int Str { get; set; }
        public int Per { get; set; }
        public int _int { get; set; }
    }

    public class Buffs
    {
        public bool Seafoam { get; set; }
        public bool ShinySeed { get; set; }
        public bool SpookySparkles { get; set; }
        public bool Snowball { get; set; }
        public bool Streaks { get; set; }
        public int Stealth { get; set; }
        public int Con { get; set; }
        public int Per { get; set; }
        public int _int { get; set; }
        public int Str { get; set; }
    }

}
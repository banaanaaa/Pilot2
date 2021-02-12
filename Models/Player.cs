using System;
using System.Collections.Generic;

namespace Pilot2.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public ICollection<string> Words;

        public bool inGame;
    }
}

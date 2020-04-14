using System;
using System.Collections.Generic;
using System.Text;

namespace IdleDiscordGame.classes
{
    public class CharacterStats
    {
        public DateTimeOffset CreatedAt { get; set;  }
        public ulong TotalExperience { get; set; }
        public  DateTimeOffset LastUpdatedDateTime { get; set; }

    }
}

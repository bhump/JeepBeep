using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{
    public class JeepProfiles
    {
        public int JeepProfileId { get; set; }

        public string Manufactuer { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public short Year { get; set; }

        public bool PrimaryJeep { get; set; }

        public string JeepDescription { get; set; }

        public byte[] Image { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{
    public class UserProfiles
    {
        public int UserProfileId { get; set; }

        public int JeepProfileId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public byte[] Avatar { get; set; }

        public string Description { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string Ello { get; set; }

        public string GooglePlus { get; set; }

        public string Website { get; set; }

        public string UserName { get; set; }

        public string Instagram { get; set; }
    }
}

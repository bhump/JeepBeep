using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{
    public class FriendsList
    {
        public string UserId {get; set;}

        public int UserProfileId {get; set;}

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool Pending { get; set; }

        public bool Accepted { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{
    public class UserStatus
    {
        public int StatusId { get; set; }

        public string Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DateCreated { get; set; }

        public long? LikeCount { get; set; }

        public long DislikeCount { get; set; }

        public string MediaLink { get; set; }

        public string UserName { get; set; }

        public byte[] Avatar { get; set; }
    }
}

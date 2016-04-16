using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{
    public class UsersPosts
    {
        public int PostId { get; set; }

        public string PostTitle { get; set; }

        public string PostDescription { get; set; }

        public string PartType { get; set; }

        public string PartBrand { get; set; }

        public string UserName { get; set; }

        public byte[] Avatar { get; set; }

        public string PartName { get; set; }

        public Boolean ?IsVehicle { get; set; }

        public string Make { get; set; }

        public IEnumerable<string> Model { get; set; }

        public string SubModel { get; set; }

        public string Year { get; set; }

        public Boolean ?Active { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostType { get; set; }

        public int LikeCount { get; set; }

        public int DislikeCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime ?DateCreated { get; set; }
    }
}

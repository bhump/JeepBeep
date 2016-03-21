using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{
    public class Comments
    {
        public int CommentId { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Comment { get; set; }

        public List<Byte> Images { get; set; }
    }
}

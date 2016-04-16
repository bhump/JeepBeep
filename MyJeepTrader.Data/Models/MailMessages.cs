using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{
    public class MailMessages
    {
        public MailMessages()
        {
        }

        public int MessageId { get; set; }

        public string To { get; set; }

        public string From { get; set; }

        public string ToUserId { get; set; }

        public string FromUserId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public DateTime? DateSent { get; set; }

        public DateTime? DateRead { get; set; }
    }
}

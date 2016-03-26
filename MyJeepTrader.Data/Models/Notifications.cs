using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{
    public class Notifications
    {
        public int NotificationId { get; set; }

        public string UserName { get; set; }

        public string FromUserName { get; set; }

        public int? StatusId { get; set; }

        public int? CommentId { get; set; }

        public int? MessageId { get; set; }

        public string Status { get; set; }

        public DateTime? StatusDate { get; set; }

        public int StatusControlId { get; set; }

        public int? FriendListId { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateRead { get; set; }

        public string Comment { get; set; }

        public string OriginalMessage { get; set; }

        public string Message { get; set; }

        public string MessageSubject { get; set; }

        public string LikedBy { get; set; }

        public string DislikedBy { get; set; }

        public bool FriendPending { get; set; }

        public byte[] Avatar { get; set; }
    }
}

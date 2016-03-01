using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{
    //public class LiveStream
    //{
        //public byte[] Avatar { get; set; }

        //public string Status { get; set; }

        //public string UserName { get; set; }
    //}

    public abstract class LiveFeed
    {
        public abstract string FeedUserName { get; }

        public abstract string FeedDescription { get; }

        public abstract bool IsFeedPost { get; }
    }

    public class LivePost : LiveFeed
    {
        public string UserName { get; set; }

        public string PostDescription { get; set; }

        public bool IsPost { get; set; }

        public override string FeedUserName
        {
            get { return this.UserName; }
        }

        public override string FeedDescription
        {
            get { return this.PostDescription; }
        }

        public override bool IsFeedPost
        {
            get { return this.IsPost; }
        }
    }

    public class LiveStream : LiveFeed
    {
        public string UserName { get; set; }

        public string Status { get; set; }

        public bool IsPost { get; set; }

        public override string FeedUserName
        {
            get { return this.UserName; }
        }

        public override string FeedDescription
        {
            get { return this.Status; }
        }

        public override bool IsFeedPost
        {
            get { return this.IsPost; }
        }
    }
}

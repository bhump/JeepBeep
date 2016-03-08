﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJeepTrader.Data.Models
{

    public abstract class LiveFeed
    {
        public abstract string FeedUserName { get; }

        public abstract string FeedDescription { get; }

        public abstract bool IsFeedPost { get; }

        public abstract int FeedId { get; }

        public abstract byte[] FeedAvatar { get; }

        public abstract DateTime FeedDateCreated { get; }
    }

    public class LivePost : LiveFeed
    {
        public string UserName { get; set; }

        public string PostDescription { get; set; }

        public bool IsPost { get; set; }

        public int PostId { get; set; }

        public byte[] Avatar { get; set; }

        public DateTime DateCreated { get; set; }

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

        public override int FeedId
        {
            get { return this.PostId; }
        }

        public override byte[] FeedAvatar
        {
            get { return this.Avatar; }
        }

        public override DateTime FeedDateCreated
        {
            get { return this.DateCreated; }
        }
    }

    public class LiveStream : LiveFeed
    {
        public string UserName { get; set; }

        public string Status { get; set; }

        public bool IsPost { get; set; }

        public int StatusId { get; set; }

        public byte[] Avatar { get; set; }

        public DateTime DateCreated { get; set; }

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

        public override int FeedId
        {
            get { return this.StatusId; }
        }

        public override byte[] FeedAvatar
        {
            get { return this.Avatar; }
        }

        public override DateTime FeedDateCreated
        {
            get { return this.DateCreated; }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyJeepTrader.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class tNotification
    {
        public int NotificationId { get; set; }
        public string Id { get; set; }
        public string FromUserId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> CommentId { get; set; }
        public Nullable<int> MessageId { get; set; }
        public Nullable<int> StatusControlId { get; set; }
        public Nullable<int> FriendListId { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateRead { get; set; }
        public Nullable<int> MentionId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
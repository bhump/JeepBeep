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
    
    public partial class tFriendsList
    {
        public int FriendListId { get; set; }
        public string Id { get; set; }
        public string FriendId { get; set; }
        public bool Pending { get; set; }
        public bool Accepted { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}

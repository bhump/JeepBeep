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
    
    public partial class tMembership
    {
        public int MembershipId { get; set; }
        public Nullable<bool> PremiumMembership { get; set; }
        public Nullable<bool> BasicMembership { get; set; }
        public Nullable<System.DateTime> MemberSince { get; set; }
        public Nullable<bool> Expired { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<bool> AutoRenew { get; set; }
        public Nullable<bool> Renewed { get; set; }
        public string Id { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}

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
    
    public partial class tSubscription
    {
        public tSubscription()
        {
            this.tMemberships = new HashSet<tMembership>();
        }
    
        public int SubscriptionId { get; set; }
        public string Subscription { get; set; }
    
        public virtual ICollection<tMembership> tMemberships { get; set; }
    }
}

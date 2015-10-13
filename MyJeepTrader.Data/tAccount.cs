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
    
    public partial class tAccount
    {
        public tAccount()
        {
            this.tPostsControls = new HashSet<tPostsControl>();
            this.tUserProfiles = new HashSet<tUserProfile>();
            this.tVehicleProfileControls = new HashSet<tVehicleProfileControl>();
        }
    
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    
        public virtual tMembership tMembership { get; set; }
        public virtual ICollection<tPostsControl> tPostsControls { get; set; }
        public virtual ICollection<tUserProfile> tUserProfiles { get; set; }
        public virtual ICollection<tVehicleProfileControl> tVehicleProfileControls { get; set; }
    }
}
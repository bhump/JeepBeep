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
    
    public partial class tTestingCode
    {
        public tTestingCode()
        {
            this.AspNetUsers = new HashSet<AspNetUser>();
        }
    
        public int CodeId { get; set; }
        public string Code { get; set; }
        public Nullable<int> Count { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}

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
    
    public partial class tMessage
    {
        public tMessage()
        {
            this.tMessageControls = new HashSet<tMessageControl>();
        }
    
        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> DateSent { get; set; }
        public Nullable<System.DateTime> DateRead { get; set; }
        public Nullable<bool> IsIM { get; set; }
    
        public virtual ICollection<tMessageControl> tMessageControls { get; set; }
    }
}

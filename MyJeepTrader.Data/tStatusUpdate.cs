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
    
    public partial class tStatusUpdate
    {
        public tStatusUpdate()
        {
            this.tStatusMedias = new HashSet<tStatusMedia>();
        }
    
        public int StatusId { get; set; }
        public string Id { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<long> LikeCount { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual ICollection<tStatusMedia> tStatusMedias { get; set; }
    }
}

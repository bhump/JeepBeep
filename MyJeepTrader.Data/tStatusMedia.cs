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
    
    public partial class tStatusMedia
    {
        public int MediaId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string MediaLink { get; set; }
        public byte[] Image { get; set; }
    
        public virtual tStatusUpdate tStatusUpdate { get; set; }
    }
}

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
    
    public partial class tPostsControl
    {
        public int PostControlId { get; set; }
        public Nullable<int> PostId { get; set; }
        public string Id { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual tPost tPost { get; set; }
    }
}

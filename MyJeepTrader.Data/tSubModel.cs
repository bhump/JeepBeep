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
    
    public partial class tSubModel
    {
        public tSubModel()
        {
            this.tPosts = new HashSet<tPost>();
        }
    
        public int SubModelId { get; set; }
        public string SubModel { get; set; }
        public Nullable<int> ModelId { get; set; }
    
        public virtual tModel tModel { get; set; }
        public virtual ICollection<tPost> tPosts { get; set; }
    }
}
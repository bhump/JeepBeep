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
    
    public partial class tStatusComment
    {
        public tStatusComment()
        {
            this.tStatusCommentMedias = new HashSet<tStatusCommentMedia>();
        }
    
        public int CommentId { get; set; }
        public string Comment { get; set; }
        public string Id { get; set; }
        public Nullable<int> StatusId { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual ICollection<tStatusCommentMedia> tStatusCommentMedias { get; set; }
        public virtual tStatusUpdate tStatusUpdate { get; set; }
    }
}
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
    
    public partial class tStatusCommentMedia
    {
        public int CommentMediaId { get; set; }
        public byte[] Image { get; set; }
        public string MediaLink { get; set; }
        public Nullable<int> CommentId { get; set; }
    
        public virtual tStatusComment tStatusComment { get; set; }
    }
}
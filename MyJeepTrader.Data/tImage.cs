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
    
    public partial class tImage
    {
        public tImage()
        {
            this.tImageControls = new HashSet<tImageControl>();
        }
    
        public int Imageid { get; set; }
        public byte[] Image { get; set; }
    
        public virtual ICollection<tImageControl> tImageControls { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HabiticaTravel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TravelGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TravelGroup()
        {
            this.TravelGroupUsers = new HashSet<TravelGroupUser>();
        }
    
        public int TravelGroupId { get; set; }
        public string TravelGroupName { get; set; }
        public string Destination { get; set; }
        public string TravelMethod { get; set; }
        public string GroupLeader { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TravelGroupUser> TravelGroupUsers { get; set; }
    }
}

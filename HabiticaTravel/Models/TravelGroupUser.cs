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
    
    public partial class TravelGroupUser
    {
        public int TravelGroupUsersId { get; set; }
        public string UserId { get; set; }
        public int TravelGroupId { get; set; }
        public Nullable<bool> UserGroupRole { get; set; }
        public Nullable<int> UserGroupScore { get; set; }
    
        public virtual TravelGroup TravelGroup { get; set; }
    }
}

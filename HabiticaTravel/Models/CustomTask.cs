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
    
    public partial class CustomTask
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CustomTask()
        {
            this.CustomTaskItems = new HashSet<CustomTaskItem>();
        }
    
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskType { get; set; }
        public string TaskTag { get; set; }
        public string TaskNotes { get; set; }
        public Nullable<System.DateTime> TaskDueDate { get; set; }
        public double TaskDifficulty { get; set; }
        public string ReminderId { get; set; }
        public Nullable<System.DateTime> ReminderStartDate { get; set; }
        public Nullable<System.DateTime> ReminderTime { get; set; }
        public string TaskFrequency { get; set; }
        public string TaskRepeat { get; set; }
        public Nullable<int> TaskStreak { get; set; }
        public Nullable<System.DateTime> TaskStartDate { get; set; }
        public Nullable<bool> TaskHabitUp { get; set; }
        public Nullable<bool> TaskHabitDown { get; set; }
        public Nullable<int> TaskRewardValue { get; set; }
        public Nullable<int> EveryXDays { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomTaskItem> CustomTaskItems { get; set; }
    }
}

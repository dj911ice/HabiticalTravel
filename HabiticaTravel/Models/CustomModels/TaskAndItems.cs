using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabiticaTravel.Models
{
    public partial class TaskAndItem
    {
        public CustomTask CustomTask { get; set; }
        public CustomTaskItem CustomTaskItem { get; set; }
    }
}
using HabiticaTravel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabiticaTravel.ViewModel
{

    public class TaskAndItems //viewmodel of task and taskItem
    {
        public CustomTask CustomTask { get; set; }
        public IEnumerable<CustomTaskItem> CustomTaskItem { get; set; }
    }
}
using HabiticaTravel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabiticaTravel.ViewModel
{

    public class TaskAndItems //viewmodel of book and News :)
    {
        public IEnumerable<CustomTask> CustomTask { get; set; }
        public IEnumerable<CustomTaskItem> CustomTaskItem { get; set; }
    }
}
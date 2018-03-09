using HabiticaTravel.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HabiticaTravel.ViewModel
{

    public class TaskAndItems //viewmodel of task and taskItem
    {
        public CustomTask CustomTask { get; set; }
        public List<CustomTaskItem> CustomTaskItem { get; set; }
        public SelectList Items { get; set; } = new SelectList(
        new List<SelectListItem>
        {
            new SelectListItem { Value = "Habbit"},
            new SelectListItem { Value = "Daily"},
            new SelectListItem { Value = "ToDo" },
            new SelectListItem { Value = "Reward" },
        });
    }

}
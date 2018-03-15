using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HabiticaTravel.Models
{


    public class Challenge
    {
        public string taskId { get; set; }
        public string id { get; set; }
    }

    public class Group
    {
        public object[] assignedUsers { get; set; }
        public Approval approval { get; set; }
    }

    public class Approval
    {
        public bool required { get; set; }
        public bool approved { get; set; }
        public bool requested { get; set; }
    }

    public class Checklist
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool completed { get; set; }
    }

    public class UsersGroups
    {
        public List<TravelGroup> TravelGroups { get; set; }
        public string UserName { get; set; }
    }

    public class GroupUserAndName
    {
        public TravelGroupUser TGUser { get; set; }
        public string UserName { get; set; }
    }

    public class SendTask
    {
        public string text { get; set; }
        public string type { get; set; }
        public string[] tags { get; set; }
        public string notes { get; set; }
        public string date { get; set; }
        public Reminder[] reminders { get; set; }
        public string priority { get; set; }
    }

    public class Reminder
    {
        public string id { get; set; }
        public string startDate { get; set; }
        public string time { get; set; }
    }

}
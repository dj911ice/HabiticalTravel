using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HabiticaTravel.Models
{

    public class Rootobject
    {
        public bool success { get; set; }
        public Data data { get; set; }
        public object[] notifications { get; set; }
    }

    public class Data
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string userId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string text { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string alias { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string notes { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object[] tags { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int value { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double priority { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime date { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string attribute { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Challenge challenge { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Group group { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object[] reminders { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime createdAt { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime updatedAt { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Checklist[] checklist { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool collapseChecklist { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool completed { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }
    }

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

}
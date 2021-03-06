﻿using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HabiticaTravel.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    public class TaskAndItems //viewmodel of task and taskItem
    {
        public CustomTask CustomTask { get; set; }
        public List<CustomTaskItem> CustomTaskItem { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class TravelGroupandUser
    {
        public TravelGroup TravelGroup { get; set; }
        public TravelGroupUser TravelGroupUser { get; set; }
        public string UserName { get; set; }
        [Display(Name = "User Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

    }

    public class TravelGroupandUserTaskandItems
    {
        public TravelGroupandUser TravelGroupandUser { get; set; }
        public TaskAndItems TaskAndItems { get; set; }
        public List<TravelGroupUser> Users { get; set; }
        public List<TaskAndItems> ManyTaskAndItemsList { get; set; }
    }

    public class TravelGroupVM
    {

        public int TravelGroupId { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        [StringLength(20, ErrorMessage = "Please enter a valid group name between 1-20 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9-_ ])*$", ErrorMessage = "Please enter letters, numbers, Special Characters allowed: - _")]
        public string TravelGroupName { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        [RegularExpression(@"^[0-9]{5}$", ErrorMessage = "Please enter the 5 digit zip-code of your destination.")]
        public string Destination { get; set; }

        [Required]
        [Display(Name = "Travel Method")]
        public string TravelMethod { get; set; }

        public string GroupLeader { get; set; }
    }

    public class CustomTaskVM
    {

        public int TaskId { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Please enter a valid name for the task. between 1-20 characters.")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Please enter a valid task name. Letters ONLY")]
        public string TaskName { get; set; }

        public string TaskType { get; set; }
        public string TaskTag { get; set; }
        public string TaskNotes { get; set; }

        [Required]
        public Nullable<System.DateTime> TaskDueDate { get; set; }

        [Required]
        public double TaskDifficulty { get; set; }

        [Required]
        public Nullable<System.DateTime> ReminderStartDate { get; set; }

        [Required]
        public Nullable<System.DateTime> ReminderTime { get; set; }
        public string UserId { get; set; }
        public string HabiticaTaskId { get; set; }
        public Nullable<int> TravelGroupId { get; set; }
    }

    public class groupDetails
    {
        public TravelGroupVM TravelGroupVM { get; set; }

        
    }

}
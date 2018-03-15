using Flurl.Http;
using HabiticaTravel.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Flurl.Http.Configuration;

namespace HabiticaTravel.Utility
{
    public static class HabiticaHTTP
    {
        public static async Task<dynamic> PostUserLogin(string username, string password)
        {
            try
            {
                return await $"https://habitica.com/api/v3/user/auth/local/login"
                        .PostJsonAsync(new
                        {
                            username,
                            password,

                        }).ReceiveJson();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

        public static async Task<string> PostResetPassword(ApplicationUser user, ForgotPasswordViewModel model)

        {
            return await "https://habitica.com/api/v3/user/reset-password"
                .PostUrlEncodedAsync(new
                {
                    email = user.Email,
                })
                .ReceiveString();
        }

        public static async Task<dynamic> PostRegisterNewUser(ApplicationUser user, RegisterViewModel model)
        {
            try
            {
                return await "https://habitica.com/api/v3/user/auth/local/register"
                    .PostJsonAsync(new
                    {
                        username = user.UserName,
                        email = user.Email,
                        password = model.Password,
                        confirmPassword = model.ConfirmPassword,

                    })
                    .ReceiveJson();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

        public static async Task<dynamic> PostCreateTag(HabiticaUser user)
        {
            try
            {
                return await "https://habitica.com/api/v3/tags"
                       .WithHeaders(new
                       {
                           x_api_user = user.Uuid,
                           x_api_key = user.ApiToken
                       })
                       .PostJsonAsync(new
                       {
                           name = "Habitica Abroad"

                       })
                       .ReceiveJson();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }

        }

        public static async Task<string> GetNewUserTask(ApplicationUser user, RegisterViewModel model)
        {

            return await "https://habitica.com/api/v3/tasks/user"
                    .PostUrlEncodedAsync(new
                    {

                    })
                    .ReceiveString();
        }

        public static async Task<dynamic> GetUserData(HabiticaUser model)
        {
            try
            {
                return await "https://habitica.com/api/v3/user"
                   .WithHeaders(new
                   {
                       x_api_key = model.ApiToken,
                       x_api_user = model.Uuid,
                   })
                   .GetJsonAsync();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }

        }

        public static async Task<dynamic> PostNewHabiticaTask(CustomTask task, HabiticaUser user)
        {
            var data = new SendTask
            {
                text = task.TaskName,
                type = task.TaskType,
                tags = new string[] { user.TaskTagId },
                notes = task.TaskNotes,
                date = task.TaskDueDate.ToString(),
                reminders = new Reminder[1]
                {
                    new Reminder() {id = user.Uuid, startDate = task.ReminderStartDate.ToString(), time = task.ReminderTime.ToString() }
                },
                priority = task.TaskDifficulty.ToString()
            };
            try
            {
                return await "https://habitica.com/api/v3/tasks/user"
                    .WithHeaders(new
                    {
                        x_api_user = user.Uuid,
                        x_api_key = user.ApiToken
                    })
                    .PostJsonAsync(data)
                    .ReceiveJson();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

        public static async Task<dynamic> PostNewChecklistItem(CustomTaskItem item, HabiticaUser user, CustomTask task)
        {
            string str1 = "checklist";
            try
            {
                return await "https://habitica.com/api/v3/tasks/"
                   .WithHeaders(new
                   {
                       x_api_user = user.Uuid,
                       x_api_key = user.ApiToken
                   })
                   .AppendPathSegment(task.HabiticaTaskId, true)
                   .AppendPathSegment(str1, true)
                   .PostJsonAsync(new
                   {
                       text = item.ItemName,
                   })
                        .ReceiveJson();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

        public static async Task<dynamic> PostScoreATask(CustomTask task, string direction)
        {
            try
            {
                return await $"https://habitica.com/api/v3/tasks/{task.HabiticaTaskId}/score/{direction}"
                   .GetJsonAsync();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

        public static async Task<dynamic> PostScoreChecklistItem(CustomTask task, CustomTaskItem item)
        {
            try
            {
                return await $"https://habitica.com/api/v3/tasks/{task.HabiticaTaskId}/checklist/{item.HabiticaItemId}/score"
                   .GetJsonAsync();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

        public static async Task<dynamic> DeleteATask(CustomTask task, HabiticaUser user)
        {
            try
            {
                return await $"https://habitica.com/api/v3/tasks/{task.HabiticaTaskId}/tags/{task.TaskTag}"
                    .WithHeaders(new
                    {
                        x_api_key = user.ApiToken,
                        x_api_user = user.Uuid,
                    })
                   .GetJsonAsync();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }

        }

        public static async Task<dynamic> DeleteChecklistItem(CustomTask task, CustomTaskItem item, HabiticaUser user)
        {
            try
            {
                return await $"https://habitica.com/api/v3/tasks/{task.HabiticaTaskId}/checklist/{item.HabiticaItemId}"
                    .WithHeaders(new
                    {
                        x_api_key = user.ApiToken,
                        x_api_user = user.Uuid,
                    })
                   .GetJsonAsync();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

        public static async Task<dynamic> PostClearCompletedToDos(HabiticaUser user)
        {
            try
            {
                return await "https://habitica.com/api/v3/tasks/clearCompletedTodos"
                    .WithHeaders(new
                    {
                        x_api_key = user.ApiToken,
                        x_api_user = user.Uuid,
                    })
                   .GetJsonAsync();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

        public static async Task<dynamic> PutUpdateHabiticaTask(CustomTask task, HabiticaUser user)
        {
            var data = new SendTask
            {
                text = task.TaskName,
                type = task.TaskType,
                tags = new string[] { user.TaskTagId },
                notes = task.TaskNotes,
                date = task.TaskDueDate.ToString(),
                reminders = new Reminder[1]
                {
                    new Reminder() {id = user.Uuid, startDate = task.ReminderStartDate.ToString(), time = task.ReminderTime.ToString() }
                },
                priority = task.TaskDifficulty.ToString()
            };
            try
            {
                return await "https://habitica.com/api/v3/tasks/:taskId"
                    .WithHeaders(new
                    {
                        x_api_user = user.Uuid,
                        x_api_key = user.ApiToken
                    })
                    .PostJsonAsync(data)
                    .ReceiveJson();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

        public static async Task<dynamic> PutUpdateChecklistItem(CustomTaskItem item, HabiticaUser user, CustomTask task)
        {
            try
            {
                return await $"https://habitica.com/api/v3/tasks/{task.HabiticaTaskId}/checklist/{item.HabiticaItemId}"
                   .WithHeaders(new
                   {
                       x_api_user = user.Uuid,
                       x_api_key = user.ApiToken
                   })
                   .PostJsonAsync(new
                   {
                       text = item.ItemName,
                   })
                        .ReceiveJson();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }
    }
}
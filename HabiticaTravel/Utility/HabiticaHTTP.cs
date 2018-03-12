using Flurl.Http;
using HabiticaTravel.Models;
using System.Threading.Tasks;

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

        public static async Task<string> PostCreateTag(HabiticaUser user)
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

        public static async Task<dynamic> PostNewHabiticaTask(CustomTask task , HabiticaUser user)
        {
            try
            {
                return await "https://habitica.com/api/v3/tasks/user"
                   .WithHeaders(new
                   {
                       x_api_user = user.Uuid,
                       x_api_key = user.ApiToken
                   })
                       .PostJsonAsync(new
                       {
                           text = task.TaskName,
                           type = task.TaskType,
                           tags = task.TaskTag,
                           notes = task.TaskNotes,
                           date = task.TaskDueDate,
                           priority = task.TaskDifficulty,
                          

                       });  
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Flurl.Http;
using HabiticaTravel.Models;
using Newtonsoft.Json.Linq;

namespace HabiticaTravel.Utility
{
    public static partial class HabiticaPost
    {
        public static async Task<string> RegisterNewUser(ApplicationUser user, RegisterViewModel model)
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
                    .ReceiveString();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseString();
                throw;
            }
        }
    }


}
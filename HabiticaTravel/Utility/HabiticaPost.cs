using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flurl.Http;
using HabiticaTravel.Models;

namespace HabiticaTravel.Utility
{
    public static partial class HabiticaPost
    {
        public static async void RegisterNewUser(ApplicationUser user, RegisterViewModel model)
        {

            var responseString = await "https://habitica.com/api/v3/user/auth/local/login"
                    .PostUrlEncodedAsync(new
                    {
                        username = user.UserName,
                        email = user.Email,
                        password = model.Password,
                        confirmPassword = model.ConfirmPassword,

                    })
                    .ReceiveString();
        }
    }


}
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
        public static async void ResetPassword(ApplicationUser user, ResetPasswordViewModel model)
        {
            var ResponseString = await "https://habitica.com/api/v3/user/reset-password"
                .PostUrlEncodedAsync(new
                {
                    email = user.Email,
                })
                .ReceiveString();
        }
    }
}
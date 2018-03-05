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
        public static async void UserLogin(ApplicationUser user, RegisterViewModel model)
        {
            var responseString = await "https://habitica.com/api/v3/user/auth/local/register"
                        .PostUrlEncodedAsync(new
                        {
                            username = user.UserName,
                            password = model.Password,

                        })
                        .ReceiveString();

        }
    }
}
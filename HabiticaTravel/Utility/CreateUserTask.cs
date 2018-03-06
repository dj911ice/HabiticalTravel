using Flurl.Http;
using HabiticaTravel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HabiticaTravel.Utility
{
    public static partial class HabiticaRegisterNewUser
    {

        public static async Task<string> NewUserTask(ApplicationUser user, RegisterViewModel model)
        {

            return await "https://habitica.com/api/v3/tasks/user"
                    .PostUrlEncodedAsync(new
                    {
                      
                    })
                    .ReceiveString();
        }
    }
}
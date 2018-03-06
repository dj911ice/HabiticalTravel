using Flurl.Http;
using HabiticaTravel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HabiticaTravel.Utility
{
    public static partial class HabiticaPost
    {
        public static async Task<dynamic> UserLogin(string username, string password)
        {
            try
            {
                return await "https://habitica.com/api/v3/user/auth/local/login"
                        .PostJsonAsync(new
                        {
                            username,
                            password,

                        }).ReceiveJson();
            }
            catch (FlurlHttpException ex)
            {
                return ex.GetResponseJson();
                throw;
            }
        }
    }
}

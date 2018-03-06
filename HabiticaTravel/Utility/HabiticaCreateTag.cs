using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flurl.Http;
using HabiticaTravel.Models;
using System.Threading.Tasks;

namespace HabiticaTravel.Utility
{
    public partial class HabiticaRegisterNewUser
    {
      
        public static async Task<string> CreateTag(HabiticaUser user)
        {
            return await "https://habitica.com/api/v3/user/reset-password"
                        .WithHeaders(new
                        {
                            x_api_user = user.Uuid,
                            x_api_key = user.ApiToken
                        }).GetJsonAsync();
                        
        }
    }
}
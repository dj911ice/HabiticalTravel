using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flurl.Http;
using HabiticaTravel.Models;
using System.Threading.Tasks;

namespace HabiticaTravel.Utility
{
    public partial class HabiticaPost
    {
        public static async Task<string> CreateTag()
        {
            return await "https://habitica.com/api/v3/user/reset-password"
                .PostUrlEncodedAsync(new
                {
                    name = "Travel",
                })
                .ReceiveString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flurl.Http;
using HabiticaTravel.Models;
using System.Threading.Tasks;

namespace HabiticaTravel.Utility
{
    public partial class HabiticaGet
    {

        public static async Task<string> CreateTag(HabiticaUser user) => await "https://habitica.com/api/v3/tags"
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
}
using Flurl.Http;
using HabiticaTravel.Models;
using System.Threading.Tasks;

namespace HabiticaTravel.Utility
{
    public static partial class HabiticaGet
    {

        public static async Task<dynamic> UserData(HabiticaUser model)
        {

            return await "https://habitica.com/api/v3/user"
                    .WithHeaders(new
                    {
                        x_api_key = model.ApiToken,
                        x_api_user = model.Uuid,
                    })
                    .GetJsonAsync();
        }
    }
}
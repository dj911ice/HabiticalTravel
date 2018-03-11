using HabiticaTravel.Models;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace HabiticaTravel.Utility
{
    public static class HabiticaUtil
    {
        public static async Task<JObject> GetUserStats(string id)
        {
            // string CurrentUser = "", CurrentApiToken = "";

            habiticatravelEntities MyHabitica = new habiticatravelEntities();
            HabiticaUser MyHabiticaUser = MyHabitica.HabiticaUsers.Where(y => y.UserId == id).FirstOrDefault();
            return JObject.FromObject(await HabiticaGet.UserData(MyHabiticaUser));

        }

    }
}
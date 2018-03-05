using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabiticaTravel.Models.Habitica.GetUser
{


    public class HabiticaUser
    {
        // possible primary key to tie into wth
        public string Uuid { get; set; }
        public string ApiToken { get; set; }

    }


}
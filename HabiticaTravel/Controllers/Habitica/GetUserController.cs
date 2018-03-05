using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HabiticaTravel.Controllers.Habitica
{
    public class GetUserController : Controller
    {
        // GET: GetUser
        public ActionResult UserInfo()
        {
            // 42.335722   lon=-83.049944 
            string uri = "https://habitica.com/api/v3/user";
            var request = WebRequest.Create(uri);
            if (request != null)
            {
                request.Method = "GET";
                request.Timeout = 20000;
                request.ContentType = "application/json";
                request.Headers.Add("x-api-user", "8eaaae89-5e87-432e-9942-232d2b095f52");
                request.Headers.Add("x-api-key", "1470f2fb-e248-420f-8998-506d02ea7c7f");
                using (Stream response = request.GetResponse().GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(response))
                    {
                        var output = sr.ReadToEnd();
                        JObject JSON = JObject.Parse(output);
                        ViewBag.Data = JSON["data"];
                        return View();
                    }

                }
                // request.GetRequestStream().Close();  
            }
            return View("./Shared/Error");

        }
    }
}
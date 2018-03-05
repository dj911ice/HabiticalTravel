using HabiticaTravel.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HabiticaTravel.Controllers
{
    public partial class HabiticaController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        public ActionResult PostRegisterUser(ApplicationUser user, RegisterViewModel model)
        {
            // 42.335722   lon=-83.049944 
            string uri = "https://habitica.com/api/v3/user";
            var request = WebRequest.Create(uri);
            if (request != null)
            {
                request.Method = "POST";
                request.Timeout = 20000;
                request.ContentType = "application/json";
                string json = $"";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

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
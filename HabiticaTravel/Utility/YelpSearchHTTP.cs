using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HabiticaTravel.Utility
{
    public class YelpSearchHTTP
    {
        public string Url { get; set; } = "https://api.yelp.com".AppendPathSegment("/v3/businesses/search");
        public string APIKey { get; set; } = ConfigurationManager.AppSettings["yelp"];
        public SearchParams Params { get; set; }

        public async Task<dynamic> GetResults(string zipcode, string _categories)
        {
            return await Url.WithOAuthBearerToken(APIKey)
                .SetQueryParams(new SearchParams
                {
                    location = zipcode,
                    radius = 40000,
                    categories = _categories,
                    limit = 1,
                }).GetJsonAsync();
        }

    }

    public class SearchParams
    {
        public string location { get; set; }
        public int radius { get; set; }
        public string categories { get; set; }
        public int limit { get; set; }
    }

    public class YelpCat
    {
        public string Hotel { get; set; }
        public string Arts { get; set; }
        public string Restaurant { get; set; }
        public string Grocery { get; set; }

    }
}
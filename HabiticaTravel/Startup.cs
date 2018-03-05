using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HabiticaTravel.Startup))]
namespace HabiticaTravel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

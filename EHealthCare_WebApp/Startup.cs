using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EHealthCare_WebApp.Startup))]
namespace EHealthCare_WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

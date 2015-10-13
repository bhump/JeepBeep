using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyJeepTrader.Web.Startup))]
namespace MyJeepTrader.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

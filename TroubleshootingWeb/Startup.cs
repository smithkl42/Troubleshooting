using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TroubleshootingWeb.Startup))]
namespace TroubleshootingWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

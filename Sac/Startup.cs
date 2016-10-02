using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sac.Startup))]
namespace Sac
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

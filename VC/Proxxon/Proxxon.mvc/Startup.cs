using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Proxxon.mvc.Startup))]
namespace Proxxon.mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

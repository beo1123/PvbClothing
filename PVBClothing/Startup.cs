using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PVBClothing.Startup))]
namespace PVBClothing
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

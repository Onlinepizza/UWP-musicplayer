using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MusicAppWebPage.Startup))]
namespace MusicAppWebPage
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

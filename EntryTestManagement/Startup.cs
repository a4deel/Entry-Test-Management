using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EntryTestManagement.Startup))]
namespace EntryTestManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

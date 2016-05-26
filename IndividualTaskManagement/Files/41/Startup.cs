using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IndividualTaskManagement.Startup))]
namespace IndividualTaskManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

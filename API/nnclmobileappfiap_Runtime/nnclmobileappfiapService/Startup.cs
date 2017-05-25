using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(nnclmobileappfiapService.Startup))]

namespace nnclmobileappfiapService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}
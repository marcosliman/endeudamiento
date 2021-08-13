using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.WebHooks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Globalization;
[assembly: OwinStartupAttribute(typeof(tesoreria.Startup))]
namespace tesoreria
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-ES");

        }
    }
}

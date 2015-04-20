using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer;
using NewsSite.Business;

namespace NewsSite
{
    public class EPiServerApplication : Global
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            QueueConnector.Initialize();

            //Tip: Want to call the EPiServer API on startup? Add an initialization module instead (Add -> New Item.. -> EPiServer -> Initialization Module)
        }

        protected override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
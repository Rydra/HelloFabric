using System.Web.Http;
using Owin;

namespace WebApi
{
    public static class Startup
    {
        // Este código configura Web API. La clase Startup se especifica como un parámetro de tipo
        // en el método WebApp.Start.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configurar Web API para autohospedaje. 
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}

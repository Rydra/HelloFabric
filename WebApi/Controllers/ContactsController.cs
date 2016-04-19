using System.Collections.Generic;
using System.Web.Http;

namespace WebApi.Controllers
{
    using System;
    using System.Fabric;
    using System.Threading.Tasks;

    using Domain;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using ServiceProxies;

    [RoutePrefix("api")]
    public class ContactsController : ApiController
    {
        public Uri ContactsServiceUri { get; }

        public ContactsController()
        {
            ContactsServiceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/ContactsMS");
        }

        [HttpGet]
        [Route("contactos")]
        public async Task<IEnumerable<Contacto>> GetContactos()
        {
            var proxy = ServiceProxy.Create<IContacts>(ContactsServiceUri);
            return await proxy.ObtenerContactos();
        }
    }
}

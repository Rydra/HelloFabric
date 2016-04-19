using System.Collections.Generic;
using System.Web.Http;

namespace WebApi.Controllers
{
    using System;
    using System.Fabric;
    using System.Threading.Tasks;
    using Domain;

    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using ServiceProxies;

    [RoutePrefix("api")]
    public class ContactsController : ApiController
    {
        public Uri ContactsServiceUri { get; }
        public Uri AltaContactosUri { get; }

        public ContactsController()
        {
            ContactsServiceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/ContactsMS");
            AltaContactosUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/AltaContactoMS");
        }

        [HttpGet]
        [Route("contactos")]
        public async Task<IEnumerable<Contacto>> GetContactos()
        {
            var proxy = ServiceProxy.Create<IContacts>(ContactsServiceUri);
            return await proxy.ObtenerContactos();
        }

        [HttpPost]
        [Route("contactos")]
        public async Task AltaContacto(Contacto contacto)
        {
            var proxy = ServiceProxy.Create<IAltaContacto>(AltaContactosUri, new ServicePartitionKey(1));
            await proxy.DarDeAlta(contacto);
        }

        [HttpPost]
        [Route("contactos/{dni}/pagar")]
        public async Task Pagar([FromUri] string dni, [FromBody] DatosPago pago)
        {
            var proxy = ServiceProxy.Create<IAltaContacto>(AltaContactosUri, new ServicePartitionKey(1));
            await proxy.Pagar(dni, pago);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace ContactsMS
{
    using Domain;
    using ServiceProxies;

    /// <summary>
    /// El runtime de Service Fabric crea una instancia de esta clase para cada instancia del servicio.
    /// </summary>
    internal sealed class ContactsMS : StatelessService, IContacts
    {
        public ContactsMS(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Reemplazo opcional para crear agentes de escucha (por ejemplo, TCP, HTTP) para que esta réplica de servicio controle las solicitudes de cliente o usuario.
        /// </summary>
        /// <returns>Una colección de agentes de escucha.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new List<ServiceInstanceListener>() { new ServiceInstanceListener(this.CreateServiceRemotingListener) };
        }

        ///// <summary>
        ///// Este es el punto de entrada principal para la instancia del servicio.
        ///// </summary>
        ///// <param name="cancellationToken">Se cancela cuando Service Fabric tiene que cerrar esta instancia del servicio.</param>
        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Reemplace el siguiente código de ejemplo por su propia lógica 
        //    //       o quite este reemplazo de RunAsync si no es necesario en su servicio.

        //    long iterations = 0;

        //    while (true)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        ServiceEventSource.Current.ServiceMessage(this, "Working-{0}", ++iterations);

        //        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        //    }
        //}

        public async Task<IEnumerable<Contacto>> ObtenerContactos()
        {
            return new List<Contacto>()
                       {
                           new Contacto() { Nombre = "Pablito", DNI = "12345" },
                           new Contacto() { Nombre = "Chiripitifláutico", DNI = "987654" },
                           new Contacto() { Nombre = "MelónConJamón", DNI = "84874" },
                       };
        }
    }
}

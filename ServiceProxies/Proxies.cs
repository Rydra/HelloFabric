using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProxies
{
    using Domain;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IContacts : IService
    {
        Task<IEnumerable<Contacto>> ObtenerContactos();
    }
    
    public interface IAltaContacto : IService
    {
        Task<bool> DarDeAlta(Contacto contacto);

        Task<bool> Pagar(string DNI, DatosPago pago);
    }
}

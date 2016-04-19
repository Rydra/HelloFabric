using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;


namespace AltaContactoMS
{
    using System.Fabric;
    using System.IO;

    using Domain;

    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Runtime;

    using ServiceProxies;

    /// <summary>
    /// El runtime de Service Fabric crea una instancia de esta clase para cada réplica de servicio.
    /// </summary>
    internal sealed class AltaContactoMS : StatefulService, IAltaContacto
    {
        public AltaContactoMS(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Reemplazo opcional para crear escuchas (por ejemplo, HTTP, comunicación remota del servicio, WCF, etc.) de forma que esta réplica del servicio controle las solicitudes de cliente o de usuario.
        /// </summary>
        /// <remarks>
        /// Para obtener más información sobre la comunicación del servicio, consulte http://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>Una colección de agentes de escucha.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener) };
        }

        public async Task<bool> DarDeAlta(Contacto contacto)
        {
            if (!contacto.IsValid) return false;
            using (var transaction = this.StateManager.CreateTransaction())
            {
                var contactoDict = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Contacto>>(transaction, "contactoDict");
                // Let's try some hacking between sessions!!!! :D
                if (!await contactoDict.ContainsKeyAsync(transaction, "contacto" + contacto.DNI))
                    await contactoDict.AddOrUpdateAsync(transaction, "contacto" + contacto.DNI, contacto, (k, c) => contacto);
                await transaction.CommitAsync();
            }

            return true;
        }

        public async Task<bool> Pagar(string DNI, DatosPago pago)
        {
            if (!pago.IsValid) return false;
            using (var transaction = this.StateManager.CreateTransaction())
            {
                var contactoDict = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Contacto>>(transaction, "contactoDict");
                var contacto = await contactoDict.TryGetValueAsync(transaction, "contacto" + DNI);
                if (!contacto.HasValue) return false;

                // Add the contact to the DB (very stupid line)
                File.WriteAllText("contacto" + DNI, contacto.Value.Nombre);

                await contactoDict.TryRemoveAsync(transaction, "contacto" + DNI);
                await transaction.CommitAsync();
            }

            return true;
        }

        /// <summary>
        /// Este es el punto de entrada principal para la réplica del servicio.
        /// Este método se ejecuta cuando esta réplica del servicio pasa a ser principal y tiene estado de escritura.
        /// </summary>
        /// <param name="cancellationToken">Se cancela cuando Service Fabric tiene que cerrar esta réplica del servicio.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Reemplace el siguiente código de ejemplo por su propia lógica 
            //       o quite este reemplazo de RunAsync si no es necesario en su servicio.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // Si se produce una excepción antes de llamar a CommitAsync, se anula la transacción, se descartan todos los cambios
                    // y no se guarda nada en las réplicas secundarias.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}

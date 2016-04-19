using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AltaContacto.Tests
{
    using System.Fabric.Fakes;

    using AltaContactoMS;

    using Domain;

    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.ServiceFabric.Data;

    using NSubstitute;

    using Ploeh.AutoFixture;

    [TestClass]
    public class AltaContactoTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (ShimsContext.Create())
            {
                var context = new ShimStatefulServiceContext();
                var reliableState = Substitute.For<IReliableStateManagerReplica>();
                var sut = new AltaContactoMS(context, reliableState);
                var fxt = new Fixture();
                var contacto = fxt.Create<Contacto>();
                var pago = fxt.Create<DatosPago>();
                var successful = sut.DarDeAlta(contacto).Result;
                var successful2 = sut.Pagar(contacto.DNI, pago);
            }
        }
    }
}

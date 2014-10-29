using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentechGateway.Provider;

namespace PaymentechGateway.Tests
{
    [TestClass]
    public class PaymentechGatewayFacadeTests
    {

        IPaymentechGatewayFacade GetTarget()
        {
            return new PaymentechGatewayFacade();
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}

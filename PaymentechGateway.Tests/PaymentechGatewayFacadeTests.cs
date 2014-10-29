using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentechGateway.Provider;

namespace PaymentechGateway.Tests
{
    [TestClass]
    public class PaymentechGatewayFacadeTests
    {

        IPaymentechGatewayFacade GetTarget()
        {
            var settings = new PaymentechGatewaySettings();
            settings.Bin = "001";
            settings.Bin = "000001";
            settings.TerminalId = "001";
            settings.Username = ConfigurationManager.AppSettings["username"];
            settings.Password = ConfigurationManager.AppSettings["password"];
            settings.SandboxGatewayUrl = "https://wsvar.paymentech.net/PaymentechGateway";
            settings.SandboxGatewayFailoverUrl = "https://wsvar2.paymentech.net/PaymentechGateway";
            settings.MerchantId = ConfigurationManager.AppSettings["merchantid"];
            settings.RecurringMerchantId = ConfigurationManager.AppSettings["recurringmerchantid"];
            settings.UseSandbox = true;  
            return new PaymentechGatewayFacade();
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}

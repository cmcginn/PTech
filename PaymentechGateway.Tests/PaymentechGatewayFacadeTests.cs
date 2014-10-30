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
            return new PaymentechGatewayFacade(settings);
        }

        NewOrderRequest GetNewOrderRequest()
        {
            var result = new NewOrderRequest();
            result.CustomerRefNum = "49100650";
            result.GatewayOrderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 22);
            result.OrderShipping = 5.00d;
            result.OrderTotal = 100.20d;
            result.OrderTax = 3.50d;
            result.ShippingRequired = true;
            return result;
        }
        CardInfo GetCardInfo()
        {
            var result = new CardInfo
            {
                CardBrand = "VI",
                CardholderName = "Good RGCustomer",
                CardNumber = "4112344112344113",
                CCV = "123",
                ExpirationDate = "102015"

            };
            return result;
        }
        BillingAddressInfo GetBillingAddressInfo()
        {
            BillingAddressInfo result = new BillingAddressInfo
            {

                Address1 = "1 Northeastern Blvd",
                Address2="Apt 2",
                City="Bedford",
                StateProvince="NH",
                Country="US",
                PostalCode = "03109-1234",
                PhoneNumber="888-555-5555"
               
            };
            return result;
        }
        [TestMethod]
        public void CreatePaymentechProfileTest()
        {
            var target = GetTarget();
            var cp = new CustomerPaymentInfo {EmailAddress = "GoodRGCustomer@donotresolve.com"};
            cp.CardInfo = GetCardInfo();
            cp.BillingAddressInfo = GetBillingAddressInfo();
            var actual = target.CreatePaymentechProfile(cp);
            Assert.IsTrue(actual.Success);
            Assert.IsTrue(actual.ProfileId > 0);
        }

        [TestMethod]
        public void ProcessNewOrderPaymentTest()
        {
            var target = GetTarget();
            var newOrder = GetNewOrderRequest();
            var actual = target.ProcessNewOrderPayment(newOrder);
            Assert.IsTrue(actual.Success);
        }
    }
}

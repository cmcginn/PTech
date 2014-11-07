﻿using System;
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

        OrderRequest GetNewOrderRequest()
        {
            var result = new OrderRequest();
            result.CustomerRefNum = "49100650";
            result.GatewayOrderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 22);
            result.OrderShipping = 5.00d;
            result.TransactionTotal = 100.20d;
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

        RecurringCustomerProfile GetRecurringBillingRequest()
        {
            var result = new RecurringCustomerProfile();
            result.BillingAddressInfo = GetBillingAddressInfo();
            result.CardInfo = GetCardInfo();
            result.RecurringAmount = 10.00d;
            result.StartDate = System.DateTime.UtcNow.AddDays(1);
            result.RecurringFrequency = RecurringFrequency.Monthly;
            return result;
        }

        #region Common Test Methods

        ProfileResponse CreatePaymentechProfile()
        {
            var target = GetTarget();
            var cp = new CustomerProfile { EmailAddress = "GoodRGCustomer@donotresolve.com"};
            cp.CardInfo = GetCardInfo();
            cp.BillingAddressInfo = GetBillingAddressInfo();
            var result = target.CreatePaymentechProfile(cp);
            return result;
        }
        #endregion
        [TestMethod]
        public void CreatePaymentechProfileTest()
        {

            var actual = CreatePaymentechProfile();
            Assert.IsTrue(actual.Success);
            Assert.IsTrue(long.Parse(actual.CustomerRefNum) > 0);
        }

        [TestMethod]
        public void ProcessNewOrderPaymentTest()
        {
            var target = GetTarget();
            var newOrder = GetNewOrderRequest();
            var actual = target.ProcessNewOrderPayment(newOrder);
            Assert.IsTrue(actual.Success);
        }
        [TestMethod]
        public void CreatePaymentechRecurringProfileTest()
        {
            var target = GetTarget();
            var request = GetRecurringBillingRequest();
            var actual = target.CreatePaymentechRecurringProfile(request);
            Assert.IsTrue(long.Parse(actual.CustomerRefNum) > 0);
            Assert.IsTrue(actual.Success);
        }

        [TestMethod]
        public void CaptureAuthPaymentTest()
        {
            var target = GetTarget();
            var profile = CreatePaymentechProfile();
            var authOrder = GetNewOrderRequest();
            authOrder.CustomerRefNum = profile.CustomerRefNum;
            authOrder.ShippingRequired = true;
            var authResponse = target.ProcessNewOrderPayment(authOrder);
            var request = new PriorOrderRequest();
            request.CustomerRefNum = authOrder.CustomerRefNum;
            request.TransactionRefNum = authResponse.TransactionRefNum;
            request.TransactionTotal = authOrder.TransactionTotal;
            request.OrderTax = authOrder.OrderTax;
            request.AuthorizationCode = authResponse.AuthorizationCode;
            request.GatewayOrderId = authOrder.GatewayOrderId;
            target.CaptureAuthPayment(request);

        }

        [TestMethod]
        public void RefundTest()
        {
            var target = GetTarget();
            var profile = CreatePaymentechProfile();
            var refundOrder = GetNewOrderRequest();
            refundOrder.CustomerRefNum = profile.CustomerRefNum;
            var refundResponse = target.ProcessNewOrderPayment(refundOrder);
            var request = new PriorOrderRequest();
            request.CustomerRefNum = refundOrder.CustomerRefNum;
            request.TransactionRefNum = refundResponse.TransactionRefNum;
            request.TransactionTotal = refundOrder.TransactionTotal;
            request.OrderTax = refundOrder.OrderTax;
            request.AuthorizationCode = refundResponse.AuthorizationCode;
            request.GatewayOrderId = refundOrder.GatewayOrderId;
            target.Refund(request);
        }

        [TestMethod]
        public void FetchProfileTest()
        {
            var target = GetTarget();
            var profile = CreatePaymentechProfile();
            var actual = target.FetchProfile(profile.CustomerRefNum, false);
            Assert.IsTrue(actual.ProfileAction == ProfileAction.Fetch,"Expected Profile Action Fetch");
            Assert.IsTrue(actual.MerchantId == profile.MerchantId,"Expected MerchantId Match");
            Assert.IsTrue(actual.CustomerRefNum == profile.CustomerRefNum,"Expected Customer Ref Num Match");
        }

        [TestMethod]
        public void UpdateProfileTest()
        {
            var target = GetTarget();
            var profile = CreatePaymentechProfile();
            profile.CardInfo.CardholderName = "CHANGED RG CUST";
            target.UpdateProfile(profile);
            var actual = target.FetchProfile(profile.CustomerRefNum, false);
            Assert.IsTrue(actual.CardInfo.CardholderName == profile.CardInfo.CardholderName,"Expected Cardholder Name Match");

        }

        [TestMethod]
        public void VoidTest()
        {
            var target = GetTarget();
            var newOrder = GetNewOrderRequest();
            var order = target.ProcessNewOrderPayment(newOrder);
            var request = new PriorOrderRequest {TransactionRefNum = order.TransactionRefNum, MerchantId = order.MerchantId, GatewayOrderId=order.GatewayOrderId};
            var actual = target.Void(request);

        }
        #region Conditional Check Tests
        [TestMethod]
        public void ProcessNewOrderPaymentTest_Check_AuthorizationCode_Exists()
        {
            var target = GetTarget();
            var newOrder = GetNewOrderRequest();
            var actual = target.ProcessNewOrderPayment(newOrder);
            Assert.IsTrue(!String.IsNullOrEmpty(actual.AuthorizationCode));
        }
        [TestMethod]
        public void ProcessNewOrderPaymentTest_WhenShippingRequired_CheckPaymentStatus_Authorized()
        {
            var target = GetTarget();
            var newOrder = GetNewOrderRequest();
            newOrder.ShippingRequired = true;
            var actual = target.ProcessNewOrderPayment(newOrder);
            Assert.IsTrue(actual.PaymentStatus == PaymentStatus.Authorized);
        }
        [TestMethod]
        public void ProcessNewOrderPaymentTest_WhenShippingNotRequired_CheckPaymentStatus_AuthorizedForCapture()
        {
            var target = GetTarget();
            var newOrder = GetNewOrderRequest();
            newOrder.ShippingRequired = false;
            var actual = target.ProcessNewOrderPayment(newOrder);
            Assert.IsTrue(actual.PaymentStatus == PaymentStatus.AuthorizedForCapture);
        }
        [TestMethod]
        public void ProcessNewOrderPaymentTest_TransactionRefNum_Exists()
        {
            var target = GetTarget();
            var newOrder = GetNewOrderRequest();
            newOrder.ShippingRequired = false;
            var actual = target.ProcessNewOrderPayment(newOrder);
            Assert.IsFalse(String.IsNullOrEmpty(actual.TransactionRefNum));
        }
        [TestMethod]
        public void ProcessNewOrderPaymentTest_TransactionRequest_TransactionResult_Exists()
        {
            var target = GetTarget();
            var newOrder = GetNewOrderRequest();
            newOrder.ShippingRequired = false;
            var actual = target.ProcessNewOrderPayment(newOrder);
            Assert.IsNotNull(actual.TransactionRequest);
            Assert.IsNotNull(actual.TransactionResponse);
        }
        [TestMethod]
        public void CreatePaymentechProfileTest_When_LifetimeMembership_CheckMaxBilling()
        {
            //validated on payment gateway
            var target = GetTarget();
            var request = GetRecurringBillingRequest();
            request.RecurringFrequency = RecurringFrequency.Lifetime;
            request.RecurringAmount = 100.80d;
            var actual = target.CreatePaymentechRecurringProfile(request);
            Assert.IsTrue(long.Parse(actual.CustomerRefNum) > 0);
            Assert.IsTrue(actual.Success);
        }
        #endregion
    }
}

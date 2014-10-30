using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using PaymentechGateway.Provider.PaymentechServiceReference;

namespace PaymentechGateway.Provider
{
    public class PaymentechGatewayFacade:IPaymentechGatewayFacade
    {
        private readonly PaymentechGatewaySettings _settings;
        public PaymentechGatewayFacade(PaymentechGatewaySettings settings)
        {
            _settings = settings;
        }
        #region class Methods
        public PaymentechGatewayPortTypeClient GetClient()
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            EndpointAddress endpoint = null;
            endpoint = _settings.UseSandbox ? new EndpointAddress(_settings.SandboxGatewayUrl) : new EndpointAddress(_settings.GatewayUrl);
            var result = new PaymentechGatewayPortTypeClient(binding, endpoint);
            return result;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Paymentech amount formats are strings with no decimal (i.e $10.00 = 1000)
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private string GetAmount(double amount)
        {
            return amount.ToString(CultureInfo.InvariantCulture).Replace(".", "");
        }

        private static XElement SerializeNewOrderRequestElement(NewOrderRequestElement request)
        {
            var builder = new StringBuilder();
            var serializer = new XmlSerializer(typeof (NewOrderRequestElement));
            using (var writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, request);
            }
            var result = XElement.Parse(builder.ToString());

            var cc = result.DescendantsAndSelf("ccAccountNum").SingleOrDefault();
            var un = result.DescendantsAndSelf("orbitalConnectionUsername").SingleOrDefault();
            var pw = result.DescendantsAndSelf("orbitalConnectionPassword").SingleOrDefault();
            var ccv = result.DescendantsAndSelf("ccCardVerifyNum").SingleOrDefault();
            if (cc != null)
                cc.Value = cc.Value.Substring(cc.Value.Length - 4);
            if (ccv != null)
                ccv.Value = "xxx";
            if (un != null)
                un.Value = "XXXXX";
            if (pw != null)
                pw.Value = "XXXXX";
            return result;
        }
        private static XElement SerializeNewOrderResponse(NewOrderResponseElement response)
        {
            var builder = new StringBuilder();
            var serializer = new XmlSerializer(typeof(NewOrderResponseElement));
            using (var writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, response);
            }
            var result = XElement.Parse(builder.ToString());
            var cc = result.DescendantsAndSelf("ccAccountNum").SingleOrDefault();
            var un = result.DescendantsAndSelf("orbitalConnectionUsername").SingleOrDefault();
            var pw = result.DescendantsAndSelf("orbitalConnectionPassword").SingleOrDefault();
            var ccv = result.DescendantsAndSelf("ccCardVerifyNum").SingleOrDefault();
            if (cc != null)
                cc.Value = cc.Value.Substring(cc.Value.Length - 4);
            if (ccv != null)
                ccv.Value = "xxx";
            if (un != null)
                un.Value = "XXXXX";
            if (pw != null)
                pw.Value = "XXXXX";
            return result;
        }

        private XElement SerializeProfileAddElement(ProfileAddElement request)
        {
            var builder = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ProfileAddElement));
            using (var writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, request);
            }
            var result = XElement.Parse(builder.ToString());
            var cc = result.DescendantsAndSelf("ccAccountNum").SingleOrDefault();
            var un = result.DescendantsAndSelf("orbitalConnectionUsername").SingleOrDefault();
            var pw = result.DescendantsAndSelf("orbitalConnectionPassword").SingleOrDefault();
            var ccv = result.DescendantsAndSelf("ccCardVerifyNum").SingleOrDefault();
            if (cc != null)
                cc.Value = cc.Value.Substring(cc.Value.Length - 4);
            if (ccv != null)
                ccv.Value = "xxx";
            if (un != null)
                un.Value = "XXXXX";
            if (pw != null)
                pw.Value = "XXXXX";
            return result;
        }

        private XElement SerializeProfileResponse(ProfileResponseElement response)
        {
            var builder = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ProfileResponseElement));
            using (var writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, response);
            }
            var result = XElement.Parse(builder.ToString());
            var cc = result.DescendantsAndSelf("ccAccountNum").SingleOrDefault();
            var un = result.DescendantsAndSelf("orbitalConnectionUsername").SingleOrDefault();
            var pw = result.DescendantsAndSelf("orbitalConnectionPassword").SingleOrDefault();
            var ccv = result.DescendantsAndSelf("ccCardVerifyNum").SingleOrDefault();
            if (cc != null)
                cc.Value = cc.Value.Substring(cc.Value.Length - 4);
            if (ccv != null)
                ccv.Value = "xxx";
            if (un != null)
                un.Value = "XXXXX";
            if (pw != null)
                pw.Value = "XXXXX";
            return result;
        }
        #endregion
        #region IPaymentechGatewayFacade Implementation
        public ProfileResponse CreatePaymentechProfile(CustomerPaymentInfo paymentInfo)
        {
            var result = new ProfileResponse();
            try
            {
                //throw new NotImplementedException();
                var request = MapProfileAddElement(paymentInfo);
                var client = GetClient();
                var response = client.ProfileAdd(request);
                if (response.procStatus == "0" && response.profileAction == "CREATE")
                {
                    result.MerchantId = request.merchantID;
                    result.ProfileAction = ProfileAction.Create;
                    result.ProfileId = long.Parse(response.customerRefNum);
                }
                else
                {
                    result.ErrorMessage = response.procStatusMessage;
                }
            }
            catch (System.Exception ex)
            {
                result.ErrorMessage = ex.GetBaseException().Message;
            }
            return result;

        }

        public NewOrderResponse ProcessNewOrderPayment(NewOrderRequest newOrderRequest)
        {
            var result = new NewOrderResponse();
            try
            {
                var request = MapNewOrderRequestElement(newOrderRequest);
                result.TransactionRequest = SerializeNewOrderRequestElement(request);
                var client = GetClient();
                var response = client.NewOrder(request);
                result.TransactionResponse = SerializeNewOrderResponse(response);
                if (response.procStatus == "0" && response.procStatusMessage.ToLower()=="approved")
                {
                    result.ApprovalCode = response.authorizationCode;
                    result.TransactionRefNum = response.txRefNum;
                    if (response.transType == "AC")
                        result.PaymentStatus = PaymentStatus.AuthorizedForCapture;
                    else if (response.transType == "A")
                        result.PaymentStatus = PaymentStatus.Authorized;
                        

                }
                else
                {
                    result.PaymentStatus = PaymentStatus.Failed;
                    result.ErrorMessage = String.Format("ApprovalStatus:{0} Status Message:{1}", response.approvalStatus,response.procStatusMessage);
                }
                
            }
            catch (System.Exception ex)
            {
                result.ErrorMessage = ex.GetBaseException().Message;
            }
            return result;
        }

       
        #endregion

        #region Mapping
        private NewOrderRequestElement MapNewOrderRequestElement(NewOrderRequest request)
        {
            var result = new NewOrderRequestElement();

            result.amount = GetAmount(request.OrderTotal);
            result.bin = _settings.Bin;
            result.orbitalConnectionUsername = _settings.Username;
            result.orbitalConnectionPassword = _settings.Password;
            result.terminalID = _settings.TerminalId;
            result.merchantID = _settings.MerchantId;

            result.customerRefNum = request.CustomerRefNum;
            result.industryType = "EC";
            result.merchantID = _settings.MerchantId;
            result.orderID = request.GatewayOrderId;
            result.profileOrderOverideInd = "NO";
            result.taxInd = "2";
            if (request.OrderTax > 0)
            {
                result.taxAmount = GetAmount(request.OrderTax);
                result.taxInd = "1";
            }
            result.transType = request.ShippingRequired ? "A" : "AC";
            return result;
        }
        private ProfileAddElement MapProfileAddElement(CustomerPaymentInfo paymentInfo)
        {

            var result = new ProfileAddElement();
            result.version = "2.8";
            result.orbitalConnectionPassword = _settings.Password;
            result.orbitalConnectionUsername = _settings.Username;
            result.merchantID = _settings.MerchantId;
            result.bin = _settings.Bin;
            result.customerProfileFromOrderInd = "NO";
            var eligibleAccountUpdaterBrands = new List<string> { "visa", "mastercard" };
            var brand = paymentInfo.CardInfo.CardBrand;
            result.accountUpdaterEligibility = eligibleAccountUpdaterBrands.Contains(brand) ? "Y" : "N";
            result.customerProfileOrderOverideInd = "OA";
            result.customerProfileFromOrderInd = "A";
            result.customerAccountType = "CC";
            result.ccAccountNum = paymentInfo.CardInfo.CardNumber;
            result.ccExp = paymentInfo.CardInfo.ExpirationDate;
            result.customerEmail = paymentInfo.EmailAddress;
            if (paymentInfo.BillingAddressInfo != null)
            {
                result.customerAddress1 = paymentInfo.BillingAddressInfo.Address1;
                //Chase likes address 2 to be 1 if its present just .... Because
                if (!String.IsNullOrEmpty(paymentInfo.BillingAddressInfo.Address2))
                {
                    result.customerAddress1 = paymentInfo.BillingAddressInfo.Address2;
                    result.customerAddress2 = paymentInfo.BillingAddressInfo.Address1;
                }
                result.customerCity = paymentInfo.BillingAddressInfo.City;
                result.customerCountryCode = paymentInfo.BillingAddressInfo.Country;
                result.customerState = paymentInfo.BillingAddressInfo.StateProvince;
                result.customerPhone = paymentInfo.BillingAddressInfo.PhoneNumber;
                result.customerZIP = paymentInfo.BillingAddressInfo.PostalCode;
                

            }
            return result;

        }
        #endregion
    }
}

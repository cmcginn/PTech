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
            return amount.ToString("0.00").Replace(".", "");
        }
        /// <summary>
        /// Paymentech date formats are strings MMddyyyy
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string GetDate(DateTime source)
        {
            var result = source.ToString("MMddyyyy");
            return result;
        }

        private string GetRecurringSchedule(RecurringBillingRequest request)
        {
            string result = "";
            if (request.RecurringFrequency == RecurringFrequency.None)
                throw new System.ArgumentException("Recurring Frequency must be set to Yearly or Monthly");
            if (request.RecurringFrequency == RecurringFrequency.Monthly)
            {
                //billing start date must be 1 day in the future
                if (request.StartDate.Date == System.DateTime.UtcNow.Date)
                    throw new System.ArgumentException("Billing start date must be 1 day in the future");

                result = String.Format("{0} 1-12/{1} ?", ((int)request.StartDate.Day), 1);

            }
            //lifetime is included, schedule is arbirtary, max billings should be specified in request to 
            //limit 1 billing
            else if (request.RecurringFrequency == RecurringFrequency.Yearly ||
                     request.RecurringFrequency == RecurringFrequency.Lifetime)
            {
                result = String.Format("{0} {1} ?", ((int) request.StartDate.Day), request.StartDate.Month);
                return result;
            }
           
     
            return result;
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

        private static XElement SerializeMarkForCaptureRequest(MarkForCaptureElement request)
        {
            var builder = new StringBuilder();
            var serializer = new XmlSerializer(typeof(MarkForCaptureElement));
            using (var writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, request);
            }
            var result = XElement.Parse(builder.ToString());

            var un = result.DescendantsAndSelf("orbitalConnectionUsername").SingleOrDefault();
            var pw = result.DescendantsAndSelf("orbitalConnectionPassword").SingleOrDefault();
         
            return result;
        }

        private static XElement SerializeMarkForCaptureResponse(MarkForCaptureResponseElement response)
        {
            var builder = new StringBuilder();
            var serializer = new XmlSerializer(typeof(MarkForCaptureResponseElement));
            using (var writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, response);
            }
            var result = XElement.Parse(builder.ToString());
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
                    result.CustomerRefNum = response.customerRefNum;
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
                    result.AuthorizationCode = response.authorizationCode;
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

        public ProfileResponse CreatePaymentechRecurringProfile(RecurringBillingRequest recurringBillingRequest)
        {
            var result = new ProfileResponse();
            try
            {
                var request = MapRecurringProfileAddElement(recurringBillingRequest);
                var client = GetClient();
                var response = client.ProfileAdd(request);
                if (response.procStatus == "0" && response.procStatusMessage == "Profile Request Processed")
                {
                    result.MerchantId = _settings.RecurringMerchantId;
                    result.ProfileAction = ProfileAction.Create;
                    result.CustomerRefNum = response.customerRefNum;

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

        public NewOrderResponse CaptureAuthPayment(CaptureAuthPaymentRequest captureAuthPaymentRequest)
        {
            var result = new NewOrderResponse();
            var request = MapNewOrderCaptureRequest(captureAuthPaymentRequest);
            var client = GetClient();
            var response = client.NewOrder(request);
            if (response.procStatus == "0")
            {
                result.TransactionRefNum = response.txRefNum;
                result.TransactionRequest = SerializeNewOrderRequestElement(request);
                result.TransactionResponse = SerializeNewOrderResponse(response);
                result.MerchantId = request.merchantID;
                result.PaymentStatus = PaymentStatus.Captured;

            }
            else
            {
                result.ErrorMessage = response.procStatusMessage;
            }
            return result;
        }
        public NewOrderResponse MarkForCapture(CaptureAuthPaymentRequest captureAuthPaymentRequest)
        {
            var result = new NewOrderResponse();
            var request = MapCaptureRequest(captureAuthPaymentRequest);

            var client = GetClient();
            var response = client.MarkForCapture(request);
            if (response.procStatus == "0" && response.amount == request.amount)
            {
                result.TransactionRefNum = response.txRefNum;
                result.TransactionRequest = SerializeMarkForCaptureRequest(request);
                result.TransactionResponse = SerializeMarkForCaptureResponse(response);
                result.MerchantId = request.merchantID;
                result.PaymentStatus = PaymentStatus.Captured;

            }
            else
            {
                result.ErrorMessage = response.procStatusMessage;
            }
            return result;
        }
        #endregion
        #region Mapping

        private NewOrderRequestElement MapNewOrderCaptureRequest(CaptureAuthPaymentRequest request)
        {
            var result = new NewOrderRequestElement();
            result.amount = GetAmount(request.OrderTotal);
            result.bin = _settings.Bin;
            result.orbitalConnectionUsername = _settings.Username;
            result.orbitalConnectionPassword = _settings.Password;
            result.terminalID = _settings.TerminalId;
            result.merchantID = _settings.MerchantId;
            result.orderID = request.GatewayOrderId;
            result.customerRefNum = request.CustomerRefNum;
            result.industryType = "EC";

            result.orderID = request.GatewayOrderId;
            result.profileOrderOverideInd = "NO";
            result.taxInd = "2";
            if (request.OrderTax > 0)
            {
                result.taxAmount = GetAmount(request.OrderTax);
                result.taxInd = "1";
            }
            result.txRefNum = request.TransactionRefNum;
            result.priorAuthCd = request.AuthorizationCode;
            result.transType = "FC";
            return result;
        }
        private ProfileAddElement MapRecurringProfileAddElement(RecurringBillingRequest request)
        {
            var result = MapProfileAddElement(request.CustomerPaymentInfo);
            result.merchantID = _settings.RecurringMerchantId;

            var recurringAmount = GetAmount(request.RecurringAmount);
            if (request.StartDate.Date == System.DateTime.UtcNow.Date)
                throw new System.ArgumentException("Billing start date must be 1 day in the future");
            result.mbRecurringStartDate = GetDate(request.StartDate);
            //579 days is limit otherwise, do not specify end date
            if (request.EndDate.HasValue && (request.EndDate.Value - request.StartDate).TotalDays < 579)
                result.mbRecurringEndDate = GetDate(request.EndDate.Value);
            else
                result.mbRecurringNoEndDateFlag = "Y";
            result.mbType = "R";
            result.mbRecurringNoEndDateFlag = "Y";
            result.mbRecurringFrequency = GetRecurringSchedule(request);
            if (request.RecurringFrequency == RecurringFrequency.Lifetime)
            {
                result.mbRecurringMaxBillings = "1";
                result.mbRecurringEndDate = null;
                result.mbRecurringNoEndDateFlag = null;
            }
            result.orderDefaultAmount = GetAmount(request.RecurringAmount);
            result.mbOrderIdGenerationMethod = "DI";
            return result;

        }
        private MarkForCaptureElement MapCaptureRequest(CaptureAuthPaymentRequest request)
        {
            var result = new MarkForCaptureElement();
            result.bin = _settings.Bin;
            result.orbitalConnectionUsername = _settings.Username;
            result.orbitalConnectionPassword = _settings.Password;
            result.terminalID = _settings.TerminalId;
            result.merchantID = _settings.MerchantId;
            result.txRefNum = request.TransactionRefNum;
            result.amount = GetAmount(request.OrderTotal);
            result.taxInd = "2";

            if (request.OrderTax > 0)
            {
                result.taxAmount = GetAmount(request.OrderTax);
                result.taxInd = "1";
            }

            result.orderID = request.GatewayOrderId;
            return result;

        }
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

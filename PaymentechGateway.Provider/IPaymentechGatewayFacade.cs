using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentechGateway.Provider
{
    public interface IPaymentechGatewayFacade
    {
        ProfileResponse CreatePaymentechProfile(CustomerProfile customerProfile);
        NewOrderResponse ProcessNewOrderPayment(NewOrderRequest newOrderRequest);
        ProfileResponse CreatePaymentechRecurringProfile(RecurringCustomerProfile recurringBillingRequest);
        NewOrderResponse MarkForCapture(PriorOrderRequest captureAuthPaymentRequest);
        NewOrderResponse CaptureAuthPayment(PriorOrderRequest captureAuthPaymentRequest);
        NewOrderResponse Refund(PriorOrderRequest refundPaymentRequest);
    }
}

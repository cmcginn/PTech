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
        OrderResponse ProcessNewOrderPayment(OrderRequest newOrderRequest);
        ProfileResponse CreatePaymentechRecurringProfile(RecurringCustomerProfile recurringBillingRequest);
        OrderResponse CaptureAuthPayment(PriorOrderRequest captureAuthPaymentRequest);
        OrderResponse Refund(PriorOrderRequest refundPaymentRequest);
        OrderResponse Void(PriorOrderRequest voidPaymentRequest);
        ProfileResponse FetchProfile(string customerRefNum, bool recurring = false);
        ProfileResponse CancelRecurringProfile(string customerRefNum);
        ProfileResponse UpdateProfile(CustomerProfile customerProfile);
    }
}

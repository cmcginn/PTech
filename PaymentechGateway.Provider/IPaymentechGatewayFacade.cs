using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentechGateway.Provider
{
    public interface IPaymentechGatewayFacade
    {
        ProfileResponse CreatePaymentechProfile(CustomerPaymentInfo paymentInfo);
        NewOrderResponse ProcessNewOrderPayment(NewOrderRequest newOrderRequest);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentechGateway.Provider
{
    public class PaymentechGatewayFacade:IPaymentechGatewayFacade
    {
        private readonly PaymentechGatewaySettings _settings;
        public PaymentechGatewayFacade(PaymentechGatewaySettings settings)
        {
            _settings = settings;
        }
        public ProfileResponse CreatePaymentechProfile()
        {
            throw new NotImplementedException();
        }
    }
}

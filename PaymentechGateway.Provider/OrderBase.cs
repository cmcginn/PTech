using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentechGateway.Provider
{
    public abstract class OrderBase
    {

        public virtual string CustomerRefNum { get; set; }
        public virtual string GatewayOrderId { get; set; }
    }
}

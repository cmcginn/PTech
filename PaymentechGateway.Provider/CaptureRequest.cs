using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentechGateway.Provider
{
    public class CaptureRequest
    {
        public string CustomerRefNum { get; set; }
        public string AuthorizationCode { get; set; }
        public string TransactionRefNum { get; set; }
    }
}

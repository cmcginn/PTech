﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentechGateway.Provider
{
    public class PriorOrderRequest:OrderRequestBase
    {
        public virtual string AuthorizationCode { get; set; }
        public virtual string TransactionRefNum { get; set; }
    }
}

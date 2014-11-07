﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PaymentechGateway.Provider
{
    public abstract class OrderResponseBase:OrderBase
    {
        public bool Success { get { return String.IsNullOrEmpty(ErrorMessage); } }
        public string ErrorMessage { get; set; }
        public string MerchantId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string AuthorizationCode { get; set; }
        public XElement TransactionRequest { get; set; }
        public XElement TransactionResponse { get; set; }
        public string TransactionRefNum { get; set; }
        
    }
}

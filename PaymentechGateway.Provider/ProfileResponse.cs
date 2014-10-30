using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentechGateway.Provider
{
    public enum ProfileAction
    {
        None=0,
        Create=1,
        Update=2,
        Suspend=3,
        Delete=4
    }
    public class ProfileResponse
    {
        public long CustomerRefNum { get; set; }
        public bool Success { get { return String.IsNullOrEmpty(ErrorMessage); } }
        public string ErrorMessage { get; set; }
        public string MerchantId { get; set; }
        public ProfileAction ProfileAction { get; set; }

        
    }
}

using System.Security.Cryptography.X509Certificates;

namespace Textile.Core.Entities.Models.Response.BillingPrint
{
    public class CustomerPrintResponse
    {
        public string Name { get; set; }
        public string PrintName { get; set; }
        
        public string GstIn { get; set; }
        public string Pan { get; set; }

        public string BillingAddress { get; set; }

        public string StateName { get; set; }
        public string StateCode { get; set; }

        public string CityName { get; set; }

    }
}

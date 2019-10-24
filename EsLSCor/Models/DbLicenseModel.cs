using System;

namespace EsLSCor.Models
{
    public class DbLicenseModel
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string LicenseKey { get; set; }
        public decimal Price { get; set; }
    }
}

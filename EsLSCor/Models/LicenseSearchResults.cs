namespace EsLSCor.Models
{
    public class LicenseSearchResults
    {
        public DbLicenseModel[] Licenses { get; set; }
        public int TotalCount { get; set; }
    }
}

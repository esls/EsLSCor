namespace EsLSCor.Models
{
    public class AuthOptions
    {
        public string PwEncKey { get; set; }
        public string PwEncIV { get; set; }
        public string JwtSecret { get; set; }
        public string JwtAudience { get; set; }
        public string JwtIssuer { get; set; }
    }
}

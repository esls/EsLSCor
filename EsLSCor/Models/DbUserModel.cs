using System;

namespace EsLSCor.Models
{
    public class DbUserModel
    {
        public int Id { get; set; }
        public DateTime CreationStamp { get; set; }
        public string Username { get; set; }
        public string PwEnc { get; set; }
        public string Role { get; set; }
    }
}

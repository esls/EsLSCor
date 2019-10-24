namespace EsLSCor.Entities
{
    public enum UserTokenState
    {
        Ok,
        Unprivileged,
        Expired,
        Missing,
        Tampered
    }
}

namespace Benefits.Infrastructure.Identity
{
    public sealed class IdentitySeedingException : Exception
    {
        public IdentitySeedingException(string? message) : base(message)
        {
        }
    }
}

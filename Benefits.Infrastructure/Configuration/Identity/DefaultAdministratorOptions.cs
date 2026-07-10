using System.ComponentModel.DataAnnotations;

namespace Benefits.Infrastructure.Configuration.Identity
{
    public sealed class DefaultAdministratorOptions
    {
        public const string SectionName = "Identity:DefaultAdministrator";

        [Required]
        public string UserName { get; init; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; init; }  = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; init; } = string.Empty;
    }
}

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    // Authentication only
    // Optional: Document as credential:
    //public string? Documento { get; set; }
}
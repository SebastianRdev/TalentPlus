using System.Security.Claims;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class AuthService : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return AuthResult.Failure("Usuario no encontrado.");

        // 1. Verificar la contraseña sin iniciar sesión de inmediato
        var passwordCheck = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordCheck)
            return AuthResult.Failure("Credenciales inválidas.");

        // 2. Si la contraseña es correcta, obtenemos todos los roles del usuario.
        var roles = await _userManager.GetRolesAsync(user);

        // 3. Crear manualmente los Claims, incluyendo el ClaimTypes.Role
        var claims = new List<Claim>
        {
            // Claims básicos (necesarios para que Identity reconozca al usuario)
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
        
            // Puedes agregar más claims si es necesario, como el email:
            new Claim(ClaimTypes.Email, user.Email)
        };

        // 4. Agregar los Claims de Rol
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 5. Crear la identidad y el principal
        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    
        // 6. Finalmente, realiza el Sign In de forma manual. 
        // Esto escribe la cookie con todos los claims, incluyendo los roles.
        await _signInManager.SignInAsync(user, isPersistent: false, authenticationMethod: null);
    
        // Opcional: Forzar el Sign In con el principal recién creado si tienes problemas con el SignOut por defecto:
        // await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);

        return AuthResult.SuccessResult();
    }

    public async Task<AuthResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return AuthResult.SuccessResult();
    }
}
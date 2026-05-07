using First_core_project.DTOs.API;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace First_core_project.Services.API
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // ضفنا الـ RoleManager للتأكد من وجود الأدوار
        private readonly IConfiguration _config;

        public AuthService(UserManager<IdentityUser> userManager,
                           RoleManager<IdentityRole> roleManager,
                           IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        // 1. ميثود تسجيل الدخول (Login)
        public async Task<AuthResponseDto?> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3), // ممكن تخليها من الـ Config
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new AuthResponseDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                    UserName = user.UserName!
                };
            }

            return null;
        }

        // 2. ميثود إنشاء حساب جديد (Register)
        public async Task<bool> RegisterAsync(RegisterDto model)
        {
            // تشيك لو المستخدم موجود أصلاً
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null) return false;

            // تكوين كائن المستخدم الجديد
            var user = new IdentityUser
            {
                UserName = model.Email, // بنستخدم الإيميل كـ UserName زي الـ MVC عندك
                Email = model.Email
            };

            // إنشاء المستخدم وتشفير الباسورد
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // حركة سنيور: التأكد إن رول "User" موجود، لو مش موجود نكريته
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }

                // إعطاء رول "User" للمستخدم الجديد تلقائياً
                await _userManager.AddToRoleAsync(user, "User");

                return true;
            }

            return false;
        }
    }
}
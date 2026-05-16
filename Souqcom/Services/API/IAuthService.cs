using First_core_project.Models;
using First_core_project.DTOs.API;

namespace First_core_project.Services.API
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto model);
        Task<bool> RegisterAsync(RegisterDto model);
    }
}

using First_core_project.DTOs.API;
using First_core_project.Helpers;
using First_core_project.Services.API;
using Microsoft.AspNetCore.Mvc;

namespace First_core_project.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountAPIController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountAPIController(IAuthService authService)
        {
            _authService = authService;
        }

        // 1. تسجيل مستخدم جديد
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // الـ Validation بيتم أوتوماتيك بفضل [ApiController] والـ FluentValidation
            var result = await _authService.RegisterAsync(model);

            if (!result)
            {
                return BadRequest(new ApiResponse<object>(false, "فشل إنشاء الحساب. قد يكون البريد الإلكتروني مستخدماً بالفعل أو كلمة المرور لا تستوفي الشروط."));
            }

            return Ok(new ApiResponse<object>(true, "تم إنشاء الحساب بنجاح! يمكنك الآن تسجيل الدخول."));
        }

        // 2. تسجيل الدخول
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);

            if (result == null)
            {
                return Unauthorized(new ApiResponse<object>(false, "البريد الإلكتروني أو كلمة المرور غير صحيحة"));
            }

            // بنرجع الـ AuthResponseDto (اللي فيه الـ Token والـ Expiration) جوه الـ Data
            return Ok(new ApiResponse<object>(true, "تم تسجيل الدخول بنجاح", result));
        }
    }
}
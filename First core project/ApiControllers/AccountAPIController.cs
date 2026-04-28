using First_core_project.Helpers; // تأكد من استدعاء فولدر الهيلبرز
using First_core_project.Models;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVm model)
        {
            var result = await _authService.LoginAsync(model);

            if (result == null)
            {
                // الرد الموحد في حالة الفشل
                return Unauthorized(new ApiResponse<object>(false, "البريد الإلكتروني أو كلمة المرور غير صحيحة", null!));
            }

            // الرد الموحد في حالة النجاح
            return Ok(new ApiResponse<object>(true, "تم تسجيل الدخول بنجاح", result));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using First_core_project.Services.API;
using First_core_project.Helpers;

namespace First_core_project.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly IApiCartService _cartService;

        public CartAPIController(IApiCartService cartService) => _cartService = cartService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // حماية من الـ Null وتحويل الـ Warning لأخضر
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var baseUrl = $"{Request.Scheme}://{Request.Host}/Images/";

            var cart = await _cartService.GetUserCartAsync(userId, baseUrl);
            return Ok(new ApiResponse<object>(true, "تم جلب السلة", cart));
        }

        [HttpPost("add/{productId}")]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            await _cartService.AddToCartAsync(userId, productId);
            return Ok(new ApiResponse<object>(true, "تمت الإضافة للسلة", null));
        }

        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var result = await _cartService.RemoveFromCartAsync(userId, cartItemId);

            if (!result)
                return NotFound(new ApiResponse<object>(false, "العنصر غير موجود في السلة", null));

            return Ok(new ApiResponse<object>(true, "تم الحذف من السلة", null));
        }
    }
}
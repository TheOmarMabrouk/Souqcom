using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using First_core_project.Services.API;
using First_core_project.Helpers;
using First_core_project.DTOs.API;

namespace First_core_project.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersAPIController : ControllerBase
    {
        private readonly IApiOrderService _orderService;
        private readonly IIPaymentService _paymentService;

        public OrdersAPIController(IApiOrderService orderService, IIPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> Create()
        {
            // 1. استخراج الـ UserId من التوكن
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            // 2. إنشاء الطلب وجلب بيانات الدفع (كله من جوه السيرفس)
            var orderId = await _orderService.CreateOrderAsync(userId);

            if (orderId == null)
                return BadRequest(new ApiResponse<object>(false, "السلة فارغة، لا يمكن إنشاء طلب", null));

            // 3. هنا بقى بننادي سيرفس الدفع (لاحظ إننا بنبعت الـ orderId بس)
            // إحنا هنعدل السيرفس دلوقتي عشان هي اللي تجيب باقي البيانات من الـ DB
            var paymentKey = await _paymentService.GetPaymentKeyForApiAsync(orderId.Value);

            return Ok(new ApiResponse<object>(true, "تم إنشاء الطلب بنجاح", new { orderId, paymentKey }));
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(new ApiResponse<List<OrderResponseDto>>(true, "تم جلب طلباتك بنجاح", orders));
        }
    }
}
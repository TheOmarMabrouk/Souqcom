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
    public class OrdersAPIController : ControllerBase
    {
        private readonly IApiOrderService _orderService;
        private readonly IIPaymentService _paymentService;

        public OrdersAPIController(IApiOrderService orderService, IIPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// إنشاء طلب جديد وجلب توكن الدفع للموبايل
        /// </summary>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("create-order")]
        public async Task<IActionResult> Create()
        {
            try
            {
                // 1. استخراج الـ UserId من التوكن
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

                // 2. إنشاء الطلب في قاعدة البيانات (حالة الطلب الافتراضية Pending)
                var orderId = await _orderService.CreateOrderAsync(userId);

                if (orderId == null)
                    return BadRequest(new ApiResponse<object>(false, "السلة فارغة، لا يمكن إنشاء طلب", null));

                // 3. جلب مفتاح الدفع (Payment Key) من Paymob
                var paymentKey = await _paymentService.GetPaymentKeyForApiAsync(orderId.Value);

                // 4. بناء الرد النهائي للموبايل مع رابط الـ Iframe
                var response = new
                {
                    OrderId = orderId,
                    PaymentKey = paymentKey,
                    // استبدل 123456 بالـ Iframe ID الخاص بك من لوحة تحكم Paymob
                    IframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/123456?payment_token={paymentKey}"
                };

                return Ok(new ApiResponse<object>(true, "تم إنشاء الطلب، برجاء إتمام الدفع", response));
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ وإعادة رد مناسب
                return StatusCode(500, new ApiResponse<object>(false, $"حدث خطأ أثناء معالجة الطلب: {ex.Message}", null));
            }
        }

        /// <summary>
        /// نقطة استقبال تأكيد الدفع من Paymob (Webhook)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("payment-callback")]
        public async Task<IActionResult> PaymentCallback([FromBody] dynamic paymobResponse)
        {
            try
            {
                // Paymob ترسل حالة العملية ومعرف الطلب في الـ Body
                bool isSuccess = paymobResponse.obj.success;
                // merchant_order_id هو الـ OrderId الخاص بنا الذي أرسلناه لـ Paymob
                string merchantOrderId = paymobResponse.obj.order.merchant_order_id;

                if (isSuccess && int.TryParse(merchantOrderId, out int orderId))
                {
                    // تحديث حالة الطلب في قاعدة البيانات إلى "مدفوع" (مثلاً الحالة 1)
                    await _orderService.UpdateOrderStatusAsync(orderId, 1);
                    return Ok();
                }
            }
            catch
            {
                // يمكن إضافة Logging هنا لفحص المشاكل في الـ Callback
            }

            return BadRequest();
        }

        /// <summary>
        /// جلب قائمة طلبات المستخدم الحالي
        /// </summary>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(new ApiResponse<List<OrderResponseDto>>(true, "تم جلب طلباتك بنجاح", orders));
        }
    }
}
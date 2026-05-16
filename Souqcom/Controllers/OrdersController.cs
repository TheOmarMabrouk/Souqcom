using First_core_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    // ضفنا السيرفس دي عشان نتأكد من صحة البيانات (لو موجودة عندك)
    private readonly IPaymentService _paymentService;

    public OrdersController(IOrderService orderService, IPaymentService paymentService)
    {
        _orderService = orderService;
        _paymentService = paymentService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // حماية ضد هجمات الـ CSRF
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _orderService.CheckoutAsync(userId);

        if (!result.Success)
        {
            // بنستخدم TempData عشان نعرض رسالة الخطأ في صفحة السلة
            TempData["Error"] = result.Message;
            return RedirectToAction("Index", "Cart");
        }

        return Redirect(result.PaymentUrl);
    }

    [HttpGet]
    public async Task<IActionResult> PaymentCallback(string success, string merchant_order_id, string hmac)
    {
        // 1. التأكد من وجود البيانات الأساسية
        if (string.IsNullOrEmpty(success) || !int.TryParse(merchant_order_id, out int orderId))
        {
            return BadRequest("بيانات الدفع غير مكتملة.");
        }

        // 2. التعديل الجوهري: التحقق من "صحة" المصدر (Security Check)
        // بنكلم السيرفس تتأكد من Paymob إن العملية دي حقيقية مش تلاعب في اللينك
        bool isValidPayment = await _paymentService.VerifyPaymentAsync(Request.Query);

        if (success == "true" && isValidPayment)
        {
            await _orderService.MarkOrderAsPaidAsync(orderId);
            return RedirectToAction("Success", new { id = orderId });
        }

        // لو الدفع فشل، نرجع لصفحة السلة مع رسالة تنبيه
        TempData["Error"] = "فشلت عملية الدفع، يرجى المحاولة مرة أخرى.";
        return RedirectToAction("Index", "Cart");
    }

    [HttpGet]
    public IActionResult Success(int id)
    {
        // نأمن الصفحة بحيث محدش يكتب ID أي أوردر ويشوفه
        // (يفضل التأكد إن الأوردر ده يخص اليوزر اللي عامل Login حالياً)
        ViewBag.OrderId = id;
        return View();
    }
}
using First_core_project.DTOs;
using First_core_project.Models;
using Microsoft.EntityFrameworkCore;

namespace First_core_project.Services
{
    public class OrderService : IOrderService
    {
        private readonly SouqcomContext _db;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(SouqcomContext db,
            IPaymentService paymentService,
            ILogger<OrderService> logger)
        {
            _db = db;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<CheckoutResultDto> CheckoutAsync(string userId)
        {
            _logger.LogInformation("Checkout started for user {UserId}", userId);

            // 1. نبدأ الـ Transaction (يا كل الخطوات تنجح يا كلها تتمسح)
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // جلب محتويات السلة
                var cartItems = await _db.Carts.Where(c => c.UserId == userId).ToListAsync();
                if (!cartItems.Any())
                {
                    return new CheckoutResultDto { Success = false, Message = "سلة التسوق فارغة." };
                }

                // جلب المنتجات المرتبطة بالسلة للتأكد من الأسعار والمخزن
                var productIds = cartItems.Select(i => (int)i.ProductId).ToList();
                var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

                decimal total = 0;

                // 2. التحقق من المخزن وحساب الإجمالي
                foreach (var item in cartItems)
                {
                    if (!products.TryGetValue((int)item.ProductId, out var product))
                        return new CheckoutResultDto { Success = false, Message = "أحد المنتجات لم يعد متوفراً." };

                    // [نقطة سينيور] التأكد إن الكمية المطلوبة متوفرة في المخزن
                    // if (product.Stock < item.Qty) 
                    //    return new CheckoutResultDto { Success = false, Message = $"عذراً، الكمية المطلوبة من {product.Name} غير متاحة." };

                    total += (decimal)((product.Price ?? 0) * (item.Qty ?? 0));
                }

                // 3. إنشاء الأوردر الرئيسي (Status 0 تعني قيد الانتظار)
                var order = new Order
                {
                    UserId = userId,
                    TotalPrice = total,
                    Status = 0, // Pending
                    CreatAt = DateTime.UtcNow
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync(); // هنا بناخد OrderId الجديد

                // 4. تحويل الـ CartItems إلى OrderItems
                var orderItems = cartItems.Select(item => new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = (int)item.ProductId,
                    ProductName = products[(int)item.ProductId].Name,
                    Price = products[(int)item.ProductId].Price,
                    Quantity = item.Qty,
                    Total = (products[(int)item.ProductId].Price ?? 0) * (item.Qty ?? 0)
                }).ToList();

                _db.OrderItems.AddRange(orderItems);

                // 5. مسح السلة بعد تحويلها لأوردر
                _db.Carts.RemoveRange(cartItems);

                await _db.SaveChangesAsync();

                // 6. طلب رابط الدفع من Paymob
                _logger.LogInformation("Calling Paymob for OrderId {OrderId}", order.Id);
                var paymentUrl = await _paymentService.CreatePaymentAsync(order.Id, total);

                // 7. تثبيت كل العمليات السابقة في قاعدة البيانات
                await transaction.CommitAsync();

                return new CheckoutResultDto
                {
                    Success = true,
                    PaymentUrl = paymentUrl
                };
            }
            catch (Exception ex)
            {
                // في حالة أي خطأ، نلغي كل اللي عملناه (Rollback)
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Transaction failed for User {UserId}. All changes rolled back.", userId);

                return new CheckoutResultDto { Success = false, Message = "حدث خطأ أثناء إتمام الطلب، يرجى المحاولة لاحقاً." };
            }
        }

        public async Task MarkOrderAsPaidAsync(int orderId)
        {
            var order = await _db.Orders.FindAsync(orderId);

            if (order != null && order.Status == 0)
            {
                order.Status = 1; // Paid
                // هنا ممكن تضيف منطق خصم الكمية من المخزن فعلياً بعد التأكد من الدفع
                await _db.SaveChangesAsync();
                _logger.LogInformation("Order {OrderId} updated to Paid status.", orderId);
            }
        }
    }
}
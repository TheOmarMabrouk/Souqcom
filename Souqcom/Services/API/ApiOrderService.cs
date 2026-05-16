using AutoMapper;
using First_core_project.DTOs.API;
using First_core_project.Models;
using First_core_project.Services.API;
using Microsoft.EntityFrameworkCore;

namespace First_core_project.Services.API
{
    public class ApiOrderService : IApiOrderService
    {
        private readonly SouqcomContext _context;
        private readonly IMapper _mapper;

        public ApiOrderService(SouqcomContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems) // مهم جداً عشان تفاصيل الطلب تظهر
                .OrderByDescending(o => o.CreatAt)
                .ToListAsync();

            return _mapper.Map<List<OrderResponseDto>>(orders);
        }

        public async Task<int?> CreateOrderAsync(string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Product)
                    .ToListAsync();

                if (!cartItems.Any()) return null;

                // 1. إنشاء الطلب الأساسي
                var order = new Order
                {
                    UserId = userId,
                    TotalPrice = cartItems.Sum(c => (c.Qty ?? 0) * (c.Product?.Price ?? 0)),
                    Status = 0, // 0 = Pending
                    CreatAt = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // بنحفظ هنا عشان ناخد الـ OrderId

                // 2. تحويل عناصر السلة لعناصر طلب (OrderItems)
                var orderItems = cartItems.Select(c => new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product?.Name,
                    Price = c.Product?.Price,
                    Quantity = c.Qty,
                    Total = (c.Qty ?? 0) * (c.Product?.Price ?? 0)
                }).ToList();

                _context.OrderItems.AddRange(orderItems);

                // 3. مسح السلة بعد نجاح العملية
                _context.Carts.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // الميثود اللي كانت ناقصة وبتحل الأيرور في الكنترولر
        public async Task UpdateOrderStatusAsync(int orderId, int status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}
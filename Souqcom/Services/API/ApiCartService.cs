using AutoMapper;
using First_core_project.DTOs.API;
using First_core_project.Models;
using Microsoft.EntityFrameworkCore;

namespace First_core_project.Services.API
{
    public class ApiCartService : IApiCartService
    {
        private readonly SouqcomContext _context;
        private readonly IMapper _mapper;

        public ApiCartService(SouqcomContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // جلب السلة كاملة
        public async Task<ApiCartDto> GetUserCartAsync(string userId, string baseUrl)
        {
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // تحويل البيانات باستخدام AutoMapper وتمرير الـ BaseUrl للصور
            var itemsDto = _mapper.Map<List<ApiCartItemDto>>(cartItems, opt => opt.Items["BaseUrl"] = baseUrl);

            return new ApiCartDto
            {
                Items = itemsDto,
                TotalItems = itemsDto.Count,
                TotalPrice = itemsDto.Sum(i => (i.Price ?? 0) * i.Quantity)
            };
        }

        // إضافة منتج للسلة (أو زيادة الكمية لو موجود)
        public async Task AddToCartAsync(string userId, int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new Exception("المنتج غير موجود");

            var item = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                item.Qty++; // زيادة الكمية بمقدار 1
            }
            else
            {
                _context.Carts.Add(new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Qty = 1
                });
            }
            await _context.SaveChangesAsync();
        }

        // تحديث الكمية لرقم محدد (تستخدم في الزائد والناقص من الموبايل)
        public async Task<bool> UpdateQuantityAsync(string userId, int productId, int newQuantity)
        {
            // لو الكمية بقت 0 أو أقل، نحذف المنتج من السلة تماماً
            if (newQuantity <= 0)
            {
                var itemToRemove = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (itemToRemove != null)
                {
                    _context.Carts.Remove(itemToRemove);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }

            var item = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (item == null) return false;

            item.Qty = newQuantity;
            await _context.SaveChangesAsync();
            return true;
        }

        // حذف عنصر محدد من السلة
        public async Task<bool> RemoveFromCartAsync(string userId, int cartItemId)
        {
            var item = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (item == null) return false;

            _context.Carts.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
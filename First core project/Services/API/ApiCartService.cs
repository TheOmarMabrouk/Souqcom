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

        public async Task<ApiCartDto> GetUserCartAsync(string userId, string baseUrl)
        {
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // استخدام AutoMapper لتحويل العناصر
            var itemsDto = _mapper.Map<List<ApiCartItemDto>>(cartItems, opt => opt.Items["BaseUrl"] = baseUrl);

            return new ApiCartDto
            {
                Items = itemsDto,
                TotalItems = itemsDto.Count,
                TotalPrice = itemsDto.Sum(i => (i.Price ?? 0) * i.Quantity)
            };
        }

        public async Task AddToCartAsync(string userId, int productId)
        {
            var item = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                item.Qty++;
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

        // إضافة ميثود الحذف عشان السلة تكمل
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
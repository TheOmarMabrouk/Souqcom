using AutoMapper;
using First_core_project.DTOs.API;
using First_core_project.Models;
using First_core_project.Services.API;
using Microsoft.EntityFrameworkCore;

namespace First_core_project.Services.API
{
    public class ApiProductService : IApiProductService
    {
        private readonly SouqcomContext _context;
        private readonly IMapper _mapper;

        public ApiProductService(SouqcomContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(List<ApiProductDto> products, int totalCount)> GetAllProductsAsync(int page, int pageSize, int? categoryId, string? search, string? sort, string baseUrl)
        {
            // بنجيب البيانات ومعاها الصور (Include)
            var query = _context.Products.Include(p => p.ProductImages).AsQueryable();

            if (categoryId.HasValue) query = query.Where(p => p.Catid == categoryId);
            if (!string.IsNullOrEmpty(search)) query = query.Where(p => p.Name!.Contains(search));

            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Id)
            };

            var totalCount = await query.CountAsync();
            var productsData = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // المابينج السحري للـ DTO مع تمرير الـ BaseUrl
            var productsDto = _mapper.Map<List<ApiProductDto>>(productsData, opt => opt.Items["BaseUrl"] = baseUrl);

            return (productsDto, totalCount);
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<bool> UpdateProductAsync(int id, Product updatedProduct)
        {
            // 1. بندور على المنتج الأصلي اللي في الداتابيز
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return false;

            // 2. بنخلي الـ Id بتاع الـ Object اللي جاي هو هو بتاع الداتابيز عشان نضمن مفيش تغيير
            updatedProduct.Id = id;

            // 3. بنحدث القيم (الـ EF هنا ذكي مش هيقرب للـ ID لأنه عارف إنه المفتاح)
            _context.Entry(existing).CurrentValues.SetValues(updatedProduct);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
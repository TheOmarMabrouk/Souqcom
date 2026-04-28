using AutoMapper;
using First_core_project.DTOs.API;
using First_core_project.Models;
using Microsoft.EntityFrameworkCore;

namespace First_core_project.Services.API
{
    public class ApiCategoryService : IApiCategoryService
    {
        private readonly SouqcomContext _context;
        private readonly IMapper _mapper;

        public ApiCategoryService(SouqcomContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ApiCategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            // استخدام AutoMapper للتحويل لـ DTO
            return _mapper.Map<List<ApiCategoryDto>>(categories);
        }

        public async Task<int> CreateCategoryAsync(Category category)
        {
            var exists = await _context.Categories.AnyAsync(c => c.Name == category.Name);
            if (exists) throw new InvalidOperationException("هذا القسم موجود بالفعل");

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task<bool> UpdateCategoryAsync(int id, Category updatedCategory)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            // تحديث احترافي باستخدام SetValues (تجاهل الـ ID أوتوماتيك)
            _context.Entry(existing).CurrentValues.SetValues(updatedCategory);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
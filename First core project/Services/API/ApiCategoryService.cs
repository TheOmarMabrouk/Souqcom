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

        // 1. عرض كل الأقسام
        public async Task<List<ApiCategoryDto>> GetAllCategoriesAsync()
        {
            // AsNoTracking بتخلي الاستعلام سريع جداً لأنه للقراءة فقط
            var categories = await _context.Categories.AsNoTracking().ToListAsync();

            // تحويل القائمة من Entity لـ DTO
            return _mapper.Map<List<ApiCategoryDto>>(categories);
        }

        // 2. إضافة قسم جديد
        public async Task<int> CreateCategoryAsync(CategoryCreateDto dto)
        {
            // شيك لو الاسم موجود قبل أي حاجة (Data Integrity)
            var exists = await _context.Categories.AnyAsync(c => c.Name == dto.Name);
            if (exists) throw new InvalidOperationException("هذا القسم موجود بالفعل");

            // تحويل الـ DTO لـ Entity عشان الداتابيز تفهمه
            var category = _mapper.Map<Category>(dto);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category.Id;
        }

        // 3. تعديل قسم موجود
        public async Task<bool> UpdateCategoryAsync(int id, CategoryCreateDto dto)
        {
            // بندور على القسم الأصلي في الداتابيز
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            // المابينج الذكي: بياخد القيم من الـ DTO ويصبها في الـ Entity اللي الـ Context ماسكه
            _mapper.Map(dto, existing);

            await _context.SaveChangesAsync();
            return true;
        }

        // 4. حذف قسم
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            // حركة سنيور: شيك هل القسم فيه منتجات قبل ما تمسح؟
            var hasProducts = await _context.Products.AnyAsync(p => p.Catid == id);
            if (hasProducts)
                throw new InvalidOperationException("لا يمكن حذف القسم لأنه يحتوي على منتجات مرتبطة به");

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
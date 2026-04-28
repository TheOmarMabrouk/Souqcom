using First_core_project.Models;
using First_core_project.DTOs.API;

namespace First_core_project.Services.API
{
    public interface IApiCategoryService
    {
        Task<List<ApiCategoryDto>> GetAllCategoriesAsync();

        // الميثود اللي كانت ناقصة
        Task<bool> DeleteCategoryAsync(int id);

        // الميثود اللي بتاخد الكائن كامل
        Task<int> CreateCategoryAsync(Category category);

        // لو لسه محتاج الـ Update ضيفها هنا بالمرة
        Task<bool> UpdateCategoryAsync(int id, Category category);
    }
}
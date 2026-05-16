using First_core_project.Models;
using First_core_project.DTOs.API;

namespace First_core_project.Services.API
{
    public interface IApiCategoryService
    {
        Task<List<ApiCategoryDto>> GetAllCategoriesAsync();
        Task<int> CreateCategoryAsync(CategoryCreateDto dto); // تعديل هنا
        Task<bool> UpdateCategoryAsync(int id, CategoryCreateDto dto); // وتعديل هنا
        Task<bool> DeleteCategoryAsync(int id);
    }
}
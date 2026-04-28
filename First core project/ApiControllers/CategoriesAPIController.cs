using AutoMapper;
using First_core_project.DTOs.API;
using First_core_project.Helpers;
using First_core_project.Models;
using First_core_project.Services.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace First_core_project.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesAPIController : ControllerBase
    {
        private readonly IApiCategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesAPIController(IApiCategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(new ApiResponse<object>(true, "تم جلب الأقسام بنجاح", categories));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
        {
            // استخدام المابينج بدل الـ new اليدوي
            var category = _mapper.Map<Category>(categoryDto);

            var categoryId = await _categoryService.CreateCategoryAsync(category);
            return Ok(new ApiResponse<object>(true, "تمت إضافة القسم بنجاح", new { categoryId }));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryCreateDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            var result = await _categoryService.UpdateCategoryAsync(id, category);
            if (!result) return NotFound(new ApiResponse<object>(false, "القسم غير موجود"));

            return Ok(new ApiResponse<object>(true, "تم تحديث القسم بنجاح", new { id }));
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result) return NotFound(new ApiResponse<object>(false, "القسم غير موجود"));

            return Ok(new ApiResponse<object>(true, "تم حذف القسم بنجاح", new { deletedId = id }));
        }
    }
}
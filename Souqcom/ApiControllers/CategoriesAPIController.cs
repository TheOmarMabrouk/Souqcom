using First_core_project.DTOs.API;
using First_core_project.Helpers;
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

        // ملاحظة: شيلنا الـ IMapper لأن السيرفيس هي اللي بتعمل المابينج دلوقتي
        public CategoriesAPIController(IApiCategoryService categoryService)
        {
            _categoryService = categoryService;
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
            // بنبعت الـ DTO للسيرفيس مباشرة.. السيرفيس هي اللي هتعمل المابينج
            try
            {
                var categoryId = await _categoryService.CreateCategoryAsync(categoryDto);
                return Ok(new ApiResponse<object>(true, "تمت إضافة القسم بنجاح", new { categoryId }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>(false, ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryCreateDto categoryDto)
        {
            // بنبعت الـ DTO للسيرفيس مباشرة
            var result = await _categoryService.UpdateCategoryAsync(id, categoryDto);

            if (!result) return NotFound(new ApiResponse<object>(false, "القسم غير موجود"));

            return Ok(new ApiResponse<object>(true, "تم تحديث القسم بنجاح", new { id }));
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result) return NotFound(new ApiResponse<object>(false, "القسم غير موجود"));

                return Ok(new ApiResponse<object>(true, "تم حذف القسم بنجاح", new { deletedId = id }));
            }
            catch (InvalidOperationException ex)
            {
                // دي عشان لو القسم فيه منتجات ورفض يمسحه
                return BadRequest(new ApiResponse<object>(false, ex.Message));
            }
        }
    }
}
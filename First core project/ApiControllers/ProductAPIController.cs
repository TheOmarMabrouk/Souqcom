using First_core_project.DTOs.API;
using First_core_project.Helpers;
using First_core_project.Services.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace First_core_project.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsAPIController : ControllerBase
    {
        private readonly IApiProductService _productService;

        // شيلنا الـ Mapper من هنا لأن السيرفيس هي اللي بقت بتعمل Mapping
        public ProductsAPIController(IApiProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, int? categoryId = null, string? search = null, string? sort = null)
        {
            // بناء الـ BaseUrl للصور
            var baseUrl = $"{Request.Scheme}://{Request.Host}/Images/";

            var (products, totalCount) = await _productService.GetAllProductsAsync(page, pageSize, categoryId, search, sort, baseUrl);

            return Ok(new ApiResponse<object>(true, "Success", new { page, pageSize, totalCount, items = products }));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
        {
            // بنبعت الـ DTO للسيرفيس مباشرة وهي اللي تحوله لـ Entity
            var id = await _productService.CreateProductAsync(dto);

            return Ok(new ApiResponse<object>(true, "Created Successfully", new { id }));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductCreateDto dto)
        {
            // بنبعت الـ DTO للسيرفيس
            var success = await _productService.UpdateProductAsync(id, dto);

            if (!success) return NotFound(new ApiResponse<object>(false, "المنتج غير موجود"));

            return Ok(new ApiResponse<object>(true, "تم التحديث بنجاح"));
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound(new ApiResponse<object>(false, "Not Found"));
            return Ok(new ApiResponse<object>(true, "Deleted Successfully"));
        }
    }
}
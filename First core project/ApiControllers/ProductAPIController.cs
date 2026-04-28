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
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsAPIController : ControllerBase
    {
        private readonly IApiProductService _productService;
        private readonly IMapper _mapper;

        public ProductsAPIController(IApiProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, int? categoryId = null, string? search = null, string? sort = null)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/Images/";
            var (products, totalCount) = await _productService.GetAllProductsAsync(page, pageSize, categoryId, search, sort, baseUrl);
            return Ok(new ApiResponse<object>(true, "Success", new { page, pageSize, totalCount, items = products }));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var id = await _productService.CreateProductAsync(product);
            return Ok(new ApiResponse<object>(true, "Created Successfully", new { id }));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductCreateDto dto)
        {
           
            var product = _mapper.Map<Product>(dto);

            var success = await _productService.UpdateProductAsync(id, product);

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









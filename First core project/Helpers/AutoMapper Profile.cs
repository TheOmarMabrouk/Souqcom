using AutoMapper;
using First_core_project.DTOs.API;
using First_core_project.Models;

namespace First_core_project.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // من Entity لـ DTO (القراءة)
            CreateMap<Product, ApiProductDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Discription))
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items["BaseUrl"] + (src.Photo ?? "default.jpg").Replace("~/", "")))
                .ForMember(dest => dest.Images, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.ProductImages.Select(img => context.Items["BaseUrl"] + (img.Image ?? "default.jpg").Replace("~/", "")).ToList()));

            // استخدام نفس الـ DTO للإنشاء والتحديث
            CreateMap<ProductCreateDto, Product>();
            // ضيف دول جوه الـ Constructor بتاع الـ MappingProfile
            CreateMap<Category, ApiCategoryDto>(); // للقراءة
            CreateMap<CategoryCreateDto, Category>(); // للإنشاء والتحديث


            // ضيف دول جوه الـ Constructor في MappingProfile
            CreateMap<Cart, ApiCartItemDto>()
                .ForMember(dest => dest.CartItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Qty))
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items["BaseUrl"] + (src.Product.Photo ?? "default.jpg").Replace("~/", "")));

            // ضيف دول في MappingProfile.cs
           CreateMap<Order, OrderResponseDto>()
               .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatAt.Value.ToString("yyyy-MM-dd HH:mm")))
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemDto>();
        }
    }
}
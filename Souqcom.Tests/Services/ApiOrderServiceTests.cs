using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using First_core_project.Services.API;
using First_core_project.Models;
using First_core_project.Helpers;
using System;
using System.Threading.Tasks;

namespace First_core_project.Tests.Services // ده اللي هينظم لك الـ Test Explorer
{
    public class ApiOrderServiceTests
    {
        private readonly IMapper _mapper;
        private readonly SouqcomContext _context;
        private readonly ApiOrderService _service;

        public ApiOrderServiceTests()
        {
            // إعداد داتابيز وهمية فريدة لكل اختبار
            var options = new DbContextOptionsBuilder<SouqcomContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SouqcomContext(options);

            // إعداد AutoMapper حقيقي باستخدام البروفايل بتاعك
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();

            _service = new ApiOrderService(_context, _mapper);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldReturnNull_WhenCartIsEmpty()
        {
            var result = await _service.CreateOrderAsync("user_123");
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldReturnOrderId_WhenCartHasItems()
        {
            // Arrange
            var userId = "user_456";
            _context.Products.Add(new Product { Id = 10, Name = "Laptop", Price = 15000 });
            _context.Carts.Add(new Cart { UserId = userId, ProductId = 10, Qty = 1 });
            await _context.SaveChangesAsync();

            // Act
            var orderId = await _service.CreateOrderAsync(userId);

            // Assert
            Assert.NotNull(orderId);
        }
    }
}
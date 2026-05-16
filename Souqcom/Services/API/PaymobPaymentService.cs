using System.Text;
using System.Text.Json;
using First_core_project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace First_core_project.Services.API
{
    public class PaymobPaymentService : IIPaymentService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly SouqcomContext _context;
        private readonly UserManager<IdentityUser> _userManager; // أو ApplicationUser لو عندك كلاس مخصص

        public PaymobPaymentService(IConfiguration config, HttpClient httpClient, SouqcomContext context, UserManager<IdentityUser> userManager)
        {
            _config = config;
            _httpClient = httpClient;
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> GetPaymentKeyForApiAsync(int orderId)
        {
            // 1. جلب الطلب فقط (لأن مفيش Include يوزر)
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) throw new Exception("Order not found");

            // 2. جلب بيانات اليوزر من جدول الـ Users باستخدام الـ UserId اللي جوه الطلب
            var user = await _userManager.FindByIdAsync(order.UserId);

            // 3. استخراج البيانات (مع شوية Default Values عشان لو البيانات ناقصة)
            var total = order.TotalPrice ?? 0;
            var email = user?.Email ?? "customer@mail.com";
            var fName = user?.UserName ?? "Customer"; // غالباً عندك UserName مش FirstName
            var lName = "User";
            var phone = user?.PhoneNumber ?? "01000000000";

            // 4. كمل خطوات Paymob عادي
            var token = await GetAuthTokenAsync();
            var paymobOrderId = await RegisterOrderAsync(token, total, orderId);
            return await GetPaymentKeyAsync(token, paymobOrderId, total, email, fName, lName, phone);
        }

        private async Task<string> GetAuthTokenAsync()
        {
            var requestBody = new { api_key = _config["Paymob:ApiKey"] };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens", content);
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("token").GetString() ?? "";
        }

        private async Task<int> RegisterOrderAsync(string token, decimal amount, int merchantOrderId)
        {
            var requestBody = new
            {
                auth_token = token,
                delivery_needed = false,
                amount_cents = (long)(amount * 100),
                currency = "EGP",
                merchant_order_id = merchantOrderId.ToString(),
                items = Array.Empty<object>()
            };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/orders", content);
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("id").GetInt32();
        }

        private async Task<string> GetPaymentKeyAsync(string token, int paymobOrderId, decimal amount, string email, string fName, string lName, string phone)
        {
            var requestBody = new
            {
                auth_token = token,
                amount_cents = (long)(amount * 100),
                expiration = 3600,
                order_id = paymobOrderId,
                billing_data = new
                {
                    email = email,
                    first_name = fName,
                    last_name = lName,
                    phone_number = phone,
                    apartment = "NA",
                    floor = "NA",
                    street = "NA",
                    building = "NA",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    state = "NA"
                },
                currency = "EGP",
                integration_id = int.Parse(_config["Paymob:IntegrationId"] ?? "0")
            };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", content);
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("token").GetString() ?? "";
        }
    }
}
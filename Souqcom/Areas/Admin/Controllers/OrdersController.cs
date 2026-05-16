using First_core_project.Data;
using First_core_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace First_core_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class OrdersController : Controller
    {
        private readonly SouqcomContext _db;
        private readonly ApplicationDbContext _identityDb;

        public OrdersController(SouqcomContext db, ApplicationDbContext identityDb)
        {
            _db = db;
            _identityDb = identityDb;
        }

      
        public IActionResult Index()
        {
         
            var allOrders = _db.Orders
                .Where(o => o.Status != 4)
                .OrderByDescending(o => o.CreatAt)
                .ToList();

     
            var users = _identityDb.Users.ToDictionary(u => u.Id, u => u.Email);
            ViewBag.Users = users;

            return View(allOrders);
        }


        public IActionResult Details(int id)
        {
            var order = _db.Orders.FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();

        
            var items = _db.OrderItems.Where(i => i.OrderId == id).ToList();
            ViewBag.Items = items;

            
            var user = _identityDb.Users.FirstOrDefault(u => u.Id == order.UserId);
            ViewBag.CustomerEmail = user?.Email;

            return View(order);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, int newStatus)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = $"تم تحديث حالة الطلب #{orderId} بنجاح.";
            }
            return RedirectToAction(nameof(Index));
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId, string reason)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

          
            order.Status = 4;

          

            await _db.SaveChangesAsync();

        
            TempData["SuccessMessage"] = $"تم إلغاء الطلب رقم {orderId} بنجاح واختفى من القائمة الحالية.";

            return RedirectToAction(nameof(Index));
        }


        public IActionResult CancelledOrders()
        {
            var cancelledOrders = _db.Orders
                .Where(o => o.Status == 4)
                .OrderByDescending(o => o.CreatAt)
                .ToList();

            var users = _identityDb.Users.ToDictionary(u => u.Id, u => u.Email);
            ViewBag.Users = users;

            return View("Index", cancelledOrders); 
        }
    }
}
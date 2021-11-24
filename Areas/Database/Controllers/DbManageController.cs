using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Areas.Database.Controllers
{


    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbManageController(AppDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _dbContext.Database.MigrateAsync();
            StatusMessage = "Cập nhật Db thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var success = await _dbContext.Database.EnsureDeletedAsync();
            StatusMessage = success ? "Xóa database thành công" : "Xóa database thất bại";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SeedDataAsync()
        {
            // Tạo các role nếu chưa có
            var roleNames = typeof(RoleName).GetFields().ToList();
            foreach(var r in roleNames)
            {
                var roleName = r.GetRawConstantValue();
                var rFound = await _roleManager.FindByNameAsync((string)roleName);
                if(rFound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole((string)roleName));
                }

            }
            // Tạo ra admin mặc định   
            // admin, admin123, admin@admin.com
            var user = await _userManager.FindByEmailAsync("admin@admin.com");
            if(user == null)
            {
                user = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(user, "admin123");
                await _userManager.AddToRoleAsync(user, RoleName.Administrator);
            }

            StatusMessage = "Đã tạo ra Seed Database";

            return RedirectToAction(nameof(Index));
        }
    }
}

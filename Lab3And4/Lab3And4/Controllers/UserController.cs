using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lab3And4.Models;
using Lab3And4.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;

namespace Lab3And4.Controllers
{
    [Authorize(Roles = "Admin")] //админ: логин babkinvvns@mail.ru, пароль 123456
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationDbContext context, UserManager<AppUser> userManager,
                               IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id != null)
            {
                ViewData["Title"] = "Редактирование пользователя";
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                    return View(user);
                return NotFound();
            }
            else
            {
                ViewData["Title"] = "Регистрация пользователя";
                var user = new AppUser
                {
                    Email = "ex@mail.ru",
                    FirstName = "Вячеслав",
                    Surname = "Бабкин",
                    LastName = "Вячеславович"
                };
                return View(user);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppUser user)
        {
            var changedUser = await _context.User.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (changedUser != null)
            {
                changedUser.Email = user.Email;
                changedUser.FirstName = user.FirstName;
                changedUser.LastName = user.LastName;
                changedUser.Surname = user.Surname;
                changedUser.UserName = user.Email;
                await _userManager.UpdateAsync(changedUser);
            }
            else
            {
                user.UserName = user.Email;
                await _userManager.CreateAsync(user);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var urlEncode = HttpUtility.UrlEncode(code);
                var callbackUrl = $"{Request.Scheme}://{Request.Host.Value}/Identity/Account/ResetPassword?userId={user.Id}&code={urlEncode}";
                await _emailSender.SendEmailAsync(user.Email, $"Создание аккаунта на {Request.Host.Value}", $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>Нажмите здесь</a>");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Remove(string id)
        {
            if (id != null)
            {
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> ToggleAdmin(string id)
        {
            var role = await _roleManager.FindByNameAsync("Admin");
            if (role == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = await _userManager.FindByIdAsync(id);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return RedirectToAction("Index");
        }
    }
}
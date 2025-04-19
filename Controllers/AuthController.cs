using Microsoft.AspNetCore.Mvc;
using CampusNewsSystem.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace CampusNewsSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

		// 登录
		[HttpGet]
		public IActionResult Login() => View();

		[HttpPost]
		public async Task<IActionResult> Login(string email, string password)
		{
			var hash = HashPassword(password);
			var user = _db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);

			if (user == null)
			{
				ModelState.AddModelError("", "邮箱或密码错误");

				return View();
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Email),
				new Claim(ClaimTypes.Role.ToString(), user.Role.ToString())
			};

			var identity = new ClaimsIdentity(claims, "CookieAuth");

			var principal = new ClaimsPrincipal(identity);
			await HttpContext.SignInAsync("CookieAuth", principal);

			return RedirectToAction("List", "News");
		}

		// 登出
		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("CookieAuth");

			return RedirectToAction("Login");
		}

		// 注册
		[HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string email, string password, string role)
        {
			if (!Enum.TryParse<Models.User.Auth>(role, out var userRole))
			{
				ModelState.AddModelError("", "角色无效");
				return View();
			}

			if (_db.Users.Any(u => u.Email == email))
            {
                ModelState.AddModelError("", "用户已存在");

                return View();
            }

            var user = new User
            {
                Email = email,
                PasswordHash = HashPassword(password),
                Role = userRole,
                IsEmailConfirmed = true
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return RedirectToAction("Login");
        }

        // 计算密码的哈希值
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
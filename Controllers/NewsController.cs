using CampusNewsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusNewsSystem.Controllers
{
	[Authorize]
	public class NewsController : Controller
	{
		private readonly AppDbContext _context;

		public NewsController(AppDbContext context)
		{
			_context = context;
		}

		// 创建新闻
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(News news, IFormFile? image)
		{
			if (!ModelState.IsValid) return View(news);

			// 处理图片上传
			if (image != null && image.Length > 0)
			{
				var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
				var filePath = Path.Combine("wwwroot/uploads", fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await image.CopyToAsync(stream);
				}

				news.ImagePath = "/uploads/" + fileName;
			}

			// 获取当前登录用户 ID（假设你已经在认证中设置了 UserId Claim）
			if (User.Identity?.IsAuthenticated == true)
			{
				var userId = int.Parse(User.FindFirst("UserId")!.Value);
				news.UserId = userId;
			}

			news.CreatedAt = DateTime.Now;

			_context.News.Add(news);
			await _context.SaveChangesAsync();

			return RedirectToAction("List");
		}

		public async Task<IActionResult> List()
		{
			var newsList = await _context.News
				.Include(n => n.User)
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();

			return View(newsList);
		}

		// 删除新闻
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var news = await _context.News
				.Include(n => n.User)
				.FirstOrDefaultAsync(n => n.Id == id);

			if (news == null) return NotFound();

			// 检查是否是管理员
			var role = User.FindFirst("Role")?.Value;
			if (role != "Admin")
			{
				return Forbid(); // 拒绝访问
			}

			return View(news); // 显示确认删除页面
		}

		[HttpPost, ActionName("DeleteConfirmed")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var news = await _context.News.FindAsync(id);
			if (news != null)
			{
				_context.News.Remove(news);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(List));
		}
	}
}

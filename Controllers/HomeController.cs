using CampusNewsSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusNewsSystem.Controllers
{
	public class HomeController : Controller
	{
		private readonly AppDbContext _context;

		public HomeController(AppDbContext context)
		{
			_context = context;
		}

		// 所有用户都能访问的新闻列表
		public async Task<IActionResult> Index()
		{
			var newsList = await _context.News
				.Include(n => n.User)
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();

			return View(newsList);
		}
	}
}
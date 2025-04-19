using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusNewsSystem.Models
{
	public class News
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(150)]
		public string Title { get; set; } = string.Empty;

		[Required]
		public string Content { get; set; } = string.Empty;

		public string? ImagePath { get; set; } // 图片路径（可选）

		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// 外键：发布者
		[ForeignKey("User")]
		public int UserId { get; set; }

		public User? User { get; set; }	// 导航属性：发布者
	}
}

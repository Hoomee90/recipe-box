using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{
	public class HomeController : Controller
	{
		private readonly RecipeBoxContext _db;
		private readonly UserManager<ApplicationUser> _userManager;

		public HomeController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
		{
			_userManager = userManager;
			_db = db;
		}

		[HttpGet("/")]
		public async Task<ActionResult> Index()
		{
			
			string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
			if (currentUser != null)
			{
				List<Recipe> userItems = _db.Recipes
										.Where(entry => entry.User.Id == currentUser.Id)
										.ToList();
				ViewBag.items = userItems;
			}
			return View();
		}
	}
}
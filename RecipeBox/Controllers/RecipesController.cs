using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{
	[Authorize]
	public class RecipesController : Controller
	{
		private readonly RecipeBoxContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		
		public RecipesController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
		{
			_userManager = userManager;
			_db = db;
		}
		
		public async Task<ActionResult> Index(string sortOrder)
		{
			ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
			ViewBag.RatingSortParm = sortOrder == "rating" ? "rating_desc" : "rating";
			
			string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
			var userRecipes = from r in _db.Recipes.Where(entry => entry.User.Id == currentUser.Id)
																select r;
			switch (sortOrder)
			{
				case "name_desc":
        userRecipes = userRecipes.OrderByDescending(r => r.Name);
        break;
      case "rating":
        userRecipes = userRecipes.OrderBy(r => r.Rating);
        break;
      case "rating_desc":
        userRecipes = userRecipes.OrderByDescending(r => r.Rating);
        break;
      default:
        userRecipes = userRecipes.OrderBy(r => r.Name);
        break;
			}

			return View(userRecipes.ToList());
		}
		
		public ActionResult Create()
		{
			return View();
		}
		
		[HttpPost]
		public async Task<ActionResult> Create(Recipe recipe)
		{
			if (!ModelState.IsValid)
			{
				return View(recipe);
			}
			else
			{
				string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
				recipe.User = currentUser;
				_db.Recipes.Add(recipe);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
		}
		
		public ActionResult Details(int id)
		{
			Recipe thisRecipe = _db.Recipes
				.Include(recipe => recipe.JoinEntities)
				.ThenInclude(join => join.Tag)
				.FirstOrDefault(recipe => recipe.RecipeId == id);
			return View(thisRecipe);
		}
		
		public ActionResult Edit(int id)
		{
			Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
			return View(thisRecipe);
		}
		
		[HttpPost]
		public ActionResult Edit(Recipe recipe)
		{
			if (!ModelState.IsValid)
			{
				return View(recipe);
			}
			else
			{
				_db.Recipes.Update(recipe);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
		}
		
		public ActionResult AddTag(int id)
		{
			Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
			ViewBag.TagId = new SelectList(_db.Tags, "TagId", "Name");
			return View(thisRecipe);
		}
		
		[HttpPost]
		public ActionResult AddTag(Recipe recipe, int tagId)
		{
			bool joinEntityExists = _db.RecipeTags.Any(join => join.TagId == tagId && join.RecipeId == recipe.RecipeId);
			if (!joinEntityExists && tagId != 0)
			{
				_db.RecipeTags.Add(new RecipeTag() { TagId = tagId, RecipeId = recipe.RecipeId });
				_db.SaveChanges();
			}
			return RedirectToAction("Details", new { id = recipe.RecipeId });
		}
		
		public ActionResult Delete(int id)
		{
			Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
			return View(thisRecipe);
		}
		
		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id)
		{
				Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
				_db.Recipes.Remove(thisRecipe);
				_db.SaveChanges();
				return RedirectToAction("Index");
		}
		
		[HttpPost]
		public ActionResult DeleteJoin(int joinId)
		{
			RecipeTag joinEntry = _db.RecipeTags.FirstOrDefault(entry => entry.RecipeTagId == joinId);
			_db.RecipeTags.Remove(joinEntry);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
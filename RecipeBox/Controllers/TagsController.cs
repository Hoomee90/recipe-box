using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{
	[Authorize]
	public class TagsController : Controller
	{
		private readonly RecipeBoxContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		
		public TagsController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
		{
			_userManager = userManager;
			_db = db;
		}

		public async Task<ActionResult> Index()
		{
			string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
			List<Tag> userTags = _db.Tags
				.Where(entry => entry.User.Id == currentUser.Id)
				.ToList();
			return View(userTags);
		}
		
		public ActionResult Details(int id)
		{
			Tag thisTag = _db.Tags
				.Include(tag => tag.JoinEntities)
				.ThenInclude(join => join.Recipe)
				.FirstOrDefault(tag => tag.TagId == id);
			return View(thisTag);
		}
		
		public ActionResult Create()
		{
			return View();
		}
		
		[HttpPost]
		public async Task<ActionResult> Create(Tag tag)
		{
			if (!ModelState.IsValid)
			{
				return View(tag);
			}
			else
			{
				string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
				tag.User = currentUser;
				_db.Tags.Add(tag);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
		}
		
		public ActionResult AddRecipe(int id)
		{
			Tag thisTag = _db.Tags.FirstOrDefault(tag => tag.TagId == id);
			ViewBag.RecipeId = new SelectList(_db.Recipes, "RecipeId", "Name");
			return View(thisTag);
		}
		
		[HttpPost]
		public ActionResult AddRecipe(Tag tag, int recipeId)
		{
			bool joinEntityExists = _db.RecipeTags.Any(join => join.RecipeId == recipeId && join.TagId == tag.TagId);
			if (!joinEntityExists && recipeId != 0)
			{
				_db.RecipeTags.Add(new RecipeTag() { RecipeId = recipeId, TagId = tag.TagId });
				_db.SaveChanges();
			}
			return RedirectToAction("Details", new { id = tag.TagId});
		}
		
		public ActionResult Edit(int id)
		{
			Tag thisTag = _db.Tags.FirstOrDefault(tag => tag.TagId == id);
			return View(thisTag);
		}
		
		[HttpPost]
		public ActionResult Edit(Tag tag)
		{
			_db.Tags.Update(tag);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}
		
		public ActionResult Delete(int id)
		{
			Tag thisTag = _db.Tags.FirstOrDefault(tags => tags.TagId == id);
			return View(thisTag);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id)
		{
			Tag thisTag = _db.Tags.FirstOrDefault(tags => tags.TagId == id);
			_db.Tags.Remove(thisTag);
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
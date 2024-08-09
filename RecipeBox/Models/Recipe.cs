using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeBox.Models
{
	public class Recipe
	{
		public int RecipeId { get; set; }
		[Required(ErrorMessage = "The recipe requires a name")]
		public string Name { get; set; }
		[Required(ErrorMessage = "The recipe requires ingredients")]
		public string Ingredients { get; set; }
		[Required(ErrorMessage = "the recipe requires instructions")]
		public string Instructions { get; set; }
		[RegularExpression("^([0-9]|(10)|\\s)$", ErrorMessage = "Your rating must be an integer 1 to 10, or 0 to leave unrated")]
		public int Rating { get; set; }
		public List<RecipeTag> JoinEntities { get; }
		public ApplicationUser User { get; set; }
	}
}
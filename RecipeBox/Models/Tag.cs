using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeBox.Models
{
	public class Tag
	{
		public int TagId { get; set; }
		[Required(ErrorMessage = "The tag requires a name")]
		public string Name { get; set; }
		public string Description { get; set; }
		public List<RecipeTag> JoinEntities { get; }
		public ApplicationUser User { get; set; }
	}
}
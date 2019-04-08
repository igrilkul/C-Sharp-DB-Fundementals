using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Categories
{
    public class CreateCategoryInputModel
    {
        [Required]
        [MinLength(3),MaxLength(30)]
        public string CategoryName { get; set; }
    }
}

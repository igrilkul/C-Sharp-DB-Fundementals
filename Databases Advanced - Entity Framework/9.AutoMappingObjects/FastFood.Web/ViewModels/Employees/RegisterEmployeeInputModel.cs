using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Employees
{
    public class RegisterEmployeeInputModel
    {
        [Required]
        [MinLength(3),MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }

        //public int PositionId { get; set; }

        public string PositionName { get; set; }

        public string Address { get; set; }
    }
}

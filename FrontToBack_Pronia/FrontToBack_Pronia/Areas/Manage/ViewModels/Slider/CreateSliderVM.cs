using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.Areas.Manage.ViewModels
{
    public class CreateSliderVM
    {


        [Required(ErrorMessage = "The slider title can't be empty")]
        [MaxLength(25, ErrorMessage = "The length of the slider title can't be more than 25 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The slider offer can't be empty")]
        [MaxLength(25, ErrorMessage = "The length of the slider offer can't be more than 25 characters")]
        public string Offer { get; set; }

        [Required(ErrorMessage = "The slider offer can't be empty")]
        [MaxLength(50, ErrorMessage = "The length of the slider description can't be more than 50 characters")]
        public string Description { get; set; }
        [Required(ErrorMessage = "The button can't be empty")]
        [MaxLength(15,ErrorMessage ="The length of the button context can't be more than 15")]
        public string Button { get; set; }

        [Required(ErrorMessage = "The slider order can't be empty")]
        public int Order { get; set; }

        [Required]
        public IFormFile Photo { get; set; }
    }
}

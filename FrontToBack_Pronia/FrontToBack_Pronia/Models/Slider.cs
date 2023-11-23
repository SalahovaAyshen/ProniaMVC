using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBack_Pronia.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The slider image can't be empty")]
        public string? ImageURL { get; set; }
        [Required(ErrorMessage = "The slider offer can't be empty")]
        [MaxLength(25,ErrorMessage ="The length of the slider offer can't be more than 25 characters")]
        public string Offer { get; set; }
        [Required(ErrorMessage = "The slider title can't be empty")]
        [MaxLength(25, ErrorMessage = "The length of the slider title can't be more than 25 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "The slider offer can't be empty")]
        [MaxLength(50, ErrorMessage = "The length of the slider offer can't be more than 50 characters")]
        public string Description { get; set; }
        public string? Button { get; set; }

        [Required(ErrorMessage = "The slider offer can't be empty")]

        public int Order { get; set; }

        [NotMapped]
       
        public IFormFile? Photo { get; set; }
    }
}

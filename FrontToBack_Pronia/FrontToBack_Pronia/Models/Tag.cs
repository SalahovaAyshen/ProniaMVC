using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.Models
{
	public class Tag
	{
        public  int Id { get; set; }
        [Required(ErrorMessage ="The tag name can't be empty")]
        [MaxLength(25,ErrorMessage ="The length of the tag name can't be more than 25")]
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
    }
}

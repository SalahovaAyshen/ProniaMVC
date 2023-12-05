using Microsoft.AspNetCore.Identity;

namespace FrontToBack_Pronia.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}

using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Utilities;
using FrontToBack_Pronia.Utilities.Enums;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace FrontToBack_Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManger;
        public AccountController(UserManager<AppUser> userManager)
        {
            _userManger = userManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if (!ModelState.IsValid) return View();

            if (!userVM.Name.Check())
            {
                ModelState.AddModelError("Name", "The name can't contain number");
                return View();
            }
            if (!userVM.Surname.Check())
            {
                ModelState.AddModelError("Surname", "The surname can't contain number");
                return View();
            }

            string email = userVM.Email;
            Regex regex = new Regex(@"^(([0-9a-z]|[a-z0-9(\.)?a-z]|[a-z0-9])){1,}(\@)[a-z((\-)?)]{1,}(\.)([a-z]{1,}(\.))?([a-z]{2,3})$");
            if (!regex.IsMatch(email))
            {
                ModelState.AddModelError("Email", "The wrong structure");
                return View();
            }

            AppUser appUser = new AppUser
            {
                Name = userVM.Name.Trim().Capitalize(),
                Surname = userVM.Surname.Trim().Capitalize(),
                Email = email.Trim(),
                UserName = userVM.Username.Trim(),
                
            };
            IdentityResult result = await _userManger.CreateAsync(appUser, userVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError item in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}

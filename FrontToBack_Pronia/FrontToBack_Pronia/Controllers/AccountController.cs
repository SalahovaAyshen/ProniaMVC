using FrontToBack_Pronia.Interfaces;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Utilities;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace FrontToBack_Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManger;
        private readonly SignInManager<AppUser> _signInManager;
        public readonly RoleManager<IdentityRole> _roleManger;
        private readonly IEmailService _emailService;
        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signIn,RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManger = userManager;
            _signInManager = signIn;
            _roleManger = roleManager;
            _emailService = emailService;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if (!ModelState.IsValid) return View();

            if (userVM.Name.Check())
            {
                ModelState.AddModelError("Name", "The name can't contain number");
                return View();
            }
            if (userVM.Surname.Check())
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

            await _userManger.AddToRoleAsync(appUser, UserRole.Member.ToString());
            var token = await _userManger.GenerateEmailConfirmationTokenAsync(appUser);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token,Email = appUser.Email}, Request.Scheme);
            _emailService.SendEmailAsync(appUser.Email,"Email Confirmation", confirmationLink);
            //await _signInManager.SignInAsync(appUser, false);
            return RedirectToAction(nameof(SuccessfullyRegistered), "Account");
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            AppUser user = await _userManger.FindByEmailAsync(email);
            if(user==null) return NotFound();
            var result = await _userManger.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            await _signInManager.SignInAsync(user,false);

            return View();
        }
        public IActionResult SuccessfullyRegistered()
        {
            return View();
        }
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> LogIn(LoginVM loginVM, string returnurl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManger.FindByNameAsync(loginVM.UsernameOrEmail);
            if (user == null)
            {
                user = await _userManger.FindByEmailAsync(loginVM.UsernameOrEmail);
                if(user == null)
                {
                    ModelState.AddModelError(String.Empty, "Username, Email or Password is incorrect");
                    return View();  
                }
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsRemembered, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "You are blocked, please try again later");
                return View();
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(String.Empty, "Please confirm your email");
                return View();
            }
            if(!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username, Email or Password is incorrect");
                return View();
            }
            if(returnurl == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnurl);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> CreateRoles()
        {
           
                foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
                {
                    if (!await _roleManger.RoleExistsAsync(role.ToString()))
                    {
                        await _roleManger.CreateAsync(new IdentityRole
                        {
                            Name = role.ToString(),
                        });
                    }

                }
            
            return RedirectToAction("Index", "Home");
        }
    }
}

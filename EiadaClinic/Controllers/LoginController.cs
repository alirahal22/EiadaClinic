using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EiadaClinic.Data;
using EiadaClinic.Models;
using EiadaClinic.Models.BindingModels;
using EiadaClinic.Models.Singleton;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EiadaClinic.Controllers
{
    public class LoginController : Controller
    {
        private ActiveUser _activeUser;
        private readonly SignInManager<EiadaUser> _signInManager;
        private readonly UserManager<EiadaUser> _userManager;
        

        [BindProperty]
        public LoginBindingModel Input { get; set; }


        public LoginController(SignInManager<EiadaUser> signInManager, UserManager<EiadaUser> userManager, ActiveUser activeUser)
        {

            _activeUser = activeUser;
            _userManager = userManager;
            _signInManager = signInManager;
            
        }




        public IActionResult Index()
        {
            //var user = new EiadaUser
            //{
            //    UserName = "admin",
            //    SecurityStamp = Guid.NewGuid().ToString()
            //};
            //var result = await _userManager.CreateAsync(user, "123qweASD!");
            //await _roleManager.CreateAsync(new IdentityRole("Admin"));
            //await _userManager.AddToRoleAsync(user, "Admin");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login()
        {
            //Invalid Input
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Your input is invalid";
                return View("Index");
            }

            //Sign in Attempt
            var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                
                var user = await _userManager.FindByNameAsync(Input.UserName);
                var roles = await _userManager.GetRolesAsync(user);

                _activeUser.UserName = Input.UserName;
                _activeUser.Id = user.Id;
                _activeUser.Role = roles[0] + "s";
                return Redirect("~/" + roles[0] + "s");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View("Index");
            }
        }
    }
}
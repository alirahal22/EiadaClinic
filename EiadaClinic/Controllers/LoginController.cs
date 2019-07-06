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
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync()
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
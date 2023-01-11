using DemoApplication.Areas.Client.ViewModels.Authentication;
using DemoApplication.Contracts.Email;
using DemoApplication.Contracts.Identity;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;

namespace DemoApplication.Controllers
{
    [Area("client")]
    [Route("auth")]
    public class AuthenticationController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

       
        public AuthenticationController(DataContext dbContext, IUserService userService , IEmailService emailService )
        {
            _dbContext = dbContext;
            _userService = userService;
            _emailService = emailService;
        }

        #region Login and Logout

        [HttpGet("login", Name = "client-auth-login")]
        public async Task<IActionResult> LoginAsync()
        {
            if (_userService.IsAuthenticated)
            {
                return RedirectToRoute("client-account-dashboard");
            }

            return View(new LoginViewModel());
        }

        [HttpPost("login", Name = "client-auth-login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel? model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!await _userService.CheckPasswordAsync(model!.Email, model!.Password))
            {
                ModelState.AddModelError(String.Empty, "Email or password is not correct");
                return View(model);
            }


            if (!_userService.IsEmailConfirmed(model.Email))
            {
                ModelState.AddModelError(String.Empty, "Email is not confirmed");
                return View(model);
            }


            await _userService.SignInAsync(model!.Email, model!.Password);
            return RedirectToRoute("client-home-index");



        }

        [HttpGet("logout", Name = "client-auth-logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _userService.SignOutAsync();

            return RedirectToRoute("client-home-index");
        }

        #endregion

        #region Register

        [HttpGet("register", Name = "client-auth-register")]
        public ViewResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost("register", Name = "client-auth-register")]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _userService.CreateAsync(model);

            return RedirectToRoute("client-auth-login");
        }

        #endregion

        [HttpGet("forget-link", Name = "client-auth-forget-link")]
        public IActionResult ForgetLink()
        {
            return View(new User());
        }

        [HttpPost("forget-link", Name = "client-auth-forget-link")]
        public async Task<IActionResult> ForgetLink(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user == null)
            {
                ModelState.AddModelError(String.Empty, "Email  is not found");
                GetView();
            }

            if (!_userService.IsEmailConfirmed(user.Email))
            {
                ModelState.AddModelError(String.Empty, "Email  is not confirmed");
                GetView();            }

            var contentLink = _userService.GenerateUrl<Guid>("auth", "reset-password", user.Id);

            _userService.SendEmail(user , CustomEmailTitles.Reset , contentLink);

            return Ok("Forget token is sended to email");

            IActionResult GetView()
            {
                var model = new User
                {
                    Email = email,
                };
                return View(model);
            }
        }
        [HttpGet("reset-password/{id}", Name = "client-auth-reset-password")]
        public async Task<IActionResult> ResetPassword(Guid Id)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == Id);

            var model = new ResetPasswordViewModel
            {
                Id = user.Id
            };

            return View(model);
        }

        [HttpPost("reset-password/{id}", Name = "client-auth-reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == model.Id);

            user.Password = BC.HashPassword(model.Password);
            await _dbContext.SaveChangesAsync();



            return RedirectToRoute("client-auth-login");
        }

    }
}

using DemoApplication.Contracts.Email;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("activation")]
    public class ActivationController : Controller
    {
        private readonly DataContext _dataContext;

        private readonly IEmailService _emailService;

        private readonly IUserService _userService;

        public ActivationController(DataContext dataContext, IEmailService emailService, IUserService userService)
        {
            _dataContext = dataContext;
            _emailService = emailService;
            _userService = userService; 
        }

        [HttpGet("activated/{token}", Name = "client-activated")]
        public async Task<IActionResult> Activated([FromRoute] Guid token)
        {
            //var user = _dataContext.Users.FirstOrDefault(x => x.UserActivation.Token == token);

            var tokenInDb = await _dataContext.UserActivations.FirstOrDefaultAsync(t => t.Token == token);

            if (tokenInDb == null) return NotFound();

            var user = _dataContext.Users.FirstOrDefault(u => u.Id == tokenInDb.UserId);

            var now = DateTime.Now;

            if(tokenInDb.TokenExpireDate < DateTime.Now) // 12 1
            {
                _dataContext.Remove(tokenInDb);
                await _dataContext.SaveChangesAsync();

                return RedirectToRoute("client-none-activated", user.Id);
            }
            else
            {
                user.IsEmailConfirmed = true;

                await _dataContext.SaveChangesAsync();

                return RedirectToRoute("client-auth-login");
            }

            return RedirectToRoute("client-auth-register");

        }

        [HttpGet("none-activated/{id}", Name = "client-none-activated")]
        public async Task<IActionResult> NoneActivated([FromRoute] Guid id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            return View(user);
        }

        [HttpGet("is-activated/{id}", Name = "client-is-activated")]
        public async Task<IActionResult> NoneActivatedAsync( Guid id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            var activedToken = _dataContext.UserActivations.FirstOrDefault(u => u.UserId == user.Id);

            if(activedToken.Token != null && activedToken.TokenExpireDate < DateTime.Now)
            {
                return RedirectToRoute("client-activated");
            }

            var token = await CreateUserActivation();

            var contentLink = _userService.GenerateUrl<Guid>("activation", "activated", token);

            _userService.SendEmail(user, CustomEmailTitles.Confirm, contentLink);

            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("client-auth-login");

            async Task<Guid> CreateUserActivation()
            {
                var userActivation = new UserActivation
                {
                    Token = Guid.NewGuid(),
                    TokenExpireDate = DateTime.Now.AddHours(1),
                    User = user
                };

                _dataContext.UserActivations.AddAsync(userActivation);

                return userActivation.Token;
            }

          
        }



    }
}

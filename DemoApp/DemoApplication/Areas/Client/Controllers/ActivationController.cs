using DemoApplication.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("activation")] 
    public class ActivationController : Controller
    {
        private readonly DataContext _dataContext;

        public ActivationController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("activated/{token}", Name = "client-activated")]
        public async Task<IActionResult> Activated([FromRoute]Guid token)
        {
            //var user = _dataContext.Users.FirstOrDefault(x => x.UserActivation.Token == token);

            var tokenInDb = await _dataContext.UserActivations.FirstOrDefaultAsync(t => t.Token == token);

            if (tokenInDb == null) return NotFound();

            var user = _dataContext.Users.FirstOrDefault(u => u.Id == tokenInDb.UserId);

            if (tokenInDb.User is not null && tokenInDb.TokenExpireDate != DateTime.Now)
            {
                user.IsEmailConfirmed = true;

                await _dataContext.SaveChangesAsync();

                return RedirectToRoute("client-auth-login");
            };

            return RedirectToRoute("client-auth-register");

        }
    }
}

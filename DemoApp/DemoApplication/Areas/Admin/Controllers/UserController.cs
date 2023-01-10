using DemoApplication.Areas.Admin.ViewModels.Role;
using DemoApplication.Areas.Admin.ViewModels.User;
using DemoApplication.Areas.Client.ViewModels.Basket;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using BC = BCrypt.Net.BCrypt;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/user")]
    [Authorize(Roles ="admin")]
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("list", Name = "admin-user-list")]
        public async Task<IActionResult> ListAsync(int page = 1)
        {
            var model = await _dataContext.Users.Skip((page-1)*5).Take(5)
                .Select(u => new ListItemViewModel(u.Id, u.Email, u.FirstName, u.LastName, u.CreatedAt, u.UpdatedAt, u.Role != null ? u.Role.Name : null))
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_dataContext.Users.Count() / 5);
            return View(model);
        }

        [HttpGet("add-user", Name = "admin-user-add")]
        public async Task<IActionResult> Add()
        {
            var model = new AddViewModel
            {
                Roles = await _dataContext.Roles.Select(r => new ItemViewModel(r.Id, r.Name)).ToListAsync()
            };
            return View(model);
        }

        [HttpPost("add-user", Name = "admin-user-add")]
        public async Task<IActionResult> Add(AddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return  GetView();
            }

            var user = await CreateUser();

            var basket = await CreateBasketAsync();

            await _dataContext.SaveChangesAsync();

            async Task<Basket> CreateBasketAsync()
            {
                //Create basket process
                var basket = new Basket
                {
                    User = user,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                await _dataContext.Baskets.AddAsync(basket);

                return basket;
            }

            async Task<User> CreateUser()
            {
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = BC.HashPassword(model.Password),
                    Email = model.Email,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    RoleId = model.RoleId

                };

                await _dataContext.Users.AddAsync(user);

                return user;
            }

            IActionResult GetView()
            {
                model.Roles = _dataContext.Roles!.Select(r => new ItemViewModel(r.Id, r.Name)).ToList();
                return View(model);
            }

            return RedirectToRoute("admin-user-list");
        }

        [HttpGet("update-user/{id}", Name = "admin-user-update")]
        public async Task<IActionResult> Update(Guid id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return NotFound();
            }

            var model = new UpdateViewModel
            {
                Id = id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = user.RoleId,
                Roles = await _dataContext.Roles!.Select(r => new ItemViewModel(r.Id, r.Name)).ToListAsync(),
            };

            return View(model);
        }

        [HttpPost("update-user/{id}",Name ="admin-user-update")]
        public async Task<IActionResult> Update (Guid id , UpdateViewModel model)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
               return await GetView();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UpdatedAt = DateTime.Now;
            user.RoleId = model.RoleId;

            await _dataContext.SaveChangesAsync();

            async Task<IActionResult> GetView()
            {
                var model = new UpdateViewModel
                {
                    Id = id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    Roles = _dataContext.Roles!.Select(r => new ItemViewModel(r.Id, r.Name)).ToList(),
                };

                return View(model);
            }

            return RedirectToRoute("admin-user-list");
        }
        [HttpPost("delete-user/{id}",Name ="admin-user-delete")]
        public async Task <IActionResult> Delete(Guid id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if(user is null)
            {
                return NotFound();
            }




            _dataContext.Users.Remove(user);

            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("admin-user-list");

        }
    }
}

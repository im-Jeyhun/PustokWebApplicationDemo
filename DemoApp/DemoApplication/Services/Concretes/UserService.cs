using DemoApplication.Areas.Client.ViewModels.Authentication;
using DemoApplication.Areas.Client.ViewModels.Basket;
using DemoApplication.Contracts.Identity;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Exceptions;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;
using DemoApplication.Contracts.Email;

namespace DemoApplication.Services.Concretes
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private User _currentUser;
        private readonly IEmailService _emailService;

        public UserService(
            DataContext dataContext,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public bool IsAuthenticated
        {
            get => _httpContextAccessor.HttpContext!.User.Identity!.IsAuthenticated;
        }

        public User CurrentUser
        {
            get
            {
                if (_currentUser is not null)
                {
                    return _currentUser;
                }

                var idClaim = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(C => C.Type == CustomClaimNames.ID);
                if (idClaim is null)
                    throw new IdentityCookieException("Identity cookie not found");

                _currentUser = _dataContext.Users.First(u => u.Id == Guid.Parse(idClaim.Value));

                return _currentUser;
            }
        }


        public string GetCurrentUserFullName()
        {
            return $"{CurrentUser.FirstName} {CurrentUser.LastName}";
        }

        public bool IsEmailConfirmed(string email)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.Email == email);

            return user.IsEmailConfirmed == true;
        }

        public async Task<bool> CheckPasswordAsync(string? email, string? password)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            //return await _dataContext.Users.AnyAsync(u => u.Email == email && u.Password == password);
            return user is not null && BC.Verify(password, user.Password);

        }

        public async Task SignInAsync(Guid id, string? role = null)
        {
            var claims = new List<Claim>
            {
                new Claim(CustomClaimNames.ID, id.ToString())
            };

            if (role is not null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
        }

        public async Task SignInAsync(string? email, string? password, string? role = null)
        {
            var user = await _dataContext.Users.FirstAsync(u => u.Email == email);

            if (user is not null && BC.Verify(password, user.Password) && user.IsEmailConfirmed == true)
            {
                await SignInAsync(user.Id, role);
            }

        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task CreateAsync(RegisterViewModel model)
        {
            var user = await CreateUserAsync();
            var basket = await CreateBasketAsync();

            await CreteBasketProductsAsync();
            var token = await CreateUserActivation();
            SendEmail(token , user , CustomEmailTitles.Confirm);

            await _dataContext.SaveChangesAsync();

            async Task<User> CreateUserAsync()
            {
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = BC.HashPassword(model.Password),
                  
                };
                await _dataContext.Users.AddAsync(user);

                return user;
            }

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

            async Task CreteBasketProductsAsync()
            {
                //Add products to basket if cookie exists
                var productCookieValue = _httpContextAccessor.HttpContext!.Request.Cookies["products"];
                if (productCookieValue is not null)
                {
                    var productsCookieViewModel = JsonSerializer.Deserialize<List<ProductCookieViewModel>>(productCookieValue);
                    foreach (var productCookieViewModel in productsCookieViewModel)
                    {
                        var book = await _dataContext.Books.FirstOrDefaultAsync(b => b.Id == productCookieViewModel.Id);
                        var basketProduct = new BasketProduct
                        {
                            Basket = basket,
                            BookId = book!.Id,
                            Quantity = productCookieViewModel.Quantity,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        await _dataContext.BasketProducts.AddAsync(basketProduct);
                    }
                }
            }
        }

        public void SendEmail<T>(T token, User user , string title)
        {

            var link = $"https://localhost:7026/activation/activated/{token}";

            List<string> targetEmail = new List<string>();

            targetEmail.Add(user.Email);


            var message = new Message(targetEmail, title, link);

            _emailService.Send(message);
        }
    }
}

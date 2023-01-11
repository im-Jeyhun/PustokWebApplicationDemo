using DemoApplication.Areas.Client.ViewModels.Authentication;
using DemoApplication.Database.Models;

namespace DemoApplication.Services.Abstracts
{
    public interface IUserService
    {
        public bool IsAuthenticated { get; }
        public User CurrentUser { get; }

        Task<bool> CheckPasswordAsync(string? email, string? password);

        public bool IsEmailConfirmed(string email);
        string GetCurrentUserFullName();
        Task SignInAsync(Guid id, string? role = null);
        Task SignInAsync(string? email, string? password, string? role = null);
        Task CreateAsync(RegisterViewModel model);
        Task SignOutAsync();

        public void SendEmail(User user, string title, string content);
        public string GenerateUrl<T>(string controller, string action, T lastPath);


    }
}

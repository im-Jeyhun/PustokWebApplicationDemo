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

        void SendEmail<T>(T token, User user , string title);

    }
}

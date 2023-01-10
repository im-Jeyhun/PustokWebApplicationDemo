using DemoApplication.Areas.Admin.ViewModels.Role;
using DemoApplication.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace DemoApplication.Areas.Admin.ViewModels.User
{
    public class AddViewModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }

        public int? RoleId { get; set; }
        public List<ItemViewModel>? Roles { get; set; }
    }
}

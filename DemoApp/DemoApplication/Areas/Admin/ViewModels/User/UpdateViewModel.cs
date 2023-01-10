using DemoApplication.Areas.Admin.ViewModels.Role;
using DemoApplication.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace DemoApplication.Areas.Admin.ViewModels.User
{
    public class UpdateViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? RoleId { get; set; }
        public List<ItemViewModel>? Roles { get; set; }
    }
}

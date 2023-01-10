namespace DemoApplication.Areas.Admin.ViewModels.User
{
    public class ListItemViewModel
    {
        public ListItemViewModel(Guid id, string? email, string? firstName, string? lastName, DateTime createdAt, DateTime updatedAt, string role)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Role = role;
        }

        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Role { get; set; }
    }
}

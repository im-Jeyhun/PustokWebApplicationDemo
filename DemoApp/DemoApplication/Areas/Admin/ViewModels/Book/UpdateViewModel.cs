using DemoApplication.Areas.Admin.ViewModels.Book.Add;
using System.ComponentModel.DataAnnotations;

namespace DemoApplication.Areas.Admin.ViewModels.Book
{
    public class UpdateViewModel
    {
        public int? Id { get; set; }

        public string? Title { get; set; }

        public decimal Price { get; set; }

        [Required]
        public List<int> CategoryIds { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public List<IFormFile> Image { get; set; }

        public List<string>? ImageUrl { get; set; }

        public List<AuthorListItemViewModel>? Authors { get; set; }
        public List<CategoryListItemViewModel>? Categories { get; set; }
    }
}

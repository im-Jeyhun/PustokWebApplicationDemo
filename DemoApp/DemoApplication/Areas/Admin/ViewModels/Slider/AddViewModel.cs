using System.ComponentModel.DataAnnotations;

namespace DemoApplication.Areas.Admin.ViewModels.Slider
{
    public class AddViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public IFormFile BgImage { get; set; }

        [Required]
        public string ButtonName { get; set; }

        [Required]
        public string BtnRedirectUrl { get; set; }

        [Required]
        public int Order { get; set; }
    }
}

using DemoApplication.Attriubute;
using System.ComponentModel.DataAnnotations;

namespace DemoApplication.Areas.Admin.ViewModels.Slider
{
    public class UpdateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public string BackgroundImageUrl { get; set; }
        public IFormFile BgImage { get; set; }

        public string ButtonName { get; set; }

        public string BtnRedirectUrl { get; set; }

        public int Order { get; set; }
    }
}

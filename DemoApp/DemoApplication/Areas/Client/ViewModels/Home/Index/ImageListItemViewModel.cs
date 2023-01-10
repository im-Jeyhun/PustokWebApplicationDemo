namespace DemoApplication.Areas.Client.ViewModels.Home.Index
{
    public class ImageListItemViewModel
    {
        public ImageListItemViewModel(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public string ImageUrl { get; set; }
    }
}

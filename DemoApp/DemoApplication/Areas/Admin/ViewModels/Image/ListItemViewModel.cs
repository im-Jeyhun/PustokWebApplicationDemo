namespace DemoApplication.Areas.Admin.ViewModels.Image
{
    public class ListItemViewModel
    {
        public ListItemViewModel( string imageNameInFileSystem)
        {
           
            ImageNameInFileSystem = imageNameInFileSystem;
        }

        public string ImageNameInFileSystem { get; set; }

    }
}

namespace DemoApplication.Areas.Client.ViewModels.Home.Index
{
    public class SliderListItemViewModel
    {
        public SliderListItemViewModel(string title, string content, string bgImageName, string bgImageUrl, string buttonName, string btnRedirectUrl, int order)
        {
            Title = title;
            Content = content;
            BgImageName = bgImageName;
            BgImageUrl = bgImageUrl;
            ButtonName = buttonName;
            BtnRedirectUrl = btnRedirectUrl;
            Order = order;
        }

        public string Title { get; set; }
        public string Content { get; set; }
        public string BgImageName { get; set; }
        public string BgImageUrl { get; set; }
        public string ButtonName { get; set; }
        public string BtnRedirectUrl { get; set; }
        public int Order { get; set; }
    }
}

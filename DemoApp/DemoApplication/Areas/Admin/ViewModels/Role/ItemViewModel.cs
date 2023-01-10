namespace DemoApplication.Areas.Admin.ViewModels.Role
{
    public class ItemViewModel
    {
        public ItemViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}

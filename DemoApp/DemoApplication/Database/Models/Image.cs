using DemoApplication.Database.Models.Common;

namespace DemoApplication.Database.Models
{
    public class Image : BaseEntity<int>
    {

        public string ImageName { get; set; }
        public string ImageNameInFileSystem { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

    }
}

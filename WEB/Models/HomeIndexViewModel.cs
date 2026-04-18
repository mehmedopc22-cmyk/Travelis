namespace WEB.Models
{
    public class HomeIndexViewModel
    {
        public List<PopularStayViewModel> PopularStays { get; set; } = [];
    }

    public class PopularStayViewModel
    {
        private static readonly string[] DefaultImages =
        [
            "https://kapana-art-sky-studios-parking-included.plovdiv-hotels.com/data/Imgs/OriginalPhoto/16471/1647193/1647193227/img-kapana-art-sky-studios-parking-included-plovdiv-6.JPEG",
            "https://morski-briz.com/images/2019/08/01/.jpg",
            "https://cf.bstatic.com/xdata/images/hotel/max1024x768/220164400.jpg?k=1aefd73caad27d52c39fc3b2b816c4ec39f18ecbe7ea761216b38ea3cc4711eb&o="
        ];

        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public static string GetDefaultImageByIndex(int index)
        {
            if (index < 0)
            {
                return DefaultImages[0];
            }

            return DefaultImages[index % DefaultImages.Length];
        }
    }
}

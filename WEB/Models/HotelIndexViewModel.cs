namespace WEB.Models
{
    public class HotelIndexViewModel
    {
        public HotelFilterViewModel Filters { get; set; } = new();
        public List<HotelCardViewModel> Hotels { get; set; } = [];
        public string? AiResponse { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages => TotalCount == 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);
        public int FirstItemNumber => TotalCount == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;
        public int LastItemNumber => Math.Min(CurrentPage * PageSize, TotalCount);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public string DestinationTitle =>
            string.IsNullOrWhiteSpace(Filters.Destination)
                ? "Хотели"
                : $"Хотели в {Filters.Destination}";
    }
}

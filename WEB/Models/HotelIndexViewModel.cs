namespace WEB.Models
{
    public class HotelIndexViewModel
    {
        public HotelFilterViewModel Filters { get; set; } = new();
        public List<HotelCardViewModel> Hotels { get; set; } = [];
        public string? AiResponse { get; set; }
        public int TotalCount => Hotels.Count;

        public string DestinationTitle =>
            string.IsNullOrWhiteSpace(Filters.Destination)
                ? "Хотели"
                : $"Хотели в {Filters.Destination}";
    }
}

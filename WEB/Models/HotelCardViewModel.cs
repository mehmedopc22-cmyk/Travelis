namespace WEB.Models
{
    public class HotelCardViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool Approved { get; set; }
        public int Status { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<string> ImageUrls { get; set; } = [];
        public string PrimaryImageUrl => ImageUrls.FirstOrDefault() ?? string.Empty;

        public string FullLocation =>
            $"{Country}, {City} · {Street}, {PostalCode}";

        public string StatusText =>
            Status switch
            {
                1 => "Активен",
                0 => "Неактивен",
                _ => "Неизвестен"
            };

        public string ApprovalText => Approved ? "Одобрен" : "Неодобрен";

        public List<HotelRoomViewModel> Rooms { get; set; } = new List<HotelRoomViewModel>();
    }
}

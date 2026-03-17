
namespace Domain.Entities
{
    public class HotelPaymentEntity
    {
        public required Guid Id { get; set; }
        public required Guid UserID { get; set; }
        public required Guid RoomID { get; set; }
        public required Guid CurrencyID { get; set; }
        public required Guid Amount { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; } = null;
    }
}

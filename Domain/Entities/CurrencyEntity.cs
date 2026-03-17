
namespace Domain.Entities
{
    public class CurrencyEntity
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
    }
}

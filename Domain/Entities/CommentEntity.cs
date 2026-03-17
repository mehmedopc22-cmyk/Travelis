

namespace Domain.Entities
{
    public class CommentEntity
    {
        public required Guid Id { get; set; }
        public required string Comment { get; set; }
        public required Guid UserID { get; set; }
        public required bool Disabled { get; set; }
        public DateTimeOffset? CreatedAt { get; set; } = null;
        public DateTimeOffset? UpdatedAt { get; set; } = null;
    }
}

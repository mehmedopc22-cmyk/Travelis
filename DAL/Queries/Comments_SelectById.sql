SELECT
    Id,
    Comment,
    UserId,
    Disabled,
    CreatedAt,
    UpdatedAt
FROM Comments
WHERE Id = @Id

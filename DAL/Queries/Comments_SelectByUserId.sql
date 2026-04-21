SELECT
    Id,
    Comment,
    UserId,
    Disabled,
    CreatedAt,
    UpdatedAt
FROM Comments
WHERE UserId = @UserId

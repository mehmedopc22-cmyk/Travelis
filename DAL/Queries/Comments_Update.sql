UPDATE Comments
SET
    Comment = @Comment,
    UserId = @UserId,
    Disabled = @Disabled,
    UpdatedAt = @UpdatedAt
WHERE Id = @Id

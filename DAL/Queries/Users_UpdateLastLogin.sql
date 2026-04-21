UPDATE Users
SET
    LastLoginAt = @LastLoginAt,
    UpdatedAt = @UpdatedAt
WHERE Id = @UserId

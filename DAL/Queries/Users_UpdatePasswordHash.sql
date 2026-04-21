UPDATE Users
SET PasswordHash = @PasswordHash,
    UpdatedAt = @UpdatedAt
WHERE Id = @UserId

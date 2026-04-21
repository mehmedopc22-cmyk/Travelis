UPDATE Users
    SET
        Status = @Status,
        UpdatedAt = @UpdatedAt
    WHERE Id = @UserId

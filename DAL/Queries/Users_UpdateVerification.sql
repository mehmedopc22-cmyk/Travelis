UPDATE Users
    SET
        IsVerified = @IsVerified,
        UpdatedAt = @UpdatedAt
    WHERE Id = @UserId

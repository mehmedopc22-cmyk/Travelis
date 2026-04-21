UPDATE Users
SET RoleId = @RoleId,
    UpdatedAt = @UpdatedAt
WHERE Id = @UserId

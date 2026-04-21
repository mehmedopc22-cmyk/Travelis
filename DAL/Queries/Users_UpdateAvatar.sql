UPDATE Users
SET AvatarID = @ImageId,
    UpdatedAt = @UpdatedAt
WHERE Id = @UserId

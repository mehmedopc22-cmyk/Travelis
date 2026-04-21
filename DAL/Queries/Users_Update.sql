UPDATE Users
SET
    Email = @Email,
    FirstName = @FirstName,
    LastName = @LastName,
    LoyaltyPoints = @LoyaltyPoints,
    AvatarID= @AvatarID,
    PasswordHash = @PasswordHash,
    MFAType = @MFAType,
    TFASecret = @TFASecret,
    IsVerified = @IsVerified,
    Status = @Status,
    RoleId = @RoleId,
    LastLoginAt = @LastLoginAt,
    UpdatedAt = @UpdatedAt
WHERE Id = @Id

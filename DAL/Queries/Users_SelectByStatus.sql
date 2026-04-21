SELECT
        Id,
        Email,
        FirstName,
        LastName,
        LoyaltyPoints,
        AvatarID,
        PasswordHash,
        MFAType,
        TFASecret,
        IsVerified,
        Status,
        RoleId,
        LastLoginAt,
        CreatedAt,
        UpdatedAt
    FROM Users
    WHERE Status = @Status

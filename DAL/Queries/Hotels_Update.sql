UPDATE Hotels
SET
    Name = @Name,
    Country = @Country,
    City = @City,
    Street = @Street,
    PostalCode = @PostalCode,
    PhoneNumber = @PhoneNumber,
    Email = @Email,
    Status = @Status,
    Approved = @Approved,
    UpdatedAt = @UpdatedAt
WHERE Id = @Id

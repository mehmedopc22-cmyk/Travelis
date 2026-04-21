SELECT
    Id,
    Name,
    Country,
    City,
    Street,
    PostalCode,
    PhoneNumber,
    Email,
    Status,
    Approved,
    CreatedAt,
    UpdatedAt
FROM TaxiCompany
WHERE Id = @Id

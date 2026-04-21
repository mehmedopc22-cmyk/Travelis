SELECT
    Id,
    TaxiCompanyId,
    RatingId
FROM TaxiCompanyRatings
WHERE Id = @Id

SELECT
    Id,
    TaxiCompanyId,
    RatingId
FROM TaxiCompanyRatings
WHERE TaxiCompanyId = @TaxiCompanyId

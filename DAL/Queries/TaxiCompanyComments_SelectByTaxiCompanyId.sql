SELECT
    Id,
    TaxiCompanyId,
    CommentId
FROM TaxiCompanyComments
WHERE TaxiCompanyId = @TaxiCompanyId

SELECT
    Id,
    TaxiCompanyId,
    CommentId
FROM TaxiCompanyComments
WHERE Id = @Id

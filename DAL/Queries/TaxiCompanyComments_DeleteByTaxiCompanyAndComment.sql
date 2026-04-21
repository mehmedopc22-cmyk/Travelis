DELETE FROM TaxiCompanyComments
WHERE TaxiCompanyId = @TaxiCompanyId
  AND CommentId = @CommentId

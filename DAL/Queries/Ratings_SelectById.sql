SELECT
    Id,
    Disabled,
    Rating,
    CreatedAt
FROM Ratings
WHERE Id = @Id

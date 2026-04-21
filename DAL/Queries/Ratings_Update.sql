UPDATE Ratings
SET
    Disabled = @Disabled,
    Rating = @Rating
WHERE Id = @Id

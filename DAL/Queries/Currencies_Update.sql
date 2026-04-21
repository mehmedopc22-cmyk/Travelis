UPDATE Currencies
SET
    Name = @Name,
    Code = @Code
WHERE Id = @Id

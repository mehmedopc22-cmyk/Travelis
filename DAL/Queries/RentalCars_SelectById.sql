SELECT
    Id,
    Brand,
    Model,
    Kilometers
FROM RentalCars
WHERE Id = @Id

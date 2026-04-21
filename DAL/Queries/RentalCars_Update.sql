UPDATE RentalCars
SET
    Brand = @Brand,
    Model = @Model,
    Kilometers = @Kilometers
WHERE Id = @Id

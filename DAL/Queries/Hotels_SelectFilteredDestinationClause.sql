AND (
    h.Name LIKE @Destination
    OR h.Country LIKE @Destination
    OR h.City LIKE @Destination
    OR h.Street LIKE @Destination
)

SELECT
    tr.Id,
    tr.TaxiCompanyId,
    c.Name AS TaxiCompanyName,
    c.PhoneNumber AS TaxiCompanyPhoneNumber,
    tr.UserId,
    tr.PickupAddress,
    tr.DestinationAddress,
    tr.Time,
    tr.CreatedAt,
    tr.UpdatedAt,
    tr.Status,
    CONCAT(u.FirstName, ' ', u.LastName) AS UserName
FROM TaxiReservation AS tr
INNER JOIN Users AS u ON tr.UserId = u.Id
INNER JOIN TaxiCompany AS c ON tr.TaxiCompanyId = c.Id;

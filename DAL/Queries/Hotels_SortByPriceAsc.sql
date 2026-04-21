ORDER BY (
    SELECT MIN(hr.Price)
    FROM HotelRooms hr
    WHERE hr.HotelId = h.Id
) ASC, h.Name ASC

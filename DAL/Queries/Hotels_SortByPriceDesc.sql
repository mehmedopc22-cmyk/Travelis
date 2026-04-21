ORDER BY (
    SELECT MIN(hr.Price)
    FROM HotelRooms hr
    WHERE hr.HotelId = h.Id
) DESC, h.Name ASC

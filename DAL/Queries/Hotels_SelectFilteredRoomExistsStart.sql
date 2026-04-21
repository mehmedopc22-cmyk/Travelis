AND EXISTS (
    SELECT 1
    FROM HotelRooms hr
    WHERE hr.HotelId = h.Id

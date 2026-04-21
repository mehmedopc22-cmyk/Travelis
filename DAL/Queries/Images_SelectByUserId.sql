SELECT i.Id, i.Name
FROM [UserImages] ui
INNER JOIN [Images] i ON i.Id = ui.ImageId
WHERE ui.UserId = @UserId
ORDER BY ui.Id DESC

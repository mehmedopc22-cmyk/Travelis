SELECT 
    u.Id,
    u.Email,
    u.PasswordHash,
    u.FirstName,
    u.LastName,
    u.IsVerified,
    u.Status,
    u.RoleId,
    r.Name AS RoleName
FROM Users u
INNER JOIN Roles r ON r.Id = u.RoleId
WHERE u.Email = @Email

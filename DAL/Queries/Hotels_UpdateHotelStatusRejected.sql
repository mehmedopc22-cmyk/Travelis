UPDATE [Hotels]
SET
    [Approved] = 0,
    [Status] = 2,
    [UpdatedAt] = @UpdatedAt
WHERE [Id] = @Id

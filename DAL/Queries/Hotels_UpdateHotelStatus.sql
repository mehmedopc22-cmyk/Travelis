UPDATE [Hotels]
SET
    [Approved] = 1,
    [Status] = 1,
    [UpdatedAt] = @UpdatedAt
WHERE [Id] = @Id

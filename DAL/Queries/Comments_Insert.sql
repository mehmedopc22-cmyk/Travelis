INSERT INTO Comments
(
    Id,
    Comment,
    UserId,
    Disabled,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @Id,
    @Comment,
    @UserId,
    @Disabled,
    @CreatedAt,
    @UpdatedAt
)

SELECT
    Id,
    Prompt,
    Reply,
    PromptedUserId
FROM AIRecommendations
WHERE Id = @Id

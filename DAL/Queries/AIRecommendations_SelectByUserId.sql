SELECT
    Id,
    Prompt,
    Reply,
    PromptedUserId
FROM AIRecommendations
WHERE PromptedUserId = @PromptedUserId

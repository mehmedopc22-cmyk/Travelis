UPDATE AIRecommendations
SET
    Prompt = @Prompt,
    Reply = @Reply,
    PromptedUserId = @PromptedUserId
WHERE Id = @Id

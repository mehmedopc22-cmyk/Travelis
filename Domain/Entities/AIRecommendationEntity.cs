namespace Domain.Entities
{
    public class AIRecommendationEntity
    {
        public Guid Id { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string Reply { get; set; } = string.Empty;
        public Guid PromptedUsedID { get; set; }
    }
}

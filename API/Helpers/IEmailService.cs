namespace API.Helpers
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string temporaryPassword, CancellationToken cancellationToken);
    }
}

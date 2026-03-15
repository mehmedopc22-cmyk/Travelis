using Microsoft.AspNetCore.Identity;

namespace API.Helpers
{
    public class PasswordHasherService
    {
        private readonly PasswordHasher<object> _passwordHasher = new();

        public string Hash(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool Verify(string password, string hash)
        {
            PasswordVerificationResult result =
                _passwordHasher.VerifyHashedPassword(null!, hash, password);

            return result == PasswordVerificationResult.Success
                   || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
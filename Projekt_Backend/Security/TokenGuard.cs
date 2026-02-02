using Microsoft.EntityFrameworkCore;
using Projekt_Backend.Models;

namespace Projekt_Backend.Security
{
  
    public sealed class TokenGuard
    {
        private readonly AcsolasContext _db;

        // Demo/admin szabály
        private const int AdminClientId = 1;
        private const string AdminEmail = "admin@acsolas.hu";

        public TokenGuard(AcsolasContext db)
        {
            _db = db;
        }

        
        // Tokenből visszaadja a bejelentkezett ClientId-t, ha érvényes.
        
        public async Task<int?> GetClientIdAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            if (token.Length > 64) return null;

            var session = await _db.AuthoritySessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Token == token && !s.IsRevoked);

            return session?.ClientId;
        }

        
        // Token admin-e? (ClientId==1 vagy admin email)
        
        public async Task<bool> IsAdminAsync(string token)
        {
            var clientId = await GetClientIdAsync(token);
            if (clientId is null) return false;

            if (clientId.Value == AdminClientId) return true;

            var email = await _db.Clients
                .AsNoTracking()
                .Where(c => c.ClientId == clientId.Value)
                .Select(c => c.Email)
                .FirstOrDefaultAsync();

            return string.Equals(email, AdminEmail, StringComparison.OrdinalIgnoreCase);
        }
    }
}


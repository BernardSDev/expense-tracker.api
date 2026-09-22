using System.Security.Cryptography;
using System.Text;

namespace expense_tracker.api.Security;

public class RefreshTokenHasher : IRefreshTokenHasher
{
    public string Hash(string token)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        
        return Convert.ToBase64String(hashBytes);
    }
}
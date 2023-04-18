using System.Security.Cryptography;

namespace rbkApiModules.Commons.Core;

public static class PasswordHasher
{
    /// <summary>
    /// Método para gerar um hash a partir de uma senha
    /// </summary>
    /// <param name="password">Senha do usuário</param>
    public static string GenerateSaltedHash(string password)
    {
        var saltBytes = new byte[64];
        var provider = new RNGCryptoServiceProvider();
        provider.GetNonZeroBytes(saltBytes);
        string salt = Convert.ToBase64String(saltBytes);

        var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA512);
        string hash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

        return salt + hash;
    }

    /// <summary>
    /// Método para conferir se uma senha em hash confere com uma senha digitada
    /// </summary>
    /// <param name="enteredPassword">Senha digitada</param>
    /// <param name="hashedPassword">Senha salva no banco</param>
    /// <returns></returns>
    public static bool VerifyPassword(string enteredPassword, string hashedPassword)
    {
        var salt = hashedPassword.Substring(0, 88);
        var hash = hashedPassword.Substring(88, hashedPassword.Length - 88);

        var saltBytes = Convert.FromBase64String(salt);
        var rfc2898DerivedBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 100000, HashAlgorithmName.SHA512);

        return Convert.ToBase64String(rfc2898DerivedBytes.GetBytes(256)) == hash;
    }
}
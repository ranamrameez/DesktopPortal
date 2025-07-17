using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DesktopPortal.Helpers;
public static class SecureStorage
{
    private static readonly string DirPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DesktopPortal");

    private static readonly string FilePath = Path.Combine(DirPath, "user.dat");

    public static void Save(string username, string password)
    {
        try
        {
            Directory.CreateDirectory(DirPath); // Ensure directory exists
            string combined = $"{username}:{password}";
            byte[] data = Encoding.UTF8.GetBytes(combined);

            // Encrypt using current user's context
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(FilePath, encrypted);
        }
        catch (Exception)
        {
            // Log or handle error (optional)
        }
    }

    public static (string username, string password)? Load()
    {
        if (!File.Exists(FilePath))
            return null;

        try
        {
            byte[] encrypted = File.ReadAllBytes(FilePath);

            // Decrypt using current user's context
            byte[] data = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            string combined = Encoding.UTF8.GetString(data);
            var parts = combined.Split(':', 2);

            if (parts.Length == 2)
                return (parts[0], parts[1]);
        }
        catch
        {
            // Possibly corrupted file or unauthorized access
        }

        return null;
    }

    public static void Clear()
    {
        try
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
        catch
        {
            // Optional: log error
        }
    }
}
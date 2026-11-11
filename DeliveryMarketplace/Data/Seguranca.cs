using System.Security.Cryptography;
using System.Text;

namespace DeliveryMarketplace.Data;

/// <summary>Gera o hash da senha. A senha em texto puro nunca é gravada no banco.</summary>
public static class Seguranca
{
    public static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes("delivery-poo2:" + senha));
        return Convert.ToHexString(bytes);
    }
}

using System.Security.Cryptography;
using System.Text;

namespace Common.Module.Helpers
{
  public static class EncryptionHelper
  {
    private static string _publicKey =
        "<RSAKeyValue><Modulus>5f1RAIMVLEiNZ0GQd640uPXU0RDUflxrLHXj9a/kWhLxaVFBISDt7pa/GG7cxKr+oC18us1ZHyXXykYjtVX43sK2kxnlXSd7B2oZrhlLbhrO7Z7/Xn1/O2LVn6ywY5eCtUp4exw/UzcXGz2uL+eplEqPeWTSMh8VfluXi5Q3Ytk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    // This constant is used to determine the keysize of the encryption algorithm in bits.
    // We divide this by 8 within the code below to get the equivalent number of bytes.
    private const int Keysize = 128;

    // This constant determines the number of iterations for the password bytes generation function.
    private const int DerivationIterations = 5000;

    public static string Encrypt(string plainText)
    {
      // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
      // so that the same Salt and IV values can be used when decrypting.
      var saltStringBytes = Generate128BitsOfRandomEntropy();
      var ivStringBytes = Generate128BitsOfRandomEntropy();
      var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
      var keyBytes = Rfc2898DeriveBytes.Pbkdf2(
          _publicKey,
          saltStringBytes,
          DerivationIterations,
          HashAlgorithmName.SHA1,
          Keysize / 8);
      using (var symmetricKey = Aes.Create())
      {
        symmetricKey.BlockSize = 128;
        symmetricKey.Mode = CipherMode.CBC;
        symmetricKey.Padding = PaddingMode.PKCS7;
        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
        {
          using (var memoryStream = new MemoryStream())
          {
            using (var cryptoStream =
                new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
              cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
              cryptoStream.FlushFinalBlock();

              // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
              var cipherTextBytes = saltStringBytes;
              cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
              cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
              memoryStream.Close();
              cryptoStream.Close();
              return Repad(Convert.ToBase64String(cipherTextBytes));
            }
          }
        }
      }
    }

    public static string Decrypt(string cipherText)
    {
      // Get the complete stream of bytes that represent:
      // [16 bytes of Salt] + [16 bytes of IV] + [n bytes of CipherText]
      var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
      // Get the saltbytes by extracting the first 16 bytes from the supplied cipherText bytes.
      var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
      // Get the IV bytes by extracting the next 16 bytes from the supplied cipherText bytes.
      var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
      // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
      var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8 * 2)
          .Take(cipherTextBytesWithSaltAndIv.Length - Keysize / 8 * 2).ToArray();

      var keyBytes = Rfc2898DeriveBytes.Pbkdf2(
          _publicKey,
          saltStringBytes,
          DerivationIterations,
          HashAlgorithmName.SHA1,
          Keysize / 8);
      using (var symmetricKey = Aes.Create())
      {
        symmetricKey.BlockSize = 128;
        symmetricKey.Mode = CipherMode.CBC;
        symmetricKey.Padding = PaddingMode.PKCS7;
        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
        {
          using (var memoryStream = new MemoryStream(cipherTextBytes))
          {
            using (var cryptoStream =
                new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
              var plainTextBytes = new byte[cipherTextBytes.Length];
              int totalBytesRead = 0;
              while (totalBytesRead < cipherTextBytes.Length)
              {
                int bytesRead = cryptoStream.Read(plainTextBytes, totalBytesRead, plainTextBytes.Length - totalBytesRead);
                if (bytesRead == 0)
                {
                  break;
                }

                totalBytesRead += bytesRead;
              }

              memoryStream.Close();
              cryptoStream.Close();
              return Encoding.UTF8.GetString(plainTextBytes, 0, totalBytesRead);
            }
          }
        }
      }
    }

    private static byte[] Generate128BitsOfRandomEntropy()
    {
      var randomBytes = new byte[16]; // 16 Bytes will give us 128 bits.
      RandomNumberGenerator.Fill(randomBytes);

      return randomBytes;
    }

    public static string Repad(string base64)
    {
      StringBuilder sb = new StringBuilder();

      var l = base64.Length;
      if (l < 2)
      {
        return base64;
      }

      if (l % 4 == 1 && base64[l - 1] == '=' && base64[l - 2] != '=')
      {
        return sb.Append(base64).Append("=").ToString();
      }
      else
      if (l % 4 == 1 && base64[l - 1] == '=' && base64[l - 2] == '=')
      {
        return base64.Substring(0, l - 1);
      }
      else
      {
        return base64;
      }
    }

    public static byte[] DecryptFileData(byte[] data)
    {
      string sec = "oNy3feHf3DJqggodCWdsyX9BA6jKbCO4e2U7qzUWB0peacUz1ASt/MLXyqmGiDVcrYpj302POH5Bmk+g1Ug474klIpBRnZrJ3pULZ1jkQjOxq5Xx7juBAQWLlOy/7tZN8mKz/kkAsJnr/LUrkO9TC2ZjGaTRcW6yPlhddKnUYG98aZwvoL2llK2R9c8s/mYRCn9UY9vOr1cXjmeLDqwhw91NLVVoG/RgUL00ORmeoL5ful8dorrFNhAWfG/UbFhmsW7kRCVda5LhBzVafCpHb/kh2kRDWp4E+zSr5QQAR9qbdBj7h72xIvZWVyJmdDWhPggpQvpxfyrprtOr72zkLw==";
      string cypher = Decrypt(sec);

      byte[] res = new byte[data.Length];

      res[0] = data[0];

      for (int i = 1; i < data.Length; i++)
      {
        string ch = cypher.Substring(i % cypher.Length, 1);
        res[i] = (byte)(data[i] ^ ch[0]);
      }

      return res;
    }
  }
}


using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace VPortal.Vpn
{

  public class WireGuardKeyGenerator
  {
    public static (string privateKey, string publicKey) GenerateKeyPair()
    {
      var keyPairGenerator = new X25519KeyPairGenerator();
      keyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 255));
      var keyPair = keyPairGenerator.GenerateKeyPair();

      var privateKey = Convert.ToBase64String(((X25519PrivateKeyParameters)keyPair.Private).GetEncoded());
      var publicKey = Convert.ToBase64String(((X25519PublicKeyParameters)keyPair.Public).GetEncoded());

      return (privateKey, publicKey);
    }
  }
}

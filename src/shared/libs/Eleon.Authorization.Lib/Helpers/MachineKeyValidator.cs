using Common.Module.Helpers;
using Common.Module.Keys;
using IdentityModel;

namespace Authorization.Module.MachineKeyValidation
{
  public class MachineKeyValidator
  {
    private static readonly TimeSpan MachineHeaderExpireTime = TimeSpan.FromMinutes(5);

    public static bool ValidateMachineKey(string encrytedMachineKeyWithToken, string encryptedEtalonMachineKey, string token)
    {
      RawCompoundKey machineKeyWithToken = RawCompoundKey.Parse(EncryptionHelper.Decrypt(encrytedMachineKeyWithToken));
      DateTime providedDate = long.Parse(machineKeyWithToken[2]).ToDateTimeFromEpoch();
      if (DateTime.UtcNow.Subtract(providedDate) > MachineHeaderExpireTime)
      {
        return false;
      }

      string providedMachineKey = machineKeyWithToken[0];
      string providedToken = machineKeyWithToken[1];
      string etalonMachineKey = EncryptionHelper.Decrypt(encryptedEtalonMachineKey);

      bool machineKeyMatches = providedMachineKey == etalonMachineKey;
      bool tokenMatches = providedToken == token;
      return machineKeyMatches && tokenMatches;
    }
  }
}

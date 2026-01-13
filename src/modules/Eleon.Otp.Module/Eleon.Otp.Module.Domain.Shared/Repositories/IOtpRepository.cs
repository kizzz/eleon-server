using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Otp.Module.Entities;

namespace VPortal.Otp.Module.Repositories
{
  public interface IOtpRepository : IBasicRepository<OtpEntity, Guid>
  {
    Task<OtpEntity> FindByKey(string key);
    Task<OtpEntity> GetOtpByRecipient(string recipient);
  }
}

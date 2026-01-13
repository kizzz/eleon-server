using Common.Module.Constants;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Infrastructure.Module.Entities;
using VPortal.Shared.Modules.Core.Module.Repositories;

namespace VPortal.Shared.Modules.Core.Module.DomainServices;


public class SeriesDomainService : DomainService, ISingletonDependency
{

  internal static readonly long DefaultNumber = 1;

  private readonly ISeriaNumbersRepository seriaNumbersRepository;
  private readonly IVportalLogger<SeriesDomainService> logger;
  private SemaphoreSlim seriaGenerationLocker = new(1, 1);

  public SeriesDomainService(
      ISeriaNumbersRepository seriaNumbersRepository,
      IVportalLogger<SeriesDomainService> logger)
  {
    this.seriaNumbersRepository = seriaNumbersRepository;
    this.logger = logger;
  }

  public async Task<string> GetLastSeriaNumber(string documentObjectType, string prefix, string refId = null)
  {

    string formattedNumber = string.Empty;
    try
    {
      var seriaNumberEntity = await seriaNumbersRepository.GetSeriaNumberAsync(documentObjectType, refId, prefix);
      if (seriaNumberEntity != null)
      {
        formattedNumber = prefix + seriaNumberEntity.LastUsedNumber.ToString();
      }
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }

    return formattedNumber;
  }

  /// <summary>
  /// Gets the next seria number for a given triplet of identifiers,
  /// then increments the seria afterwards.
  /// The function is thread-safe.
  /// </summary>
  /// <param name="documentObjectType">The document object type that will be used as a part of identifier.</param>
  /// <param name="documentType">The document type that will be used as a part of identifier.</param>
  /// <param name="refId">
  /// The reference identifier that will be used as a part of a serie identifier.
  /// May or may not be null depending on your needs.
  /// Can be any arbitrary string, such as an Id of a document or any other value that will
  /// help you to distinguish the seria in case when multiple series are needed for a
  /// single document object type and document type pair.
  /// </param>
  /// <returns>
  /// A string representation of a seria
  /// with a prefix appropriate for the given parameters and the actual seria number.
  /// </returns>
  public async Task<string> GetNextSeriaNumber(string documentObjectType, string prefix, string refId = null)
  {

    string formattedNumber = string.Empty;
    try
    {
      seriaGenerationLocker.Wait();

      var seriaNumberEntity = await seriaNumbersRepository.GetSeriaNumberAsync(documentObjectType, refId, prefix);
      if (seriaNumberEntity != null)
      {
        seriaNumberEntity.LastUsedNumber++;
        await seriaNumbersRepository.UpdateAsync(seriaNumberEntity, true);
      }
      else
      {
        var id = GuidGenerator.Create();
        seriaNumberEntity = new SeriaNumberEntity(id, prefix, documentObjectType, DefaultNumber)
        {
          RefId = refId,
        };
        await seriaNumbersRepository.InsertAsync(seriaNumberEntity, true);
      }

      formattedNumber = prefix + seriaNumberEntity.LastUsedNumber.ToString();
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
      seriaGenerationLocker.Release();
    }

    return formattedNumber;
  }
}

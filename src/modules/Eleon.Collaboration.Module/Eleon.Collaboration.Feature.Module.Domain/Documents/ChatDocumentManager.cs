using Common.EventBus.Module;
using Commons.Module.Messages.Storage;
using Logging.Module;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;

namespace VPortal.Collaboration.Feature.Module.Documents
{
  public class ChatDocumentManager : ITransientDependency
  {
    private readonly IVportalLogger<ChatDocumentManager> logger;
    private readonly IDistributedEventBus _eventBus;
    private readonly IGuidGenerator guidGenerator;

    public ChatDocumentManager(
        IVportalLogger<ChatDocumentManager> logger,
        IDistributedEventBus eventBus,
        IGuidGenerator guidGenerator)
    {
      this.logger = logger;
      this._eventBus = eventBus;
      this.guidGenerator = guidGenerator;
    }

    public async Task<string> SaveDocument(byte[] document)
    {
      string result = null;
      try
      {

      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}

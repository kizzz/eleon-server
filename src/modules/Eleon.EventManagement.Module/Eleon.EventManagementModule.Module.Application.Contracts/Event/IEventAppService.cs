using Microsoft.AspNetCore.Mvc;
using ModuleCollector.EventManagementModule.EventManagementModule.Module.Application.Contracts.Event;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventManagementModule.Module.Application.Contracts.Event;
public interface IEventAppService : IApplicationService
{
  Task PublishErrorAsync(string message);
  Task PublishAsync(PublishMessageRequestDto input);
  Task<RecieveMessagesResponseDto> ReceiveManyAsync(string queueName, int maxCount = 1000);
  Task<ClaimMessagesResponseDto> ClaimManyAsync(ClaimMessagesRequestDto input);
  Task AckAsync(AckRequestDto input);
  Task NackAsync(NackRequestDto input);
  Task<PagedResultDto<EventDto>> GetListAsync(MessagesPagedAndSortedResultRequestDto input);
  Task<FullEventDto> DownloadMessageAsync(Guid messageId);
}

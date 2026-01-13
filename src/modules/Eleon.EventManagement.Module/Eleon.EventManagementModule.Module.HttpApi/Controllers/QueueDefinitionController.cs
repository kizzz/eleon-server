//using EventManagementModule.Module.Application.Contracts.Queue;
//using EventManagementModule.Module.Application.Contracts.QueueDefenition;
//using GoogleApi.Entities.Search.Video.Common;
//using Logging.Module;
//using Microsoft.AspNetCore.Mvc;
//using Volo.Abp;
//using Volo.Abp.Application.Dtos;
//using VPortal.EventManagementModule.Module;
//using static PInvoke.User32;

//namespace VPortal.EventManagementModule.HttpApi.Controllers;

//[RemoteService(Name = EventManagementRemoteServiceConsts.RemoteServiceName)]
//[Route("api/EventManagement/QueueDefinitions")]
//public class QueueDefinitionController : EventManagementCotroller, IQueueDefinitionAppService
//{
//    private readonly IVportalLogger<QueueDefinitionController> logger;
//    private readonly IQueueDefinitionAppService queueDefenitionAppService;

//    public QueueDefinitionController(
//        IVportalLogger<QueueDefinitionController> logger,
//        IQueueDefinitionAppService queueDefenitionAppService)
//    {
//        this.logger = logger;
//        this.queueDefenitionAppService = queueDefenitionAppService;
//    }

//    [HttpGet("Get/{id}")]
//    public async Task<QueueDefinitionDto> GetAsync(Guid id)
//    {

//        var response = await queueDefenitionAppService.GetAsync(id);


//        return response;
//    }

//    [HttpGet("GetAll")]
//    public async Task<List<QueueDefinitionDto>> GetAllAsync()
//    {

//        var response = await queueDefenitionAppService.GetAllAsync();


//        return response;
//    }

//    [HttpGet("GetList")]
//    public async Task<PagedResultDto<QueueDefinitionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
//    {

//        var response = await queueDefenitionAppService.GetListAsync(input);


//        return response;
//    }

//    [HttpPost("Create")]
//    public async Task<QueueDefinitionDto> CreateAsync(CreateQueueDefinitionRequestDto input)
//    {

//        var response = await queueDefenitionAppService.CreateAsync(input);


//        return response;
//    }

//    [HttpPost("EnsureCreated")]
//    public async Task<QueueDefinitionDto> EnsureCreatedAsync(CreateQueueDefinitionRequestDto input)
//    {

//        var response = await queueDefenitionAppService.EnsureCreatedAsync(input);


//        return response;
//    }

//    [HttpPost("Update")]
//    public async Task<QueueDefinitionDto> UpdateAsync(UpdateQueueDefinitionRequestDto input)
//    {

//        var response = await queueDefenitionAppService.UpdateAsync(input);


//        return response;
//    }

//    [HttpDelete("Delete")]
//    public async Task DeleteAsync(Guid id)
//    {

//        await queueDefenitionAppService.DeleteAsync(id);

//    }
//}

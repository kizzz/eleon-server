using Logging.Module;
using Messaging.Module.ETO;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace VPortal.Collaboration.Feature.Module.DomainServices
{
  // deprecated (must be managed by edi)
  public class DocumentConversationDomainService : DomainService
  {
    private readonly IVportalLogger<DocumentConversationDomainService> logger;
    private readonly IChatRoomRepository chatRoomRepository;
    private readonly ChatMemberDomainService chatMemberDomainService;
    private readonly ChatMessageDomainService chatMessageDomainService;
    private readonly ChatRoomDomainService chatRoomDomainService;

    public DocumentConversationDomainService(
        IVportalLogger<DocumentConversationDomainService> logger,
        IChatRoomRepository chatRoomRepository,
        ChatMemberDomainService chatMemberDomainService,
        ChatMessageDomainService chatMessageDomainService,
        ChatRoomDomainService chatRoomDomainService)
    {
      this.logger = logger;
      this.chatRoomRepository = chatRoomRepository;
      this.chatMemberDomainService = chatMemberDomainService;
      this.chatMessageDomainService = chatMessageDomainService;
      this.chatRoomDomainService = chatRoomDomainService;
    }

    public async Task<ChatRoomEntity> CreateDocumentConversation(string docType, string documentId)
    {
      string refId = GetConversationRefId(docType, documentId);
      return await chatRoomDomainService.CreateChatAsync(string.Empty, refId, ChatRoomType.Group, null, setOwner: false);
    }

    public async Task SendDocumentMessages(List<DocumentChatMessageEto> messages)
    {
      foreach (var msg in messages)
      {
        var chat = await EnsureDocumentConversationExists(msg.DocumentObjectType, msg.DocumentId.ToString());
        await chatMessageDomainService.AddLocalizedSystemMessage(chat.Id, msg.LocalizationKey, msg.LocalizationParams, msg.MessageSeverity);
      }
    }

    public async Task<DocumentConversationInfo> GetDocumentConversationInfo(string docType, string documentId)
    {
      string refId = GetConversationRefId(docType, documentId);
      var chat = await chatRoomRepository.GetByRefId(refId);
      if (chat != null)
      {
        var isMember = await chatMemberDomainService.CheckMembership(chat.Id);
        return new DocumentConversationInfo()
        {
          ChatRoom = chat,
          IsMember = isMember,
        };
      }
      else
      {
        var newChat = await CreateDocumentConversation(docType, documentId.ToString());
        var isMember = await chatMemberDomainService.CheckMembership(newChat.Id);
        return new DocumentConversationInfo()
        {
          ChatRoom = newChat,
          IsMember = isMember,
        };
      }
    }

    private async Task<ChatRoomEntity> EnsureDocumentConversationExists(string docType, string documentId)
    {
      string refId = GetConversationRefId(docType, documentId.ToString());
      var existing = await chatRoomRepository.GetByRefId(refId);
      if (existing != null)
      {
        return existing;
      }

      return await CreateDocumentConversation(docType, documentId);
    }

    private string GetConversationRefId(string docType, string documentId)
        => $"DocumentConversation;{docType};{documentId}";
  }
}

namespace VPortal.Collaboration.Feature.Module.Chats
{
  public class ChatRenamedMessage
  {
    public ChatMemberInfo RenamedByUser { get; set; }
    public string OldName { get; set; }
    public string NewName { get; set; }
  }
}

using System;
using System.Collections.Generic;

namespace VPortal.Collaboration.Feature.Module.SupportTickets
{
  public class CreateSupportTicketRequestDto
  {
    public string Title { get; set; }
    public List<Guid> InitialMembers { get; set; }
  }
}

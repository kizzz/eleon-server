using Common.Module.Constants;
using Eleon.Templating.Module.Templates;
using Volo.Abp.Domain.Repositories;

namespace Eleon.Templating.Module.Templates;

public interface ITemplateRepository : IRepository<Template, Guid>
{
  Task<KeyValuePair<int, List<Template>>> GetListAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      string searchQuery = null,
      TemplateType? type = null,
      TextFormat? format = null
    );
  Task<Template?> FindByNameAndTypeAsync(string name, TemplateType type, CancellationToken cancellationToken = default);
  Task<List<Template>> GetByTypeAndIsSystemAsync(TemplateType type, bool isSystem, CancellationToken cancellationToken = default);
}


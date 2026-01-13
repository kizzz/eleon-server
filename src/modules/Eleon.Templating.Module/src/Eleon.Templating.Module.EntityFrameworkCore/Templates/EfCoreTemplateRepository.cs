using Common.Module.Constants;
using Eleon.Templating.Module.Templates;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Eleon.Templating.Module.EntityFrameworkCore.Templates;

public class EfCoreTemplateRepository : EfCoreRepository<TemplatingDbContext, Template, Guid>, ITemplateRepository
{
  private readonly IVportalLogger<EfCoreTemplateRepository> _logger;
  public EfCoreTemplateRepository(IDbContextProvider<TemplatingDbContext> dbContextProvider, IVportalLogger<EfCoreTemplateRepository> vportalLogger)
        : base(dbContextProvider)
  {
    _logger = vportalLogger;
  }

  public async Task<Template?> FindByNameAndTypeAsync(
      string name,
      TemplateType type,
      CancellationToken cancellationToken = default)
  {
    var dbContext = await GetDbContextAsync();
    return await dbContext.Set<Template>()
        .FirstOrDefaultAsync(t => t.Name == name && t.Type == type, cancellationToken);
  }

  public async Task<List<Template>> GetByTypeAndIsSystemAsync(
      TemplateType type,
      bool isSystem,
      CancellationToken cancellationToken = default)
  {
    var dbContext = await GetDbContextAsync();
    return await dbContext.Set<Template>()
        .Where(t => t.Type == type && t.IsSystem == isSystem)
        .ToListAsync(cancellationToken);
  }

  public async Task<KeyValuePair<int, List<Template>>> GetListAsync(string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, string searchQuery = null, TemplateType? type = null, TextFormat? format = null)
  {
    KeyValuePair<int, List<Template>> result = default;
    try
    {
      var dbSet = await GetDbSetAsync();

      string searchPattern = searchQuery == null ? null : $"%{searchQuery}%";
      var query = dbSet
          .WhereIf(type != null, x => x.Type == type)
          .WhereIf(format != null, x => x.Format == format)
          .WhereIf(searchQuery != null, x => EF.Functions.Like(x.Name, searchPattern));


      if (!string.IsNullOrEmpty(sorting))
      {
        query = query.OrderBy(sorting);
      }
      else
      {
        query = query.OrderByDescending(x => x.CreationTime);
      }

      var paged = query
          .Skip(skipCount)
          .Take(maxResultCount);

      var entities = await paged.ToListAsync();
      var count = await query.CountAsync();

      result = new(count, entities);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }
}

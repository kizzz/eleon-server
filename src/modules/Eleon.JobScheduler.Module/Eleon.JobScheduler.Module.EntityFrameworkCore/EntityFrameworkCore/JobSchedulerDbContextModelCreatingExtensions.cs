using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.EntityFrameworkCore;

public static class JobSchedulerDbContextModelCreatingExtensions
{
  public static void ConfigureJobScheduler(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));

    builder.Entity<TaskEntity>()
        .HasKey(x => x.Id);

    builder.Entity<TaskEntity>()
        .HasMany(x => x.Executions)
        .WithOne(x => x.Task)
        .HasForeignKey(x => x.TaskId);

    builder.Entity<TaskEntity>()
        .HasMany(x => x.Actions)
        .WithOne(x => x.Task)
        .HasForeignKey(x => x.TaskId);

    builder.Entity<TaskEntity>()
        .HasMany(x => x.Triggers)
        .WithOne(x => x.Task)
        .HasForeignKey(x => x.TaskId);

    builder.Entity<TaskExecutionEntity>()
        .HasMany(x => x.ActionExecutions)
        .WithOne(x => x.TaskExecution)
        .HasForeignKey(x => x.TaskExecutionId);

    builder.Entity<ActionExecutionParentEntity>(b =>
    {
      b.HasKey(x => new { x.ChildActionExecutionId, x.ParentActionExecutionId });
      b.HasOne<ActionExecutionEntity>().WithMany(x => x.ParentActionExecutions).HasForeignKey(x => x.ChildActionExecutionId).OnDelete(DeleteBehavior.Cascade);
      b.HasOne<ActionExecutionEntity>().WithMany().HasForeignKey(x => x.ParentActionExecutionId).OnDelete(DeleteBehavior.Restrict);
    });

    builder.Entity<ActionParentEntity>(b =>
    {
      b.HasKey(x => new { x.ChildActionId, x.ParentActionId });
      b.HasOne<ActionEntity>().WithMany(x => x.ParentActions).HasForeignKey(x => x.ChildActionId).OnDelete(DeleteBehavior.Cascade);
      b.HasOne<ActionEntity>().WithMany().HasForeignKey(x => x.ParentActionId).OnDelete(DeleteBehavior.Restrict);
    });

    builder.Entity<ActionEntity>()
        .HasMany<ActionExecutionEntity>()
        .WithOne(x => x.Action)
        .HasForeignKey(x => x.ActionId);
  }
}

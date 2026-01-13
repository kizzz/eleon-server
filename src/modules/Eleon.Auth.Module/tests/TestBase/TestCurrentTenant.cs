using System;
using Volo.Abp.MultiTenancy;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;

public sealed class TestCurrentTenant : ICurrentTenant
{
    public Guid? Id { get; private set; }
    public string Name { get; private set; }
    public bool IsAvailable => true;

    public IDisposable Change(Guid? tenantId, string name = null)
    {
        var originalId = Id;
        var originalName = Name;
        Id = tenantId;
        Name = name;
        return new DisposeAction(() =>
        {
            Id = originalId;
            Name = originalName;
        });
    }

    private sealed class DisposeAction : IDisposable
    {
        private readonly Action _dispose;
        public DisposeAction(Action dispose) => _dispose = dispose;
        public void Dispose() => _dispose();
    }
}

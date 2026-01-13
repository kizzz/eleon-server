namespace Eleon.Application.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Eleon.Application.Services;
    using Eleon.Domain.Entities;
    using Eleon.Messaging;
    using Eleon.Persistence;
    using Eleon.Tests.Common;
    using Xunit;

    internal sealed class FakeRepo : IRepository<Product, Guid>
    {
        private readonly List<Product> products = new();

        public IUnitOfWork UnitOfWork => new FakeUnitOfWork();

        public Task AddAsync(Product entity, CancellationToken ct = default)
        {
            products.Add(entity);
            return Task.CompletedTask;
        }

        public Task<Product?> GetAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(products.Find(p => p.Id == id));

        public Task<IReadOnlyList<Product>> ListAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Product>>(products);

        public Task<IReadOnlyList<Product>> ListAsync(ISpecification<Product> spec, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Product>>(products);

        public Task RemoveAsync(Product entity, CancellationToken ct = default) => Task.CompletedTask;

        public Task UpdateAsync(Product entity, CancellationToken ct = default) => Task.CompletedTask;

        private sealed class FakeUnitOfWork : IUnitOfWork
        {
            public Task<int> SaveChangesAsync(CancellationToken ct = default) => Task.FromResult(1);
        }
    }

    internal sealed class FakeBus : IIntegrationEventBus
    {
        public Task PublishAsync<T>(T evt, CancellationToken ct = default)
            where T : class => Task.CompletedTask;
    }

    public class AppServiceSpec : TestBase
    {
        [Fact]
        public async Task Create_is_async_and_returns_task()
        {
            var service = new ProductAppService(new FakeRepo(), new FakeBus());
            var dto = await service.CreateAsync("B", 2m, DefaultCancellationToken);
            Assert.Equal("B", dto.Name);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace VPortal.EventManagementModule.Module.Tests.TestHelpers;

public static class DbSetMockExtensions
{
  public static DbSet<T> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
  {
    var mock = new Mock<DbSet<T>>();

    mock.As<IAsyncEnumerable<T>>()
        .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
        .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

    mock.As<IQueryable<T>>()
        .Setup(m => m.Provider)
        .Returns(new TestAsyncQueryProvider<T>(data.Provider));
    mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
    mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

    return mock.Object;
  }

  private sealed class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
  {
    private readonly IQueryProvider inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
      this.inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
        => new TestAsyncEnumerable<TEntity>(StripIncludes(expression));

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(StripIncludes(expression));

    public object Execute(Expression expression) => inner.Execute(StripIncludes(expression));
    public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(StripIncludes(expression));

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        => Execute<TResult>(expression);

    private static Expression StripIncludes(Expression expression)
        => new IncludeStripper().Visit(expression) ?? expression;
  }

  private sealed class IncludeStripper : ExpressionVisitor
  {
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
      if (node.Method.DeclaringType == typeof(EntityFrameworkQueryableExtensions) &&
          (node.Method.Name == nameof(EntityFrameworkQueryableExtensions.Include) ||
           node.Method.Name == nameof(EntityFrameworkQueryableExtensions.ThenInclude)))
      {
        return Visit(node.Arguments[0]);
      }

      return base.VisitMethodCall(node);
    }
  }

  private sealed class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
  {
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
    {
    }

    public TestAsyncEnumerable(Expression expression) : base(expression)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
  }

  private sealed class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
  {
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
      _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());

    public ValueTask DisposeAsync()
    {
      _inner.Dispose();
      return ValueTask.CompletedTask;
    }
  }
}
